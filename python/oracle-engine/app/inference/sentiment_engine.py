import torch
import numpy as np
from transformers import AutoTokenizer, AutoConfig
from optimum.onnxruntime import ORTModelForSequenceClassification
import onnxruntime as ort
from os.path import dirname
import torch.nn.functional as F

# Load model and tokenizer
save_directory = "finbert-local-onnx/"
tokenizer = AutoTokenizer.from_pretrained(f'{dirname(__file__)}/{save_directory}')
config = AutoConfig.from_pretrained("ProsusAI/finbert")

# Run ONNX inference directly
session = ort.InferenceSession(f"{dirname(__file__)}/{save_directory}model.onnx")

def chunk_text(text, max_length=512, overlap=50):
    tokens = tokenizer.encode(text)
    chunks = []
    start = 0
    while start < len(tokens):
        end = min(start + max_length, len(tokens))
        chunk = tokens[start:end]
        chunks.append(chunk)
        if end == len(tokens):
            break
        start += max_length - overlap
    return [tokenizer.decode(c) for c in chunks]

def infer(text: str):
    chunks = chunk_text(text)
    all_probs = []
    labels = ["positive", "negative", "neutral"]

    for chunk in chunks:
        inputs = tokenizer(chunk, return_tensors="np", truncation=True, padding=True)
        inputs["token_type_ids"] = np.zeros_like(inputs["input_ids"])

        outputs = session.run(
            None,
            {
                "input_ids": inputs["input_ids"],
                "attention_mask": inputs["attention_mask"],
                "token_type_ids": inputs["token_type_ids"],
            },
        )

        logits = outputs[0]
        probs = F.softmax(torch.tensor(logits), dim=-1)
        all_probs.append(probs)

    # Average across all text chunks
    avg_probs = torch.mean(torch.cat(all_probs, dim=0), dim=0)

    # Convert tensor → Python list
    probs_list = avg_probs.detach().cpu().numpy().tolist()

    # Map labels to probabilities
    result = dict(zip(labels, probs_list))

    # Get most likely sentiment
    sentiment = max(result, key=result.get)

    return sentiment, result