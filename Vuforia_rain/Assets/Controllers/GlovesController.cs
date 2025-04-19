using UnityEngine;
using System.Collections;

public class GlovesController : MonoBehaviour
{
    public float moveSpeed = 10f; // Speed of movement
    public Transform cameraTransform; // Assign the Main Camera in the Inspector
    public QRCodeScanner qrCodeScanner; // Assign the QR Code scanner

    private Vector3 targetPosition;

    void Start()
    {
        gameObject.SetActive(false); // Ensure the sword is not active initially
    }

    public void Punch()
    {
        if (qrCodeScanner.IsQRCodeDetected()) // Ensure a QR Code is detected
        {
            targetPosition = QRCodeScanner.qrCodePosition;
            
            // Set the sword’s position in front of the camera
            transform.position = cameraTransform.TransformPoint(new Vector3(0, 0, 0.5f)); 
            
            // Lock the sword's rotation to (50, 0, 0)
            transform.rotation = Quaternion.Euler(180, -90, 90); 
            
            gameObject.SetActive(true); // Activate the sword
            StartCoroutine(MoveGloves());
        }
        else
        {
            Debug.LogWarning("No QR Code detected! Sword cannot move.");
        }
    }

    private IEnumerator MoveGloves()
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f); // Optional delay before disappearing

        gameObject.SetActive(false); // Deactivate sword after reaching target
    }
}
