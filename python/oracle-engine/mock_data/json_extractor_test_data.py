import json
import re

# Example model responses simulating different cases
model_responses = [
    '''```json
{
    "headline": "AI Breakthrough in Healthcare",
    "article": "AI-driven diagnostics are changing healthcare rapidly...",
    "date": "2025-02-11"
}
```''',
    '''{
    "headline": "Biotech Stocks Surge After FDA Approval",
    "article": "Multiple biotech firms saw their stock prices rise after regulatory approval...",
    "date": "2025-02-11"
}''',
    '''"Here is your news update:" {
    "headline": "Economic Boom in Biotech",
    "article": "Biotech firms are leading market recovery...",
    "date": "2025-02-11"
}''',
    '''{
    "headline": "Healthcare Innovations",
    "article": "New advancements are reshaping the industry...",
    "date": "2025-02-11"
} More details will follow later.''',
    '''```json
{
    "headline": "New Vaccination Strategies",
    "article": "A new approach to vaccination is proving highly effective...",
    "date": "2025-02-11",
} // Note: Extra comma issue
```''',
    '''```json
{
    "headline": "First JSON"
}
``` ```json
{
    "headline": "Second JSON"
}
```''',
    '''````
{
    "headline": "Incorrect Markdown Format",
    "article": "Some invalid formatting"
}
````''',
    '''"Here’s the update: { "headline": "Breaking", "article": "Details" }"''',
    '''"Sorry, I couldn't generate this article."'''
]