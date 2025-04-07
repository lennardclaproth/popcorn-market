import json
import re

def extract_json(response_text):
    # Step 1: Remove markdown or extra formatting noise
    response_text = response_text.strip()

    # Step 2: Extract JSON inside triple backticks (most common format)
    json_match = re.search(r"```json\s*\n*(\{.*?\})\s*```", response_text, re.DOTALL)

    if json_match:
        json_str = json_match.group(1).strip()
    else:
        # Step 3: If no triple backticks, extract first valid JSON block
        json_match = re.search(r"(\{.*?\})", response_text, re.DOTALL)
        json_str = json_match.group(1).strip() if json_match else "{}"

    # Step 4: Fix common JSON formatting issues
    json_str = re.sub(r",\s*}", "}", json_str)  # Fix trailing commas

    # Step 5: Parse JSON safely
    
    parsed_json = json.loads(json_str)

    return parsed_json
