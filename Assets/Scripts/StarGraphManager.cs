using System.Collections.Generic;
using UnityEngine;

/*
Le script StarGraphManager est responsable de gérer les connexions et les relations
entre les étoiles dans un jeu. Il fournit des méthodes pour identifier les clusters d'étoiles,
obtenir les voisins d'une étoile spécifique et trouver les étoiles les plus proches d'une étoile donnée.
*/

public class StarGraphManager : MonoBehaviour
{
    private Dictionary<Star, List<Star>> starGraph;
    private List<Star> stars;

    public void Initialize(Dictionary<Star, List<Star>> graph, List<Star> starList)
    {
        starGraph = graph;
        stars = starList;
    }

    public List<List<Star>> GetClusters()
    {
        List<List<Star>> clusters = new List<List<Star>>();
        HashSet<Star> visited = new HashSet<Star>();

        foreach (Star star in stars)
        {
            if (!visited.Contains(star))
            {
                List<Star> cluster = new List<Star>();
                Queue<Star> queue = new Queue<Star>();
                queue.Enqueue(star);
                visited.Add(star);

                while (queue.Count > 0)
                {
                    Star current = queue.Dequeue();
                    cluster.Add(current);

                    foreach (Star neighbor in starGraph[current])
                    {
                        if (!visited.Contains(neighbor))
                        {
                            queue.Enqueue(neighbor);
                            visited.Add(neighbor);
                        }
                    }
                }

                clusters.Add(cluster);
            }
        }

        return clusters;
    }

    public List<Star> GetNeighbors(Star star)
    {
        if (starGraph.ContainsKey(star))
        {
            return starGraph[star];
        }
        return new List<Star>();
    }

    public List<Star> GetClosestStars(Star star, int numClosest)
    {
        List<Star> closestStars = new List<Star>();
        SortedDictionary<float, List<Star>> distances = new SortedDictionary<float, List<Star>>();

        foreach (Star otherStar in stars)
        {
            if (otherStar != star)
            {
                float distance = Vector3.Distance(star.transform.position, otherStar.transform.position);
                if (!distances.ContainsKey(distance))
                {
                    distances[distance] = new List<Star>();
                }
                distances[distance].Add(otherStar);
            }
        }

        foreach (var kvp in distances)
        {
            foreach (var s in kvp.Value)
            {
                closestStars.Add(s);
                if (closestStars.Count >= numClosest)
                {
                    break;
                }
            }
            if (closestStars.Count >= numClosest)
            {
                break;
            }
        }
        return closestStars;
    }
}
