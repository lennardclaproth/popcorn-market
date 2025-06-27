from datetime import datetime
from db.mongo_client import db
from models.chat import Chat
collection = db["chat"]

def save_chat(question: str, answer: str):
    """
    Saves chat in mongodb.
    """
    doc = {
        "question": question,
        "answer": answer,
        "timestamp": datetime.now()
    }
    collection.insert_one(doc)

def fetch_with_limit(limit=10):
    cursor = collection.find().limit(limit)
    return [Chat(**chat) for chat in cursor]
    