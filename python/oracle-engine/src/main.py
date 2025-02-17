import json
import datetime
import re
from ollama import chat, ChatResponse

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
    try:
        parsed_json = json.loads(json_str)
    except json.JSONDecodeError:
        print("Error: Could not parse JSON. Returning empty object.")
        return {"error": "Invalid JSON format"}

    return parsed_json

tones = ["Professional, informative, and engaging.", "Playful, engaging, shocking", "Formal, conservative, realistic"]
highlights = ["Revenue, net income, EPS growth, and stock performance.", "Recent trends, new products, and changes in structure"]
roles = ["financial news journalist", "industry specialist", "local journalist", "investigative journalist"]
tasks = ["**the recent financial performance**", "**a recent scandal**", "**impacting market trends**", "**an unforeseen positive event**"]

# Store previously generated news flashes (this should ideally be dynamically loaded)
past_company_articles = [
    {
        "date": "2025-02-10 12:45:00",
        "headline": "Knight-Reynolds Sees Significant Growth in Q4 2024, Surpassing Expectations",
        "article": "In a major boost to investors and analysts alike, Knight-Reynolds (Ticker: lmL804) has reported impressive financial results..."
    },
    {
        "date": "2025-02-11 08:30:00",
        "headline": "BREAKING: Knight-Reynolds Hit with $10B Scandal, Share Price Plummets!",
        "article": "A shocking scandal has rocked biotech giant Knight-Reynolds, sending its share price into a tailspin! Allegations of embezzlement and financial mismanagement have surfaced..."
    }
]

past_sector_articles = [
    {
        "date": "2025-02-10 12:00:00",
        "headline": "Gene Therapy Breakthrough Could Cure Rare Genetic Disorders",
        "article": "Scientists have developed a groundbreaking gene therapy that shows promise in curing rare genetic disorders. The therapy, currently in Phase III trials, utilizes CRISPR-based editing to repair defective DNA sequences, potentially offering a one-time cure for conditions that previously had no effective treatments. Experts are calling this a historic milestone in genomic medicine, with expectations of FDA approval by 2026."
    },
    {
        "date": "2025-02-09 10:15:00",
        "headline": "Healthcare Robotics Market Booms Amid Rising Demand for AI Surgeons",
        "article": "The demand for AI-powered surgical robots has skyrocketed, with hospitals increasingly adopting robotic-assisted procedures for precision surgery. A recent report forecasts the healthcare robotics market to surpass $25 billion by 2028, driven by advancements in machine learning, automation, and minimally invasive techniques. Surgeons praise the technology for reducing errors and improving patient recovery times."
    }
]

# Select the most recent news to include as context
recent_company_articles_context = "\n\n".join([f"📅 **{news['date']}** - **{news['headline']}**\n{news['article']}" for news in past_company_articles[-2:]])
recent_sector_articles_context = "\n\n".join([f"📅 **{news['date']}** - **{news['headline']}**\n{news['article']}" for news in past_sector_articles[-2:]])

recent_articles_context = f"""
### **Recent Company-Specific Articles**
{recent_company_articles_context}

### **Recent Sector-Wide Articles**
{recent_sector_articles_context}
""".strip()


# Company Information
company_info = {
    "Company Name": "Knight-Reynolds",
    "Industry": "Healthcare",
    "Description": "A biotech company developing cutting-edge medical solutions.",
    "Headquarters": "Wallerhaven, Tunisia",
    "CEO": "Julie Elliott",
    "Founded Year": 2018,
    "Employees": 45257,
    "Ticker Symbol": "KNRE-TN",
    "Stock Price (USD)": 87.54,
    "Market Capitalization (B)": 65.68,
    "Dividend per Share (USD)": 0.75,
    "Dividend Yield (%)": 0.67
}

# Financial Summary
financials = f"""
📊 **Financial Overview for {company_info['Company Name']} (Ticker: {company_info['Ticker Symbol']})**

**2024 Highlights:**
- Revenue: **$38.61B** (⬆ from $34.25B in 2023)
- Net Income: **$9.82B** (⬆ from $7.42B in 2023)
- Earnings per Share (EPS): **$13.10** (⬆ from $9.88 in 2023)
- Free Cash Flow: **$6.70B** (⬆ from $4.68B in 2023)

**Balance Sheet:**
- Total Assets: **$101.23B**
- Total Equity: **$65.49B**
- Debt-to-Equity Ratio: **0.55** (⬇ from 0.85 in 2023)

**Stock Performance:**
- Current Stock Price: **$87.54**
- Market Capitalization: **$65.68B**
- Dividend Yield: **0.67%**
"""

prompt = f"""
🔹 **Generate an Article** 🔹

You are a {roles[3]} writing a **concise and engaging article** about {tasks[3]} of **{company_info['Company Name']}**, a leading company in the {company_info['Industry']} sector.

### **Company Overview**
{company_info['Company Name']} is headquartered in {company_info['Headquarters']} and specializes in {company_info['Description']}. It is led by CEO {company_info['CEO']} and has a workforce of {company_info['Employees']} employees.

{recent_articles_context}

### **Latest Financial Performance**
{financials}

### **Guidelines for the Article:**
- **Tone:** {tones[2]}
- **Format:** Start with a headline, followed by the article.
- **Length:** Keep the article **around 250 words** for quick readability.
- **Highlight:** {highlights[1]}

### **Task:**
Generate a **concise and engaging article** considering past events and the latest financials.

🔹 **Output Format:** Return only a valid JSON object with the following structure:
```json
{{
    "headline": "A short, engaging headline",
    "article": "A concise article",
}}

Do not include any additional text or explanations.
"""

# response: ChatResponse = chat(model='llama3.2', messages=[
#     {'role': 'user', 'content': prompt},
# ])

response: ChatResponse = chat(model='deepseek-r1:1.5b', messages=[
    {'role': 'user', 'content': prompt},
])

# Extract the message content
response_text = response.message.content

# Step 1: Extract the <think> section
think_match = re.search(r"<think>(.*?)</think>", response_text, re.DOTALL)
think_text = think_match.group(1).strip() if think_match else "No reasoning provided."

# Step 1: Remove <think> section
cleaned_response = re.sub(r"<think>.*?</think>", "", response_text, flags=re.DOTALL).strip()

# Step 3: Parse JSON into a Python dictionary
try:
    article_data = extract_json(cleaned_response)
except json.JSONDecodeError:
    print("Error: Could not parse JSON from response.")
    article_data = {}

# Step 4: Add the "think" text to the final JSON output
article_data["reasoning"] = think_text


# Llama response parsing
# response = {}
# try:
#     response = json.loads(response_text)
#     response["context"] = ""
# except json.JSONDecodeError:
#     print("Error: Model did not return valid JSON.")

# Create JSON output
article = {
    "ticker": company_info["Ticker Symbol"],
    "industry": company_info["Industry"],
    "company_name": company_info["Company Name"],
    "date": datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S"),
    "article": article_data["article"],
    "headline": article_data["headline"],
    "metadata": {
        "reasoning": article_data["reasoning"]
    },
}

# Save JSON to a file (optional)
with open("article_data.json", "w") as f:
    json.dump(article_data, f, indent=4)

# Print the structured JSON output
print(json.dumps(article_data, indent=4))

