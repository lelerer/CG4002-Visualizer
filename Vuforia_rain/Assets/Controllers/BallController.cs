using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour
{
    public float flightDuration = 2f;
    private Vector3 startPos;
    private Vector3 controlPoint;
    private Vector3 endPos;
    private bool isMoving = false;
    public QRCodeScanner qrCodeScanner;

    void Start()
    {
        gameObject.SetActive(false); // Start hidden
    }

    public void ThrowBall()
    {
        if (isMoving || qrCodeScanner == null || !qrCodeScanner.IsQRCodeDetected())
            return;

        gameObject.SetActive(true);

        // 🟢 Start 20cm in front of the camera (phone), so it flies *outward*
        float startOffset = 0.2f;
        startPos = Camera.main.transform.position + Camera.main.transform.forward * startOffset;
        // 🎯 QR code position (world-space target)

        Debug.Log($"QR code at: {QRCodeScanner.qrCodePosition} ");
        endPos = QRCodeScanner.qrCodePosition;

        // 📐 Control point: midway between start and end, plus height for an arc
        Vector3 midPoint = (startPos + endPos) / 2f;

        // ✅ Arc goes upward relative to camera's up vector for a "throw" feeling
        float arcHeight = 0.5f;
        controlPoint = midPoint + Camera.main.transform.up * arcHeight;

        StartCoroutine(MoveBallAlongBezier());
    }

    IEnumerator MoveBallAlongBezier()
    {
        isMoving = true;
        float elapsedTime = 0f;

        while (elapsedTime < flightDuration)
        {
            float t = elapsedTime / flightDuration;

            Vector3 a = Vector3.Lerp(startPos, controlPoint, t);
            Vector3 b = Vector3.Lerp(controlPoint, endPos, t);
            transform.position = Vector3.Lerp(a, b, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        isMoving = false;
        gameObject.SetActive(false);
    }
}
