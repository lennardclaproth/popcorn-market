from db.mongo_client import db
from models.graph import Node

collection = db["graph"]

def insert_node(node: Node):
    return collection.insert_one(node.model_dump(by_alias=True))

def find_by_entity_id(entity_id: str) -> Node:
    node = collection.find_one({"entity_id":entity_id})
    if node is None:
        return None
    return Node(**node)
    