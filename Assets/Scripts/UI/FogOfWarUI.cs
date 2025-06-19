using UnityEngine;
using TMPro;

public class FogOfWarUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI instructionsText;

    private GalaxyManager galaxyManager;
    private PlayerController playerController;
    private bool lastFogOfWarState = false; // Pour éviter les appels constants

    void Start()
    {
        galaxyManager = FindObjectOfType<GalaxyManager>();
        playerController = FindObjectOfType<PlayerController>();

        if (instructionsText != null)
        {
            instructionsText.text = "Fog of War:\n• Activé par défaut\n• Appuyez sur F pour basculer\n• Seules les planètes conquises et leurs voisines sont visibles";
        }

        UpdateStatus();
    }

    void Update()
    {
        // Ne mettre à jour que si l'état a changé
        if (galaxyManager != null && lastFogOfWarState != galaxyManager.showFarStars)
        {
            lastFogOfWarState = galaxyManager.showFarStars;
            UpdateStatus();
        }
    }

    void UpdateStatus()
    {
        if (statusText != null && galaxyManager != null)
        {
            statusText.text = $"Fog of War: {(galaxyManager.showFarStars ? "DÉSACTIVÉ" : "ACTIVÉ")}";
            statusText.color = galaxyManager.showFarStars ? Color.red : Color.green;
        }
    }
}