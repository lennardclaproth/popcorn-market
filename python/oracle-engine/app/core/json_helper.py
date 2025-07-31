import json
import re

def extract_json(response_text):
    response_text = response_text.strip()

    # --- EXTRACT JSON FROM RESPONSE TEXT ---
    match = re.search(r"```json\s*(\{.*?\})\s*```", response_text, re.DOTALL)
    if match:
        json_str = match.group(1).strip()
    else:
        match = re.search(r"(\{.*?\})", response_text, re.DOTALL)
        if not match:
            raise ValueError("No JSON object found in response text.")

        json_str = match.group(1).strip()

    # --- SANITIZE JSON ---

    # Normalize quotes to standard double quotes
    json_str = __normalize_unicode_quotes(json_str)

    # Remove trailing commas before closing brackets
    json_str = re.sub(r",\s*([}\]])", r"\1", json_str)

    # --- TRY LOAD JSON ---
    try:
        return json.loads(json_str)
    except json.JSONDecodeError as e:
        raise ValueError(f"JSON decoding failed: {e}\n\nRaw string:\n{json_str}") from e
    
def __normalize_unicode_quotes(text: str) -> str:
    return (
        text
        .replace("“", '"')
        .replace("”", '"')
        .replace("‘", "'")
        .replace("’", "'")
    )
