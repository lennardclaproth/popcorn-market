import logging

from fastapi import FastAPI
from fastapi.concurrency import asynccontextmanager
from logging_config import LOGGING_CONFIG
from routers import worker, generator, chat
from services.generator import seed_generators

logging.config.dictConfig(LOGGING_CONFIG)
logger = logging.getLogger("worker_app")


@asynccontextmanager
async def lifespan(app:FastAPI):
    seed_generators()
    yield

app = FastAPI(lifespan=lifespan)

routers = [worker.router, generator.router, chat.router]

for router in routers:
    app.include_router(router)
