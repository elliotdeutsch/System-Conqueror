using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingText : MonoBehaviour
{
    public TextMeshPro textMesh;
    public float moveSpeed = 2f;
    public float fadeSpeed = 1f;
    public float lifeTime = 2f;
    public Vector3 moveDirection = Vector3.up;

    [Header("Visual Effects")]
    public float pulseSpeed = 3f; // Vitesse de pulsation
    public float pulseIntensity = 0.3f; // Intensité de la pulsation

    private Color originalColor;
    private float currentLifeTime = 0f;
    private float pulseTime = 0f;

    void Start()
    {
        if (textMesh == null)
        {
            textMesh = GetComponent<TextMeshPro>();
        }

        if (textMesh != null)
        {
            originalColor = textMesh.color;
        }
    }

    void Update()
    {
        // Déplacer le texte vers le haut
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // Effet de pulsation
        pulseTime += Time.deltaTime * pulseSpeed;
        float pulse = 1f + Mathf.Sin(pulseTime) * pulseIntensity;

        // Gérer le fade out
        currentLifeTime += Time.deltaTime;
        float normalizedTime = currentLifeTime / lifeTime;

        if (textMesh != null)
        {
            Color currentColor = originalColor;

            // Appliquer la pulsation
            currentColor.r *= pulse;
            currentColor.g *= pulse;
            currentColor.b *= pulse;

            // Appliquer le fade out
            currentColor.a = Mathf.Lerp(originalColor.a, 0f, normalizedTime);

            textMesh.color = currentColor;
        }

        // Détruire après la durée de vie
        if (currentLifeTime >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    public void SetText(string text, Color color)
    {
        if (textMesh != null)
        {
            textMesh.text = text;
            textMesh.color = color;
            originalColor = color;
        }
    }
}