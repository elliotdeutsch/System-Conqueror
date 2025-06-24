using System.Collections.Generic;
using UnityEngine;

/*
Le script StarConnectionHandler est responsable de gérer les connexions entre les
étoiles dans le jeu. Il comprend deux principales fonctionnalités : connecter
les étoiles entre elles et assurer la connectivité complète de toutes les étoiles. 
*/

public class StarConnectionHandler : MonoBehaviour
{
    private Dictionary<Star, List<Star>> starGraph;
    private List<Star> stars;

    public void Initialize(Dictionary<Star, List<Star>> graph, List<Star> starList)
    {
        starGraph = graph;
        stars = starList;
    }

    public void ConnectStars()
    {
        foreach (Star starA in stars)
        {
            int numConnections = Random.Range(1, 6);
            StarGraphManager starGraphManager = FindFirstObjectByType<StarGraphManager>();

            List<Star> closestStars = starGraphManager.GetClosestStars(starA, numConnections);
            foreach (Star starB in closestStars)
            {
                LineManager lineManager = FindFirstObjectByType<LineManager>();
                lineManager.CreateLine(starA, starB);

                if (!starGraph[starA].Contains(starB)) // Éviter les doublons
                {
                    starGraph[starA].Add(starB);
                }
                if (!starGraph[starB].Contains(starA)) // Éviter les doublons
                {
                    starGraph[starB].Add(starA);
                }
            }
        }
    }


    public void EnsureFullConnectivity()
    {
        StarGraphManager starGraphManager = FindFirstObjectByType<StarGraphManager>();

        List<List<Star>> clusters = starGraphManager.GetClusters();
        while (clusters.Count > 1)
        {
            List<Star> clusterA = clusters[0];
            List<Star> clusterB = clusters[1];
            float minDistance = float.MaxValue;
            Star closestA = null;
            Star closestB = null;

            foreach (Star starA in clusterA)
            {
                foreach (Star starB in clusterB)
                {
                    float distance = Vector3.Distance(starA.transform.position, starB.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestA = starA;
                        closestB = starB;
                    }
                }
            }

            if (closestA != null && closestB != null)
            {
                LineManager lineManager = FindFirstObjectByType<LineManager>();
                lineManager.CreateLine(closestA, closestB);
                starGraph[closestA].Add(closestB);
                starGraph[closestB].Add(closestA);
            }

            clusters = starGraphManager.GetClusters();
        }
    }

}