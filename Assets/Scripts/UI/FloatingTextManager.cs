using UnityEngine;
using TMPro;

public class FloatingTextManager : MonoBehaviour
{
    public static FloatingTextManager Instance { get; private set; }

    [Header("Floating Text Prefab")]
    public GameObject floatingTextPrefab;

    [Header("Animation Settings")]
    public float offsetY = 1f; // Distance au-dessus de la planète
    public float randomOffsetX = 0.5f; // Variation horizontale aléatoire

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Créer le prefab par défaut si aucun n'est assigné
        if (floatingTextPrefab == null)
        {
            CreateDefaultPrefab();
        }
    }

    void CreateDefaultPrefab()
    {
        // Créer un GameObject pour le texte flottant
        GameObject textObj = new GameObject("FloatingText");
        textObj.transform.SetParent(transform);

        // Ajouter TextMeshPro
        TextMeshPro textMesh = textObj.AddComponent<TextMeshPro>();
        textMesh.fontSize = 12; // Taille augmentée pour plus de visibilité
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.fontStyle = FontStyles.Bold;

        // Effet de glow/outline pour améliorer la visibilité
        textMesh.outlineWidth = 0.4f;
        textMesh.outlineColor = Color.white;

        // Configurer le matériau pour un meilleur rendu
        textMesh.fontMaterial.EnableKeyword("_EMISSION");
        textMesh.fontMaterial.SetColor("_EmissionColor", Color.white * 0.5f);

        // Ajouter le script FloatingText
        FloatingText floatingText = textObj.AddComponent<FloatingText>();
        floatingText.textMesh = textMesh;

        floatingTextPrefab = textObj;
        textObj.SetActive(false); // Désactiver le prefab de base
    }

    public void ShowFloatingText(Vector3 position, string text, Color color)
    {
        if (floatingTextPrefab == null) return;

        // Position avec offset aléatoire
        Vector3 spawnPosition = position + Vector3.up * offsetY;
        spawnPosition.x += Random.Range(-randomOffsetX, randomOffsetX);

        // Créer l'instance
        GameObject instance = Instantiate(floatingTextPrefab, spawnPosition, Quaternion.identity);
        instance.SetActive(true);

        // Configurer le texte
        FloatingText floatingText = instance.GetComponent<FloatingText>();
        if (floatingText != null)
        {
            floatingText.SetText(text, color);
        }
    }

    public void ShowUnitGeneration(Star star, int unitsGenerated)
    {
        if (star == null) return;

        string text = $"+{unitsGenerated}u";
        Color baseColor = star.Owner != null ? star.Owner.Color : Color.white;

        // Rendre la couleur plus claire et plus visible
        Color brightColor = new Color(
            Mathf.Min(baseColor.r * 1.5f, 1f), // Rouge plus clair
            Mathf.Min(baseColor.g * 1.5f, 1f), // Vert plus clair
            Mathf.Min(baseColor.b * 1.5f, 1f), // Bleu plus clair
            1f // Alpha à 100%
        );

        ShowFloatingText(star.transform.position, text, brightColor);
    }
}