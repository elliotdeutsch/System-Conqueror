using UnityEngine;

public class FogOfWarTest : MonoBehaviour
{
    private GalaxyManager galaxyManager;

    void Start()
    {
        galaxyManager = FindFirstObjectByType<GalaxyManager>();
        InvokeRepeating("TestFogOfWar", 5f, 10f); // Test toutes les 10 secondes après 5 secondes
    }

    void TestFogOfWar()
    {
        if (galaxyManager != null && galaxyManager.controlledPlayer != null)
        {
            int visibleCount = 0;
            int totalCount = galaxyManager.stars.Count;

            foreach (var star in galaxyManager.stars)
            {
                if (star.GetComponent<SpriteRenderer>().enabled)
                    visibleCount++;
            }

            Debug.Log($"[FOG OF WAR TEST] Étoiles visibles: {visibleCount}/{totalCount} ({(float)visibleCount / totalCount * 100:F1}%)");
            Debug.Log($"[FOG OF WAR TEST] Mode: {(galaxyManager.showFarStars ? "Désactivé" : "Activé")}");
        }
    }
}