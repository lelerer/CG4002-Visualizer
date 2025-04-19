using UnityEngine;
using System.Collections.Generic;

public class SnowEffectController : MonoBehaviour
{
    public GameObject anchoredSnowPrefab;
    public QRCodeScanner qrCodeScanner;
    public mqttManager mqttMgr;
    private Vector3 detectedPos;
    private List<GameObject> anchoredRains = new List<GameObject>();
    // private Dictionary<Vector3Int, int> rainCountLookup = new Dictionary<Vector3Int, int>();
    private int lastPublishedCount = -1;

    void Start()
    {
        gameObject.SetActive(false);
        anchoredSnowPrefab.SetActive(false);
    }

    void Update()
    {
        if (qrCodeScanner.IsQRCodeDetected())
        {
            Vector3 qrPos = QRCodeScanner.qrCodePosition;
            checkRain(qrPos);
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
        }
    }

    public void StartSnowEffect()
    {
        if (qrCodeScanner.IsQRCodeDetected())
        {
            gameObject.SetActive(true);
            SnowEffectFinished();
        }
    }

    private void SnowEffectFinished()
    {
        Vector3 rainPos = QRCodeScanner.qrCodePosition;
        GameObject newRain = Instantiate(anchoredSnowPrefab, rainPos, Quaternion.identity);
        newRain.SetActive(true);
        anchoredRains.Add(newRain);
        // Vector3Int regionKey = QuantizePosition(rainPos);
        // if (rainCountLookup.ContainsKey(regionKey))
        //     rainCountLookup[regionKey]++;
        // else
        //     rainCountLookup[regionKey] = 1;
    }

    public void checkRain(Vector3 qrPosition)
    {
        int count = 0;
        foreach (GameObject rain in anchoredRains)
        {
            if (Vector3.Distance(qrPosition, rain.transform.position) <= 1f)
            {
                count++;
            }
        }
        
        if (count != lastPublishedCount)
        {
            mqttMgr.PublishRain(count);
            Debug.Log($"Published rain count near QR code at {qrPosition}: {count}");
            lastPublishedCount = count;
        }
    }

    // Positions within the same 2x2x2 meter cube will share the same Vector3Int key
    public Vector3Int QuantizePosition(Vector3 pos)
    {
        float cellSize = 2f;
        return new Vector3Int(
            Mathf.FloorToInt(pos.x),
            Mathf.FloorToInt(pos.y),
            Mathf.FloorToInt(pos.z)
        );
    }
}

