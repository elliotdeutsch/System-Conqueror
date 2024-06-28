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
        LineRenderer line = new GameObject("Line").AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.SetPosition(0, starA.transform.position);
        line.SetPosition(1, starB.transform.position);

        // Définir l'épaisseur de la ligne
        line.startWidth = 0.2f;
        line.endWidth = 0.2f;

        // Définir la transparence de la ligne
        Color lineColor = new Color(1f, 1f, 1f, 0.1f); // 10% d'opacité

        // Vérifier les propriétaires des étoiles pour définir la couleur
        if (starA.owner == "Player" && starB.owner == "Player")
        {
            lineColor = starA.playerColor; // Couleur de l'étoile A pour le joueur
        }
        else if (starA.owner == "Enemy" && starB.owner == "Enemy")
        {
            lineColor = starA.enemyColor; // Couleur de l'étoile A pour le joueur
        }

        line.material = new Material(Shader.Find("Sprites/Default"));
        line.material.color = lineColor;
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
        StarGraphManager starGraphManager = FindObjectOfType<StarGraphManager>();
        List<Star> neighbors = starGraphManager.GetNeighbors(star);
        foreach (Star neighbor in neighbors)
        {
            UpdateLines(star, neighbor);
        }
    }
}
