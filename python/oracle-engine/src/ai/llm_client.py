import json
import logging
from typing import Any
from mongo_db import client
from ollama import ChatResponse, chat
from config import OLLAMA_MODEL
from helpers.json_helper import extract_json

logger = logging.getLogger("worker_app")

def execute_prompt(prompt: str) -> dict[str, str] | Any:
    """
    Sends the prompt to ollama or to the api running ollama.
    """

    response: ChatResponse = chat(model=OLLAMA_MODEL, messages=[
    {'role': 'user', 'content': prompt},
    ])

    client.save_chat(prompt, response.message.content)

    return extract_json(response.message.content)
    