import random
import json

def generate_random_payload():
    data = {
        "player_id": random.randint(1, 2),  # Random player ID (1 or 2)
        "action": random.choice(["attack", "defend", "reload", "dodge"]),  # Random action
        "game_state": {
            "p1": {
                "hp": random.randint(0, 100),
                "bullets": random.randint(0, 10),
                "bombs": random.randint(0, 3),
                "shield_hp": random.randint(0, 50),
                "deaths": random.randint(0, 5),
                "shields": random.randint(0, 2)
            },
            "p2": {
                "hp": random.randint(0, 100),
                "bullets": random.randint(0, 10),
                "bombs": random.randint(0, 3),
                "shield_hp": random.randint(0, 50),
                "deaths": random.randint(0, 5),
                "shields": random.randint(0, 2)
            }
        }
    }
    
    return json.dumps(data)