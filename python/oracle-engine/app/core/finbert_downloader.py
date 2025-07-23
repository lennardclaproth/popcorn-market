from transformers import AutoTokenizer, AutoModelForSequenceClassification

model_name = "ProsusAI/finbert"
save_path = "./finbert_local"

tokenizer = AutoTokenizer.from_pretrained(model_name)
model = AutoModelForSequenceClassification.from_pretrained(model_name)

tokenizer.save_pretrained(save_path)
model.save_pretrained(save_path)