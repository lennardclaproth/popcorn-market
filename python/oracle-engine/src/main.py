import logging
from fastapi import FastAPI, HTTPException
import threading
from logging_config import LOGGING_CONFIG
from background_worker import run
from background_worker import stop_event, worker_thread

logging.config.dictConfig(LOGGING_CONFIG)
logger = logging.getLogger("worker_app")

app = FastAPI()

@app.post("/start")
def start_worker():
    global worker_thread
    if worker_thread is not None and worker_thread.is_alive():
        logger.warning("Tried to start worker, but it's already running.")
        raise HTTPException(status_code=400, detail="Worker already running")

    stop_event.clear()
    worker_thread = threading.Thread(target=run, daemon=True)
    worker_thread.start()
    logger.info("Worker thread has been started.")
    return {"status": "Worker started"}

@app.post("/stop")
def stop_worker():
    if worker_thread is None or not worker_thread.is_alive():
        logger.warning("Tried to stop worker, but no worker is running.")
        raise HTTPException(status_code=400, detail="No worker running")
    
    stop_event.set()
    worker_thread.join()
    logger.info("Worker has been stopped.")
    return {"status": "Worker stopped"}

@app.get("/status")
def worker_status():
    running = worker_thread is not None and worker_thread.is_alive()
    logger.info(f"Status check: running = {running}")
    return {"running": running}