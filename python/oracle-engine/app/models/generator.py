from typing import Optional
from bson import ObjectId
from pydantic import BaseModel, ConfigDict, Field

class Generator(BaseModel):
    id: Optional[ObjectId] = Field(default=None, alias="_id")
    active: bool
    description: str
    representation: str
    name: str
    probability: float

    model_config = ConfigDict(
        arbitrary_types_allowed=True,
        populate_by_name=True,
        extra="ignore",
        json_encoders={ObjectId: str},
    )