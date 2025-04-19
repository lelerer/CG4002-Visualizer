import paho.mqtt.client as mqtt
import threading
import time
import json
from helper import generate_random_payload

def on_connect(client, userdata, flags, reason_code, properties):
    print(f"[FPGA] connected with reason code: {reason_code}")

def on_publish(client, userdata, mid, reason_code, properties):
    print(f"[FPGA] published with reason code: {reason_code}")

def generate_ai_action():
    actions = ["golf", "shield", "badminton", "fencing", "reload", "bomb"]
    actions = ["bomb"]
    return {"PlayerID": "2", "action": actions[int(time.time()) % len(actions)]}

client = mqtt.Client(mqtt.CallbackAPIVersion.VERSION2)
client.username_pw_set("brandon-os", "capB17")
client.on_connect = on_connect
client.on_publish = on_publish
client.connect("localhost", 1883, 60)

client.loop_start()

while True:
    # Generate game state
    game_state_payload = {
        "p1": {'hp': 78, 'bullets': 4, 'bombs': 2, 'shield_hp': 0, 'deaths': 0, 'shields': 3},
        "p2": {'hp': 78, 'bullets': 3, 'bombs': 1, 'shield_hp': 30, 'deaths': 0, 'shields': 2}
    }
    
    # Generate AI action
    ai_action_payload = generate_ai_action()
    
    # # Publish game state
    # client.publish("game_state", json.dumps(game_state_payload), qos=1)
    # print(f"Published game_state: {game_state_payload}")
    
    # Publish AI action separately
    client.publish("ai_actions", json.dumps(ai_action_payload), qos=1)
    print(f"Published ai_actions: {ai_action_payload}")
    
    time.sleep(5)
