using System.Collections.Generic;
using UnityEngine;

public class StartingStarAssignment : MonoBehaviour
{
    private List<Star> stars;
    private StarGraphManager starGraphManager;

    public void Initialize(List<Star> starList, StarGraphManager graphManager = null)
    {
        stars = starList;
        if (graphManager != null)
            starGraphManager = graphManager;
        else
            starGraphManager = FindObjectOfType<StarGraphManager>();
    }

    public void AssignStartingStars(List<Player> players)
    {
        var availableStars = new List<Star>(stars);
        var chosenStars = new List<Star>();
        foreach (var player in players)
        {
            Star bestStar = null;
            int bestMinDist = -1;
            foreach (var candidate in availableStars)
            {
                // Calculer la distance minimale à toutes les planètes déjà choisies
                int minDist = int.MaxValue;
                foreach (var chosen in chosenStars)
                {
                    int dist = GetGraphDistance(candidate, chosen);
                    if (dist < minDist) minDist = dist;
                }
                if (chosenStars.Count == 0) minDist = int.MaxValue; // Premier joueur : n'importe où
                // On veut au moins 2 sauts d'écart si possible
                if ((minDist > bestMinDist) && (minDist >= 2 || bestMinDist < 2))
                {
                    bestMinDist = minDist;
                    bestStar = candidate;
                }
            }
            if (bestStar == null)
            {
                bestStar = availableStars[0]; // fallback
            }
            // Assigner la planète
            bestStar.Owner = player;
            bestStar.units = 100;
            bestStar.isNeutral = false;
            bestStar.starType = Star.StarType.Capital;
            bestStar.starName += " (Capitale)";
            bestStar.SetInitialSprite();
            player.Stars.Add(bestStar);
            availableStars.Remove(bestStar);
            chosenStars.Add(bestStar);
        }
    }

    public Star AssignStartingStar(Player player)
    {
        if (stars.Count > 0)
        {
            Star startingStar = stars[0]; // Choisir une étoile de départ non assignée
            startingStar.Owner = player;
            startingStar.units = 100; // Nombre d'unités de départ
            startingStar.isNeutral = false;

            // Assigner le type d'étoile
            startingStar.starType = Star.StarType.Capital; // Assigner comme étoile mère

            // Ajouter "(Capitale)" au nom de l'étoile
            startingStar.starName += " (Capitale)";

            startingStar.SetInitialSprite();
            stars.Remove(startingStar); // Retirer cette étoile de la liste des étoiles disponibles
            return startingStar;
        }
        return null;
    }

    // Calcule la distance en sauts entre deux étoiles via le graphe
    public int GetGraphDistance(Star from, Star to)
    {
        if (from == to) return 0;
        if (starGraphManager == null) return int.MaxValue;
        var visited = new HashSet<Star>();
        var queue = new Queue<(Star, int)>();
        queue.Enqueue((from, 0));
        visited.Add(from);
        while (queue.Count > 0)
        {
            var (current, dist) = queue.Dequeue();
            foreach (var neighbor in starGraphManager.GetNeighbors(current))
            {
                if (neighbor == to) return dist + 1;
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue((neighbor, dist + 1));
                }
            }
        }
        return int.MaxValue; // Non accessible
    }
}
