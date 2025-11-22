"""
04/05/2025 Lennard Claproth

This file is responsible for managing the worker.
The worker is the entry point of the loop of generating 
data via de chat service. The worker fetches the list of
generators that are available in the database and executes
the generator function based on the generator it has.
"""

from concurrent.futures import Future, ThreadPoolExecutor
import logging
import random
import threading
from typing import Any

from generators import analysis_generator

from services.generator import fetch_generators
from generators.registry import REGISTRY
from apm.client import client as apm_client

logger = logging.getLogger("worker_app")

oracle_pool: ThreadPoolExecutor | None = None
analysis_pool: ThreadPoolExecutor | None = None

oracle_futures: list[Future] | None = None
analysis_futures: list[Future] | None = None

oracle_stop_event = threading.Event()
analysis_stop_event = threading.Event()

oracle_pool_size: int | None = None
analysis_pool_size: int | None = None

def start_oracle_pool(oracle_pool_size_param: int = 1) -> bool:
    global oracle_pool, oracle_futures, oracle_pool_size

    if oracle_futures is not None and oracle_futures.running():
        logger.warning("Tried to start oracle pool, but it's already running.")
        return False

    logger.info("Starting oracle pool (oracle_pool_size=%d)", oracle_pool_size_param)

    oracle_stop_event.clear()
    oracle_pool = ThreadPoolExecutor(
        max_workers=oracle_pool_size_param,
        thread_name_prefix="oracle-pool",
    )
    oracle_pool_size = oracle_pool_size_param
    oracle_futures = [
        oracle_pool.submit(_run_oracle_loop)
        for _ in range(oracle_pool_size)
    ]
    return True


def start_analysis_pool(analysis_pool_size_param: int = 1) -> bool:
    global analysis_pool, analysis_futures, analysis_pool_size

    if analysis_futures is not None and analysis_futures.running():
        logger.warning("Tried to start analysis pool, but it's already running.")
        return False

    logger.info("Starting analysis pool (analysis_pool_size=%d)", analysis_pool_size_param)

    analysis_stop_event.clear()
    analysis_pool = ThreadPoolExecutor(
        max_workers=analysis_pool_size_param,
        thread_name_prefix="analysis-pool",
    )
    analysis_pool_size = analysis_pool_size_param
    analysis_futures = [
        analysis_pool.submit(_run_analysis_loop)
        for _ in range(analysis_pool_size)
    ]
    return True

def _run_oracle_loop():
    """
    Runs a background worker that generates data based on the current
    loop and so on the worker is in.
    """

    logger.info("Worker started.")
    while not oracle_stop_event.is_set():
        apm_client.begin_transaction("worker")
        try:
            generators = fetch_generators()
            logger.info("Starting generation...")
            generator = random.choices(generators, weights=(g.probability for g in generators), k=1)
            repr = REGISTRY.get(generator[0].representation)
            fn = repr.get('func')
            fn()
        except Exception:
            logger.exception("An error occurred")
            apm_client.capture_exception()
        finally:
            apm_client.end_transaction("generator.generate")
    logger.info("Worker received stop signal and is exiting.")
    

def _run_analysis_loop():
    logger.info("Analysis started")
    while not analysis_stop_event.is_set():
        apm_client.begin_transaction("worker")
        try:
            analysis_generator.generate()
        except Exception:
            logger.exception("An error occurred")
            apm_client.capture_exception()
        finally:
             apm_client.end_transaction("analysis_generator.generate")
    logger.info("Worker received stop signal and is exiting.")

def stop_oracle_pool() -> bool:
    global oracle_pool, oracle_futures, oracle_pool_size

    if oracle_futures is None or oracle_pool is None:
        return False

    if oracle_stop_event.is_set():
        return False

    logger.info("Stopping oracle pool...")
    oracle_stop_event.set()
    for future in oracle_futures:
        future.result()
        
    oracle_pool.shutdown(wait=False)

    oracle_pool = None
    oracle_futures = None
    oracle_pool_size = None
    return True

def stop_analysis_pool() -> bool:
    global analysis_pool, analysis_futures, analysis_pool_size

    if analysis_futures is None or analysis_pool is None:
        return False

    if analysis_stop_event.is_set():
        return False

    logger.info("Stopping analysis pool...")
    analysis_stop_event.set()
    for future in analysis_futures:
        future.result()

    analysis_pool.shutdown(wait=False)

    analysis_pool = None
    analysis_futures = None
    analysis_pool_size = None
    return True

def _futures_status(futures: list[Future] | None) -> dict[str, Any]:
    if not futures:
        return {
            "running": False,
            "done": False,
            "workers": [],
        }

    workers = []
    for idx, f in enumerate(futures):
        exc = None
        if f.done():
            try:
                exc = f.exception()
            except Exception as e:
                exc = e

        workers.append(
            {
                "id": idx,
                "running": f.running(),
                "done": f.done(),
                "cancelled": f.cancelled(),
                "exception": repr(exc) if exc is not None else None,
            }
        )

    return {
        "running": any(f.running() for f in futures),
        "done": all(f.done() for f in futures),
        "workers": workers,
    }

def get_oracle_status() -> dict:
    futures_info = _futures_status(oracle_futures)
    return {
        "running": futures_info["running"],
        "done": futures_info["done"],
        "stop_requested": oracle_stop_event.is_set(),
        "pool_size": oracle_pool_size,
        "workers": futures_info["workers"],
    }

def get_analysis_status() -> dict:
    futures_info = _futures_status(analysis_futures)
    return {
        "running": futures_info["running"],
        "done": futures_info["done"],
        "stop_requested": analysis_stop_event.is_set(),
        "pool_size": analysis_pool_size,
        "workers": futures_info["workers"],
    }