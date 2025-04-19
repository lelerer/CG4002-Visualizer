using UnityEngine;
using Vuforia;
using TMPro;

public class QRCodeScanner : MonoBehaviour
{
    public static Vector3 qrCodePosition;
    public TargetStatus currentTargetStatus;
    public TextMeshProUGUI detectedText;
    public mqttManager mqttMgr; // Reference to MQTT manager
    public SnowEffectController snowEffectController; // Assign the snow particle system prefab in Unity Inspector

    private void OnEnable()
    {
        // Listen for when target status changes (such as when QR code is detected)
        var observer = GetComponent<ObserverBehaviour>();
        if (observer)
        {
            observer.OnTargetStatusChanged += OnTargetStatusChanged;
        }
    }

    private void OnDisable()
    {
        var observer = GetComponent<ObserverBehaviour>();
        if (observer)
        {
            observer.OnTargetStatusChanged -= OnTargetStatusChanged;
        }
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus targetStatus)
    {
        // Update the public target status variable
        currentTargetStatus = targetStatus;

        // Print status to the console every time the status changes
        Debug.Log($"[QR Code Status Changed] Status: {targetStatus.Status}, Info: {targetStatus.StatusInfo}");

        mqttMgr.PublishRandomBoolean();

        snowEffectController.checkRain(snowEffectController.QuantizePosition(qrCodePosition));

        if (IsQRCodeDetected())
        {
            // QR code detected
            detectedText.text = "QR code detected";
            detectedText.color = Color.green;

            qrCodePosition = behaviour.transform.position;
            Debug.Log("[QR code scanner] QR code detected at position: " + qrCodePosition);
        }

        else
        {
            detectedText.text = "QR code not detected";
            detectedText.color = Color.red;
        }
    }

    public bool IsQRCodeDetected()
    {
        // return currentTargetStatus.Status == Status.TRACKED || currentTargetStatus.Status == Status.EXTENDED_TRACKED;
        return currentTargetStatus.Status == Status.TRACKED;

    }
}

