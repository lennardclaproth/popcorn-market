from datetime import datetime
from config import MONGO_URL
from pymongo import MongoClient

client = MongoClient(MONGO_URL)
db = client["popcornOracleEngine"]