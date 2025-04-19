using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GunController : MonoBehaviour
{
    public Button shootButton;
    public Button reloadButton;
    public Transform gunTransform;
    public GameObject hitEffectPrefab;  // ✅ Effect for a successful hit
    public GameObject missEffectPrefab; // ❌ Effect for a miss
    public QRCodeScanner qrCodeScanner; // Assign the QR Code scanner
    
    public Vector3 shootOffset = new Vector3(0, 0, -0.4f);
    
    private bool isShooting = false;
    private bool isReloading = false;
    
    private Quaternion initialRotation;

    void Start()
    {
        if (gunTransform == null)
        {
            gunTransform = transform;
        }

        initialRotation = gunTransform.localRotation;

        shootButton.onClick.AddListener(Shoot);
        reloadButton.onClick.AddListener(Reload);  // 🔥 Now properly linked
    }

    public void Shoot()
    {
        if (!isShooting)
        {
            isShooting = true;
            StartCoroutine(ShootEffect());

            if (QRCodeDetected())  // ✅ Check if a QR code is detected
            {
                Debug.Log("🎯 Hit: QR Code detected!");
                StartCoroutine(HitFeedback()); // Apply hit effect

                // 💥 Spawn hit effect
                if (hitEffectPrefab)
                {
                    Instantiate(hitEffectPrefab, QRCodeScanner.qrCodePosition, Quaternion.identity);
                }
            }
            else
            {
                Debug.Log("❌ Miss: No QR Code detected!");
                StartCoroutine(MissFeedback()); // Apply miss effect

                // ❌ Spawn miss effect
                if (missEffectPrefab)
                {
                    Instantiate(missEffectPrefab, gunTransform.position + gunTransform.forward * 2, Quaternion.identity);
                }
            }
        }
    }

    public void Reload()
    {
        if (!isReloading)
        {
            isReloading = true;
            StartCoroutine(ReloadEffect());
        }
    }

    bool QRCodeDetected()
    {
        return qrCodeScanner.IsQRCodeDetected();
    }

    IEnumerator ShootEffect()
    {
        Vector3 originalPosition = gunTransform.localPosition;
        float elapsedTime = 0f;
        float shootDuration = 0.2f;

        while (elapsedTime < shootDuration)
        {
            gunTransform.localPosition = Vector3.Lerp(originalPosition, originalPosition + shootOffset, elapsedTime / shootDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gunTransform.localPosition = originalPosition;
        isShooting = false;
    }

    IEnumerator ReloadEffect()
    {
        float elapsedTime = 0f;
        float reloadDuration = 0.3f;

        Quaternion originalRotation = gunTransform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(90f, 0f, 0f); // Tilt forward like a 不倒翁

        // Tilt forward
        while (elapsedTime < reloadDuration / 2)
        {
            gunTransform.localRotation = Quaternion.Slerp(originalRotation, targetRotation, elapsedTime / (reloadDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gunTransform.localRotation = targetRotation;
        elapsedTime = 0f;

        // Return to original position
        while (elapsedTime < reloadDuration / 2)
        {
            gunTransform.localRotation = Quaternion.Slerp(targetRotation, originalRotation, elapsedTime / (reloadDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gunTransform.localRotation = originalRotation;
        isReloading = false;
    }

IEnumerator HitFeedback()  
{
    Transform hitPart = gunTransform.Find("Hit");
    if (hitPart != null)
    {
        hitPart.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        hitPart.gameObject.SetActive(false);
    }
}

IEnumerator MissFeedback()
{
    Transform missPart = gunTransform.Find("Miss");
    if (missPart != null)
    {
        missPart.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        missPart.gameObject.SetActive(false);
    }
}

}
