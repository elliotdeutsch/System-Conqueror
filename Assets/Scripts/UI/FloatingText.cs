using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingText : MonoBehaviour
{
    public TextMeshPro textMesh;
    public float moveSpeed = 2f;
    public float lifeTime = 2f;
    public Vector3 moveDirection = Vector3.up;

    private Coroutine activeCoroutine;

    void Awake()
    {
        if (textMesh == null)
        {
            textMesh = GetComponent<TextMeshPro>();
        }
    }

    public void SetText(string text, Color color)
    {
        if (textMesh != null)
        {
            textMesh.text = text;
            textMesh.color = color;
        }

        // Stop any previous animation coroutine before starting a new one
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }
        activeCoroutine = StartCoroutine(AnimateAndReturnToPool(color));
    }

    private IEnumerator AnimateAndReturnToPool(Color startColor)
    {
        float elapsedTime = 0f;

        while (elapsedTime < lifeTime)
        {
            // Move
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            // Fade
            float newAlpha = Mathf.Lerp(startColor.a, 0f, elapsedTime / lifeTime);
            textMesh.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Return to pool after animation
        ObjectPooler.Instance.ReturnToPool("FloatingText", gameObject);
    }
}