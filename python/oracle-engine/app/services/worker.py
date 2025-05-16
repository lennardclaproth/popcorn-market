"""
04/05/2025 Lennard Claproth

This file is responsible for managing the worker.
The worker is the entry point of the loop of generating 
data via de chat service. The worker fetches the list of
generators that are available in the database and executes
the generator function based on the generator it has.
"""

import asyncio
import logging
import random
import threading

from services.generator import fetch_generators
from generators.registry import REGISTRY

# Global control flag
stop_event = threading.Event()
worker_thread = None
logger = logging.getLogger("worker_app")

def run():
    """
    Runs a background worker that generates data based on the current
    loop and so on the worker is in.
    """
    
    logger.info("Worker started.")
    while not stop_event.is_set():
        try:
            generators = fetch_generators()
            logger.info("Starting generation...")
            generator = random.choices(generators, weights=(g.probability for g in generators), k=1)
            repr = REGISTRY.get(generator[0].representation)
            fn = repr.get('func')
            fn()
        except Exception as e:
            logger.exception(e)
    logger.info("Worker received stop signal and is exiting.")
