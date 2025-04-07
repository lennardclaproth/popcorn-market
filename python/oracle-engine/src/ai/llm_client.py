import json
from typing import Any
from ollama import ChatResponse, chat
from config import OLLAMA_MODEL
from helpers.json_helper import extract_json

def execute_prompt(prompt: str) -> dict[str, str] | Any:
    """
    Sends the prompt to ollama or to the api running ollama.
    """

    response: ChatResponse = chat(model=OLLAMA_MODEL, messages=[
    {'role': 'user', 'content': prompt},
    ])

    try:
        return extract_json(response.message.content)
    except json.JSONDecodeError:
        print("Error: Could not parse JSON. Returning empty object.")
        return {"error": "Invalid JSON format"}
    