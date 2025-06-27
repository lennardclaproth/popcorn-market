from datetime import datetime
from pydantic import BaseModel, ConfigDict, Field


class Chat(BaseModel):
    question: str
    answer: str
    timestamp: datetime

    model_config = ConfigDict(
        arbitrary_types_allowed=True,
        populate_by_name=True,
        extra="ignore",
    )
    