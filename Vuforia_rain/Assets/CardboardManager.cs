using UnityEngine;
using UnityEngine.UI;
using Google.XR.Cardboard;

public class CardboardManager: MonoBehaviour
{
    public Button launchButton; 
    private Google.XR.Cardboard.XRLoader cardboardLoader;

    void Start()
    {
        cardboardLoader = ScriptableObject.CreateInstance<Google.XR.Cardboard.XRLoader>();
    }

    public void LaunchGoogleCardboard() 
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            cardboardLoader.Initialize();
            cardboardLoader.Start();
            launchButton.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Google.XR.Cardboard.Api.IsCloseButtonPressed)
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                cardboardLoader.Stop();
                cardboardLoader.Deinitialize();
                launchButton.gameObject.SetActive(true);
            }
        }
    }
}