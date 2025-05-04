import random
from models.financial_atlas_models import Company
from ai.llm_client import execute_prompt
from constants import SECTORS, REGIONS, LOCALES
from helpers import ticker_creator
from faker import Faker
from datetime import datetime

MOOD_PRESETS = [
    "Playful and creative",
    "Bold and futuristic",
    "Serious and trustworthy",
    "Youthful and edgy",
    "Minimalist and elegant",
    "Eco-conscious and visionary",
    "Quirky and local",
    "Techy and innovative",
    "Premium and luxurious",
    "Disruptive and rebellious"
]


def generate() -> Company:
    """
    Uses AI to generate a structured company profile.
    """
    # Determine region and locale for company to be generated
    region = random.choice(REGIONS)
    locale = random.choice(LOCALES.get(region))

    # Initialize faker with a specific locale
    fake = Faker(locale)

    # Generate some company data
    ceo_name = fake.name()
    headquarters = fake.address()
    founded_year = random.randint(1950, datetime.now().year - 1)
    employees = random.randint(100, 100_000)
    sector = random.choice(SECTORS)

    # Get mood
    mood = random.choice(MOOD_PRESETS)

    prompt = f"""
You are generating a company **name** and **description** based on structured seed data.

Here is the seed data:

- Sector: {sector}
- Region: {region}
- CEO: {ceo_name}
- Headquarters: {headquarters}
- Founded Year: {founded_year}
- Employees: {employees}

🧠 **Mood / Tone**: {mood}

🎯 **Your task**:
Create a unique, memorable **company name** and a 1–2 sentence **description** that fits the mood and the sector.

The name should match the desired tone: **{mood}**, and still sound like a plausible publicly traded company.

The description should reflect the sector ({sector}) and region ({region}), while matching the tone above. Feel free to use creative language, metaphors, or local flair.

📦 **Output JSON only**:
{{
  "name": "Company Name",
  "description": "A short, vibrant, tone-aligned description of what the company does."
}}

⚠️ Return only valid JSON. No markdown, no explanations.
"""

    company_data = execute_prompt(prompt)
    ticker = ticker_creator.from_name(company_data["name"])
    return Company(
        ticker= ticker,
        name=company_data["name"],
        industry=sector,
        description=company_data["description"],
        headquarters=headquarters,
        ceo=ceo_name,
        founded_year=founded_year,
        employees=employees
    )