import logging
from fastapi import APIRouter
from services.generator import fetch_generators, set_probability

logger = logging.getLogger("worker_app")

router = APIRouter(
    prefix='/generator',
    tags=['generator']
)

@router.get("")
def get_generators():
    generators = fetch_generators()
    return generators

@router.patch("{generator_id}/probability")
def patch_generator_probability():
    

# def patch_generator_probability():
