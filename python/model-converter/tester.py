import torch
import numpy as np
from transformers import AutoTokenizer, AutoConfig
from optimum.onnxruntime import ORTModelForSequenceClassification
import onnxruntime as ort

# Load model and tokenizer
save_directory = "onnx/"
tokenizer = AutoTokenizer.from_pretrained(save_directory)
ort_model = ORTModelForSequenceClassification.from_pretrained(save_directory)
config = AutoConfig.from_pretrained("ProsusAI/finbert")
print(config.id2label)


# Run ONNX inference directly
session = ort.InferenceSession(f"{save_directory}/model.onnx")

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

def run_test(text: str):
    chunks = chunk_text(text)
    all_probs = []

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
        probs = torch.nn.functional.softmax(torch.tensor(logits), dim=-1)
        all_probs.append(probs)

    # Average all probabilities
    avg_probs = torch.mean(torch.cat(all_probs, dim=0), dim=0)
    print(all_probs)
    labels = ["Positive", "Negative", "Neutral"]
    sentiment = labels[avg_probs.argmax()]
    print(f"Sentiment: {sentiment}")
    print(f"Probabilities: {avg_probs}")
    return sentiment, avg_probs
