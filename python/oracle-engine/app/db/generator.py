from bson import ObjectId
from db.mongo_client import db
from models.generator import Generator

collection = db["generator"]

def get_active_generators() -> list[Generator]:
    generators = collection.find({"active":True})
    return [Generator(**doc) for doc in generators]

def insert_many(generators: list[Generator]):
    docs = [g.model_dump(by_alias=True) for g in generators]
    return collection.insert_many(docs)

def update_one(generator: Generator):
    if not generator.id:
        raise ValueError("Generator must have an ID to update.")
    
    update_date = generator.model_dump(by_alias=True, exclude={"id"})

    result = collection.update_one(
        {"_id": ObjectId(generator.id)},
        {"$set": update_date}
    )
    return result
