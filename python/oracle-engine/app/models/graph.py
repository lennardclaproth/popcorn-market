from typing import ForwardRef, Optional
from pydantic import BaseModel

Node = ForwardRef('Node')

class NodeMetadata(BaseModel):
    service: str
    kvp: Optional[dict] = None

class Node(BaseModel):
    entity_id: str
    metadata: NodeMetadata
    children: list[str]
