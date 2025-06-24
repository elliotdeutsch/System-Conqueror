using System.Collections.Generic;
using UnityEngine;

/* 
Ce script est responsable de la création et de la mise à jour des lignes entre les étoiles.
*/

public class LineManager : MonoBehaviour
{
    private Dictionary<Star, List<Star>> starGraph;
    public void Initialize(Dictionary<Star, List<Star>> graph)
    {
        starGraph = graph;
    }

    public void CreateLine(Star starA, Star starB)
    {
        GameObject lineObj = new GameObject("Line");
        lineObj.transform.SetParent(transform);
        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.SetPosition(0, starA.transform.position);
        line.SetPosition(1, starB.transform.position);

        // Définir l'épaisseur de la ligne
        line.startWidth = 0.2f;
        line.endWidth = 0.2f;

        // Ne pas définir de couleur ici, laisser LineVisibility gérer
        line.material = new Material(Shader.Find("Sprites/Default"));

        // store star references for visibility updates
        LineVisibility visibility = lineObj.AddComponent<LineVisibility>();
        visibility.starA = starA;
        visibility.starB = starB;

        // Mettre à jour immédiatement la visibilité selon l'état actuel du fog of war
        GalaxyManager galaxyManager = FindFirstObjectByType<GalaxyManager>();
        if (galaxyManager != null)
        {
            // Forcer la mise à jour de toutes les lignes
            galaxyManager.UpdateFogOfWar();
        }
    }

    public void UpdateLines(Star starA, Star starB)
    {
        // Trouver et détruire l'ancienne ligne
        foreach (Transform child in transform)
        {
            LineRenderer lineRenderer = child.GetComponent<LineRenderer>();
            if (lineRenderer != null)
            {
                Vector3 startPosition = lineRenderer.GetPosition(0);
                Vector3 endPosition = lineRenderer.GetPosition(1);

                if ((startPosition == starA.transform.position && endPosition == starB.transform.position) ||
                    (startPosition == starB.transform.position && endPosition == starA.transform.position))
                {
                    Destroy(child.gameObject);
                }
            }
        }

        // Créer une nouvelle ligne
        CreateLine(starA, starB);
    }

    public void UpdateAllLines(Star star)
    {
        StarGraphManager starGraphManager = FindFirstObjectByType<StarGraphManager>();
        List<Star> neighbors = starGraphManager.GetNeighbors(star);
        foreach (Star neighbor in neighbors)
        {
            UpdateLines(star, neighbor);
        }
    }

    public void UpdateLinesVisibility(HashSet<Star> visibleStars)
    {
        foreach (LineVisibility lv in GetComponentsInChildren<LineVisibility>())
        {
            lv.UpdateVisibility(visibleStars);
        }
    }

    public void ForceUpdateAllLines()
    {
        GalaxyManager galaxyManager = FindFirstObjectByType<GalaxyManager>();
        if (galaxyManager != null)
        {
            HashSet<Star> visibleStars = new HashSet<Star>();
            if (galaxyManager.showFarStars || galaxyManager.controlledPlayer == null)
            {
                visibleStars.UnionWith(galaxyManager.stars);
            }
            else
            {
                foreach (Star owned in galaxyManager.controlledPlayer.Stars)
                {
                    visibleStars.Add(owned);
                    // Utiliser StarGraphManager pour obtenir les voisins
                    StarGraphManager starGraphManager = FindFirstObjectByType<StarGraphManager>();
                    if (starGraphManager != null)
                    {
                        List<Star> neighbors = starGraphManager.GetNeighbors(owned);
                        foreach (Star n in neighbors)
                            visibleStars.Add(n);
                    }
                }
            }
            UpdateLinesVisibility(visibleStars);
        }
    }
}
