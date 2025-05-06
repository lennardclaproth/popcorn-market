import random
import re

def from_name(name: str, min_len=3, max_len=6) -> str:
    # Clean the name and uppercase it
    name = re.sub(r'[^\w\s]', '', name).upper()
    words = name.split()

    # Build options from name chunks
    options = []

    # Combine first letters of words
    if len(words) >= 2:
        initials = ''.join(word[0] for word in words)
        options.append(initials)

        combined = words[0][:3] + words[1][:3]
        options.append(combined)

        options.append(words[0][:2] + words[1][:2])
    
    # Use the first word alone if necessary
    if words:
        options.append(words[0])
        options.append(words[0][:max_len])
    
    # Fallback: random uppercase letters
    options.append(''.join(random.choices("ABCDEFGHIJKLMNOPQRSTUVWXYZ", k=max_len)))

    # Filter and trim to acceptable lengths
    filtered = [opt[:max_len] for opt in options if len(opt) >= min_len]

    # Return a random valid ticker
    return random.choice(filtered)
