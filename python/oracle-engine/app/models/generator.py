from bson import ObjectId
from pydantic import BaseModel, Field

class Generator(BaseModel):
    active: bool
    description: str
    representation: str
    name: str
    probability: float

    class Config:
        populate_by_name = True
        extra = "ignore"