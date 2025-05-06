import logging
from fastapi import APIRouter, HTTPException

logger = logging.getLogger("worker_app")

router = APIRouter(
    prefix='/generator',
    tags=['generator']
)

