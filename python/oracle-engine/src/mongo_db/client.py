from datetime import datetime
from config import MONGO_URL
from pymongo import MongoClient

client = MongoClient(MONGO_URL)
db = client["popcornOracleEngine"]
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
