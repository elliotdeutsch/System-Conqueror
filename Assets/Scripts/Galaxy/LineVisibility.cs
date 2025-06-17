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

        bool bothVisible = visibleStars.Contains(starA) && visibleStars.Contains(starB);
        Color visibleColor = new Color(1f, 1f, 1f, 0.1f);
        if (starA.Owner != null && starB.Owner != null && starA.Owner == starB.Owner)
        {
            visibleColor = starA.Owner.Color;
        }
        Color hiddenColor = new Color(0.5f, 0.5f, 0.5f, 0.1f);
        lineRenderer.material.color = bothVisible ? visibleColor : hiddenColor;
    }
}
