using UnityEngine;
using TMPro;

public class GameSetupUI : MonoBehaviour
{
    public GalaxyManager galaxyManager;
    public GameObject setupPanel;
    public TMP_InputField aiInput;
    public TMP_InputField widthInput;
    public TMP_InputField heightInput;
    public TMP_InputField starsInput;

    void Start()
    {
        if (galaxyManager == null)
        {
            galaxyManager = FindObjectOfType<GalaxyManager>();
        }

        if (setupPanel != null)
        {
            setupPanel.SetActive(true);
        }

        if (aiInput != null)
        {
            aiInput.text = galaxyManager.numberOfAI.ToString();
        }
        if (widthInput != null)
        {
            widthInput.text = galaxyManager.mapWidth.ToString();
        }
        if (heightInput != null)
        {
            heightInput.text = galaxyManager.mapHeight.ToString();
        }
        if (starsInput != null)
        {
            starsInput.text = galaxyManager.numberOfStars.ToString();
        }
    }

    public void OnPlayClicked()
    {
        if (galaxyManager == null) return;

        if (aiInput != null && int.TryParse(aiInput.text, out int ai))
        {
            galaxyManager.numberOfAI = Mathf.Max(0, ai);
        }
        if (widthInput != null && float.TryParse(widthInput.text, out float width))
        {
            galaxyManager.mapWidth = Mathf.Max(10f, width);
        }
        if (heightInput != null && float.TryParse(heightInput.text, out float height))
        {
            galaxyManager.mapHeight = Mathf.Max(10f, height);
        }
        if (starsInput != null && int.TryParse(starsInput.text, out int stars))
        {
            galaxyManager.numberOfStars = Mathf.Clamp(stars, 10, 1000);
        }

        if (setupPanel != null)
        {
            setupPanel.SetActive(false);
        }

        galaxyManager.InitializeGalaxy();

        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.StartTimer();
        }
    }
}
