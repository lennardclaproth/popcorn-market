from bson import ObjectId
from db.mongo_client import db
from models.generator import Generator

collection = db["generator"]

def get_active_generators() -> list[Generator]:
    generators = collection.find({"active":True})
    return [Generator(**doc) for doc in generators]

def insert_many(generators: list[Generator]):
    gen = [g.model_dump(by_alias=True) for g in generators]
    return collection.insert_many(gen)

def update_one(generator: Generator):
    if not generator.id:
        raise ValueError("Generator must have an ID to update.")
    
    update_date = generator.model_dump(by_alias=True, exclude={"id"})

    result = collection.update_one(
        {"_id": ObjectId(generator.id)},
        {"$set": update_date}
    )
    return result

def get_by_id(id: str):
    generator = collection.find_one({"_id":ObjectId(id)})
    if generator is None:
        return None
    
    return Generator(**generator)