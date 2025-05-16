import logging
from db import generator as generator_db
from generators.registry import REGISTRY
from models.generator import Generator

"""
The service for managing and fetching generators
"""

logger = logging.getLogger("worker_app")

def fetch_generators() -> list[Generator]:
    return generator_db.get_active_generators()

def seed_generators():
    """
    Seeds the generators that are available. 
    """
    logger.info("Seeding generators...")
    generators = generator_db.get_active_generators()
    existing_reprs = {generator.representation for generator in generators}
    REGISTRY

    missing = []

    for representation, meta in REGISTRY.items():
        if representation not in existing_reprs:
            generator_cls = meta["generator"]
            missing.append(generator_cls)

    if(missing):
        generator_db.insert_many(missing)
        logger.info("Successfully seeded generators.")
    else:
        logger.info("All generators have already been registered. No new generators added.")
        
    
