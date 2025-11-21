import logging
import threading
from typing import Annotated
from fastapi import APIRouter, HTTPException
from fastapi.params import Query
from services.worker import (
    start_oracle_pool,
    start_analysis_pool,
    stop_oracle_pool,
    stop_analysis_pool,
    get_oracle_status,
    get_analysis_status,
)

logger = logging.getLogger("worker_app")

router = APIRouter(
    prefix="/pools",
    tags=["pools"]
)

@router.post("/start-oracle")
def start_oracle(
        workers: Annotated[int, Query(gt=0, le=64)] = 1
):
    started = start_oracle_pool(workers)
    if not started:
        logger.warning("Tried to start oracle, but it's already running.")
        raise HTTPException(status_code=400, detail="Oracle pool already running")
    logger.info("Oracle pool has been started.")
    return {"status": "oracle started"}

@router.post("/start-analysis")
def start_analysis(
        workers: Annotated[int, Query(gt=0, le=64)] = 1
    ):
    started = start_analysis_pool(workers)
    if not started:
        logger.warning("Tried to start analysis, but it's already running.")
        raise HTTPException(status_code=400, detail="Analysis pool already running")
    logger.info("Analysis pool has been started.")
    return {"status": "Analysis started"}

@router.post("/stop")
def stop_all():
    oracle_stopped = stop_oracle_pool()
    analysis_stopped = stop_analysis_pool()

    if not oracle_stopped and not analysis_stopped:
        logger.warning("Tried to stop workers, but none are running.")
        raise HTTPException(status_code=400, detail="No workers running")

    logger.info(
        "Stopped workers (oracle_stopped=%s, analysis_stopped=%s)",
        oracle_stopped,
        analysis_stopped,
    )
    return {
        "status": "Stopped",
        "oracle_stopped": oracle_stopped,
        "analysis_stopped": analysis_stopped,
    }

@router.get("/status")
def worker_status():
    oracle = get_oracle_status()
    analysis = get_analysis_status()
    logger.info("Status check: oracle=%s, analysis=%s", oracle, analysis)
    return {
        "oracle": oracle,
        "analysis": analysis,
    }