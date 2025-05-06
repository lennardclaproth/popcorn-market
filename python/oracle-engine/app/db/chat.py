from datetime import datetime
from db.mongo_client import db

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
