from typing import Optional
from fastapi import APIRouter
from fastapi.params import Query
from services import chat

router = APIRouter(
    prefix='/chat',
    tags=['chat']
)

@router.get("")
def get_chat(limit: Optional[int] = Query(default=10, ge=1, le=100)):
    chats = chat.fetch_with_limit(limit)
    return chats