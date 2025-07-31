from enum import IntEnum
from typing import ForwardRef, Optional
from pydantic import BaseModel

Node = ForwardRef('Node')

class EventType(IntEnum):
    COMPANY_CREATED = 0
    ARTICLE_PUBLISHED = 1
    FINANCIAL_STATEMENT_PUBLISHED = 2
    MARKET_DATA_PUBLISHED = 3


class NodeMetadata(BaseModel):
    service: str
    event_type: Optional[EventType] = None
    kvp: Optional[dict] = None

class Node(BaseModel):
    entity_id: str
    metadata: NodeMetadata
    children: list[str]
