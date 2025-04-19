using UnityEngine;
// using UnityEngine.XR.ARSubsystems;

public class OpponentShieldController : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false); // Start with the shield disabled
    }

    void Update()
    {
        // Check if a QR code is detected
        if (QRCodeScanner.qrCodePosition != Vector3.zero)
        {
            gameObject.SetActive(true); // Enable shield
            transform.position = QRCodeScanner.qrCodePosition;
        }
        else
        {
            gameObject.SetActive(false); // Disable shield when QR code is lost
        }
    }
}
