using System.Collections.Generic;
using UnityEngine;

public class LineVisibility : MonoBehaviour
{
    public Star starA;
    public Star starB;
    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void UpdateVisibility(HashSet<Star> visibleStars)
    {
        if (lineRenderer == null) return;

        bool aVisible = visibleStars.Contains(starA);
        bool bVisible = visibleStars.Contains(starB);

        if (!aVisible || !bVisible)
        {
            // Au moins une étoile hors vision : gris semi-transparent
            Color gray = new Color(0.6f, 0.6f, 0.6f, 0.1f);
            Gradient g = new Gradient();
            g.SetKeys(
                new GradientColorKey[] { new GradientColorKey(gray, 0f), new GradientColorKey(gray, 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(gray.a, 0f), new GradientAlphaKey(gray.a, 1f) }
            );
            lineRenderer.colorGradient = g;
            lineRenderer.enabled = true;
        }
        else
        {
            // Les deux visibles : gradient naturel entre les deux couleurs
            Color colorA = (starA.Owner != null) ? starA.Owner.Color : new Color(1f, 1f, 1f, 0.1f);
            Color colorB = (starB.Owner != null) ? starB.Owner.Color : new Color(1f, 1f, 1f, 0.1f);
            Gradient g = new Gradient();
            g.SetKeys(
                new GradientColorKey[] { new GradientColorKey(colorA, 0f), new GradientColorKey(colorB, 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(colorA.a, 0f), new GradientAlphaKey(colorB.a, 1f) }
            );
            lineRenderer.colorGradient = g;
            lineRenderer.enabled = true;
        }
    }
}
