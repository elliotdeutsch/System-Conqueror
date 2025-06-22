using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameSetupUI : MonoBehaviour
{
    public GalaxyManager galaxyManager;
    public GameObject setupPanel;
    public TMP_InputField aiInput;
    public TMP_InputField starsInput;
    public Toggle farStarsToggle;

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
        if (starsInput != null)
        {
            starsInput.text = galaxyManager.numberOfStars.ToString();
        }

        if (farStarsToggle != null)
        {
            farStarsToggle.isOn = galaxyManager.showFarStars;
            var label = farStarsToggle.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (label != null)
                label.text = "Afficher toutes les étoiles (désactive le brouillard)";
        }
    }

    public void OnPlayClicked()
    {
        if (galaxyManager == null) return;

        if (aiInput != null && int.TryParse(aiInput.text, out int ai))
        {
            galaxyManager.numberOfAI = Mathf.Max(0, ai);
        }
        if (starsInput != null && int.TryParse(starsInput.text, out int stars))
        {
            galaxyManager.numberOfStars = Mathf.Clamp(stars, 10, 1000);
        }
        if (farStarsToggle != null)
        {
            galaxyManager.showFarStars = !farStarsToggle.isOn;
        }
        else
        {
            Debug.LogWarning("Far Stars Toggle is not assigned to GameSetupUI. Visibility option will use the default value.");
        }

        if (setupPanel != null)
        {
            setupPanel.SetActive(false);
        }

        galaxyManager.InitializeGalaxy();
        galaxyManager.UpdateFogOfWar();

        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.ResetAndStartTimer();
        }

        // Démarrer le jeu après la configuration
        if (GameTimeController.Instance != null)
        {
            GameTimeController.Instance.StartGame();
        }

        // Centrer la caméra sur le joueur après l'initialisation
        CameraController cameraController = FindObjectOfType<CameraController>();
        if (cameraController != null)
        {
            cameraController.CenterOnPlayer();
        }
    }
}
