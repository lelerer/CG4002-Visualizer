using System;
using UnityEngine;
using TMPro;
using PimDeWitte.UnityMainThreadDispatcher;

[System.Serializable]
public class GameData
{
	public GameState game_state;
	public MQTTMessage ai_actions;
}

[Serializable]
public class MQTTMessage
{
	public string PlayerID;
	public string action;
}

[Serializable]
public class PlayerState
{
	public int hp;
	public int bullets;
	public int bombs;
	public int shield_hp;
	public int deaths;
	public int shields;
}

[Serializable]
public class GameState
{
	public PlayerState p1;
	public PlayerState p2;
}


public class GameUIController : MonoBehaviour
{
	public TextMeshProUGUI groupNumberText;
	public TextMeshProUGUI playerHPText;
	public TextMeshProUGUI playerAmmoText;
	public TextMeshProUGUI playerShieldText;
	public TextMeshProUGUI playerBombText;
	public TextMeshProUGUI opponentHPText;
	public TextMeshProUGUI opponentAmmoText;
	public TextMeshProUGUI opponentShieldText;
	public QRCodeScanner qrCodeScanner;
	public TextMeshProUGUI detectedText;
	public TextMeshProUGUI opponentBombText;
	public BallController ballController; // Reference to BallController
	public GlovesController glovesController;
	public GunController gunController;
	public BadmintonController badmintonController; // Reference to BallController
	public SwordController swordController;
	public GameObject shieldEffectPanel; // Panel to simulate the shield effect
	public GameObject logoutEffect;
	public GameObject OpponentShield;
	public SnowEffectController snowEffectController; // Assign the snow particle system prefab in Unity Inspector
	public mqttManager mqttManager;  // Reference to your mqttManager class


	private int playerHP = 100;
	private int playerAmmo = 6;
	private int playerShield = 3;
	private int opponentHP = 100;
	private int opponentAmmo = 6;
	private int opponentShield = 3;
	private int playerBomb = 2;
	private int opponentBomb = 2;
	private int player_shield_hp = 100;
	private int opponent_shield_hp = 100;
	private bool isPlayerShielded = false; // To track if the player is shielded
	private bool isOpponentShielded = false; // To track if the player is shielded	
	private bool isLogoutShown = false;

	void Start()
	{
		UpdateUI();
		shieldEffectPanel.SetActive(false); // Make sure shield effect is off initially
		if (mqttManager != null)
		{
			mqttManager.OnMessageArrived += HandleMQTTMessage;
			Debug.Log("[MQTT] GameUIController subscribed to OnMessageArrived");
		}
		else
		{
			Debug.LogError("[MQTT] mqttManager is NULL in GameUIController! Make sure to assign it in the Inspector.");
		}
	}

	private void OnDestroy()
	{
		if (mqttManager != null)
		{
			mqttManager.OnMessageArrived -= HandleMQTTMessage;
		}
	}

	public void HandleMQTTMessage(mqttObj mqttObject)
	{
		UnityMainThreadDispatcher.Instance().Enqueue(() =>
		{
			thisHandleMQTTMessage();
		});
	}

	public void thisHandleMQTTMessage()
	{
		if (mqttManager == null)
		{
			Debug.LogWarning("mqttManager is not assigned!");
			return;
		}

		string topic = mqttManager.LastReceivedTopic;  // Using the topic from the mqttManager
		string messagePayload = mqttManager.LastReceivedMessage;  // Using the message payload from the mqttManager

		if (string.IsNullOrEmpty(topic) || string.IsNullOrEmpty(messagePayload))
		{
			Debug.LogWarning("MQTT message received with empty topic or payload.");
			return;
		}

		Debug.Log($"[GameUI] MQTT message received from topic: {topic}, payload: {messagePayload}");

		if (topic == "game_state")
		{
			Debug.Log("[GameUI] Updating game state now");
			GameState gameState = JsonUtility.FromJson<GameState>(messagePayload);
			if (gameState != null)
			{
				playerHP = gameState.p1.hp;
				playerAmmo = gameState.p1.bullets;
				playerShield = gameState.p1.shields;
				playerBomb = gameState.p1.bombs;
				player_shield_hp = gameState.p1.shield_hp;
				opponentHP = gameState.p2.hp;
				opponentAmmo = gameState.p2.bullets;
				opponentShield = gameState.p2.shields;
				opponentBomb = gameState.p2.bombs;
				opponent_shield_hp = gameState.p2.shield_hp;
				UpdateUI();
			}
			else
			{
				Debug.LogWarning("Failed to parse game_state message.");
			}
		}

		if (topic == "ai_actions")
		{
			MQTTMessage actionMessage = JsonUtility.FromJson<MQTTMessage>(messagePayload);
			if (actionMessage != null)
			{
				Debug.Log($"[GameUI] Action received from Player {actionMessage.PlayerID}: {actionMessage.action}");

				// if (actionMessage.PlayerID == "1")
				// {
				// 	switch (actionMessage.action)
				// 	{
				// 		case "shield":
				// 			DeactivateLogoutEffect();
				// 			ActivatePlayerShield();
				// 			break;
				// 		case "reload":
				// 			DeactivateLogoutEffect();
				// 			Reload();
				// 			break;
				// 		case "gun":
				// 			DeactivateLogoutEffect();
				// 			if (playerAmmo > 0)
				// 			{
				// 				Shoot();
				// 			}
				// 			// Shoot();
				// 			break;
				// 		case "badminton":
				// 			DeactivateLogoutEffect();
				// 			ThrowBadminton();
				// 			break;
				// 		case "golf":
				// 			DeactivateLogoutEffect();
				// 			ThrowBall();
				// 			break;
				// 		case "fencing":
				// 			DeactivateLogoutEffect();
				// 			Fence();
				// 			break;
				// 		case "boxing":
				// 			DeactivateLogoutEffect();
				// 			Punch();
				// 			break;
				// 		case "bomb":
				// 			DeactivateLogoutEffect();
				// 			if (playerBomb > 0)
				// 			{
				// 				ActivateSnowEffect();
				// 			}
				// 			// ActivateSnowEffect();
				// 			break;
				// 		case "logout":
				// 			ActivateLogoutEffect();
				// 			break;
				// 		default:
				// 			Debug.LogWarning($"Unknown action received: {actionMessage.action}");
				// 			break;
				// 	}
				// }

				if (actionMessage.PlayerID == "2")
				{
					switch (actionMessage.action)
					{
						case "shield":
							DeactivateLogoutEffect();
							ActivatePlayerShield();
							break;
						case "reload":
							DeactivateLogoutEffect();
							Reload();
							break;
						case "gun":
							DeactivateLogoutEffect();
							if (opponentAmmo > 0)
							{
								Shoot();
							}
							break;
						case "badminton":
							DeactivateLogoutEffect();
							ThrowBadminton();
							break;
						case "golf":
							DeactivateLogoutEffect();
							ThrowBall();
							break;
						case "fencing":
							DeactivateLogoutEffect();
							Fence();
							break;
						case "boxing":
							DeactivateLogoutEffect();
							Punch();
							break;
						case "bomb":
							DeactivateLogoutEffect();
							if (opponentBomb > 0)
							{
								ActivateSnowEffect();
							}

							break;
						case "logout":
							ActivateLogoutEffect();
							break;
						default:
							Debug.LogWarning($"Unknown action received: {actionMessage.action}");
							break;
					}
				}

			}
			else
			{
				Debug.LogWarning("Failed to parse ai_actions message.");
			}
		}
	}


	public void ReducePlayerHP()
	{
		if (playerHP > 0)
		{
			playerHP -= 10;
			UpdateUI();
		}
	}

	public void ReducePlayerAmmo()
	{
		if (playerAmmo > 0)
		{
			playerAmmo -= 1;
			UpdateUI();
		}
	}



	public void ReduceOpponentHP()
	{
		if (opponentHP > 0)
		{
			opponentHP -= 10;
			UpdateUI();
		}
	}

	public void ReduceOpponentAmmo()
	{
		if (opponentAmmo > 0)
		{
			opponentAmmo -= 1;
			UpdateUI();
		}
	}

	public void ThrowBall()
	{
		if (ballController != null)
		{
			ballController.ThrowBall();
		}
		else
		{
			Debug.LogWarning("BallController is not assigned!");
		}
	}

	public void Shoot()
	{
		if (gunController != null)
		{
			gunController.Shoot();
		}
		else
		{
			Debug.LogWarning("GunController is not assigned!");
		}
	}

	public void Reload()
	{
		if (gunController != null)
		{
			gunController.Reload();
		}
		else
		{
			Debug.LogWarning("GunController is not assigned!");
		}
	}
	
	public void Punch()
	{
		if (glovesController != null)
		{
			glovesController.Punch();
		}
		else
		{
			Debug.LogWarning("GlovesController is not assigned!");
		}
	}

	public void Fence()
	{
		if (swordController != null)
		{
			swordController.Fence();
		}
		else
		{
			Debug.LogWarning("SwordController is not assigned!");
		}
	}

	public void ThrowBadminton()
	{
		if (badmintonController != null)
		{
			badmintonController.ThrowBadminton();
		}
		else
		{
			Debug.LogWarning("BadmintonController is not assigned!");
		}
	}

	public void ActivateLogoutEffect()
	{
		if (!isLogoutShown)
		{
			isLogoutShown = true;
			logoutEffect.SetActive(true);
		}
	}
	public void DeactivateLogoutEffect()
	{
		if (isLogoutShown)
		{
			isLogoutShown = false;
			logoutEffect.SetActive(false);
		}
	}

	public void ActivatePlayerShield()
	{
		if (!isPlayerShielded)
		{
			isPlayerShielded = true;
			shieldEffectPanel.SetActive(true); // Turn on shield effect (darkens the screen)
		}
	}

	public void DeactivatePlayerShield()
	{
		isPlayerShielded = false;
		shieldEffectPanel.SetActive(false); // Turn off shield effect
	}

	public void ActivateOpponentShield()
	{
		if (!isOpponentShielded)
		{
			isOpponentShielded = true;
			OpponentShield.SetActive(true);
		}
	}

	public void DeactivateOpponentShield()
	{
		isOpponentShielded = false;
		OpponentShield.SetActive(false); // Turn off shield effect
	}

	public void ActivateSnowEffect()
	{
		// Call the StartSnowEffect method to trigger the snow effect
		if (snowEffectController != null)
		{
			snowEffectController.StartSnowEffect();
		}
		else
		{
			Debug.LogWarning("SnowEffectController is not assigned!");
		}
	}

	private void UpdateUI()
	{
		Debug.Log("[GameUI] Updating the UI now");
		playerHPText.text = "HP:" + playerHP;
		playerAmmoText.text = "Ammo:" + playerAmmo;
		playerShieldText.text = "Shield:" + playerShield;
		playerBombText.text = "Bomb:" + playerBomb;

		opponentHPText.text = "HP:" + opponentHP;
		opponentAmmoText.text = "Ammo:" + opponentAmmo;
		opponentShieldText.text = "Shield:" + opponentShield;
		opponentBombText.text = "Bomb:" + opponentBomb;


		if (player_shield_hp <= 0)
		{
			DeactivatePlayerShield();
		}

		if (opponent_shield_hp <= 0)
		{
			DeactivateOpponentShield();
		}

		if (qrCodeScanner.IsQRCodeDetected())
		{
			detectedText.text = "QR code detected";
		}
		else
		{
			detectedText.text = "QR code not detected";
		}
	}

}