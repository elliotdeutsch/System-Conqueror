using System.Collections.Generic;
using UnityEngine;

public class GalaxyManager : MonoBehaviour
{
    public GameObject starPrefab;
    public int numberOfStars = 50;
    public float mapWidth = 100f;
    public float mapHeight = 100f;
    public Camera mainCamera;  // Assurez-vous que la caméra principale est assignée dans l'inspecteur

    private List<Star> stars = new List<Star>();
    private Dictionary<Star, List<Star>> starGraph = new Dictionary<Star, List<Star>>();

    void Start()
    {
        GenerateGalaxy();
        Star startingStar = AssignStartingStar();
        ConnectStars();
        CenterCameraOnStartingStar(startingStar);
    }

    void GenerateGalaxy()
    {
        for (int i = 0; i < numberOfStars; i++)
        {
            Vector3 position = new Vector3(Random.Range(-mapWidth / 2, mapWidth / 2), Random.Range(-mapHeight / 2, mapHeight / 2), 0);
            GameObject newStar = Instantiate(starPrefab, position, Quaternion.identity);
            newStar.name = "Star_" + i;
            Star starComponent = newStar.GetComponent<Star>();
            starComponent.starName = "Star " + i;
            stars.Add(starComponent);
            starGraph[starComponent] = new List<Star>();
        }
    }

    Star AssignStartingStar()
    {
        if (stars.Count > 0)
        {
            Star startingStar = stars[0];
            startingStar.owner = "Player";
            startingStar.units = 100; // ou tout autre nombre d'unités de départ
            startingStar.isNeutral = false;
            startingStar.SetInitialSprite();
            return startingStar;
        }
        return null;
    }

    void ConnectStars()
    {
        foreach (Star starA in stars)
        {
            int numConnections = Random.Range(1, 6);
            List<Star> closestStars = GetClosestStars(starA, numConnections);
            foreach (Star starB in closestStars)
            {
                CreateLine(starA, starB);
                starGraph[starA].Add(starB);
                starGraph[starB].Add(starA);
            }
        }
    }

    List<Star> GetClosestStars(Star star, int numClosest)
    {
        List<Star> closestStars = new List<Star>();
        SortedList<float, Star> distances = new SortedList<float, Star>();

        foreach (Star otherStar in stars)
        {
            if (otherStar != star)
            {
                float distance = Vector3.Distance(star.transform.position, otherStar.transform.position);
                distances.Add(distance, otherStar);
            }
        }

        for (int i = 0; i < Mathf.Min(numClosest, distances.Count); i++)
        {
            closestStars.Add(distances.Values[i]);
        }

        return closestStars;
    }

    void CreateLine(Star starA, Star starB)
    {
        LineRenderer line = new GameObject("Line").AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.SetPosition(0, starA.transform.position);
        line.SetPosition(1, starB.transform.position);
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.material = new Material(Shader.Find("Sprites/Default")) { color = Color.white };
    }

    public List<Star> FindPath(Star start, Star goal)
    {
        var frontier = new PriorityQueue<Star>();
        frontier.Enqueue(start, 0);

        var cameFrom = new Dictionary<Star, Star>();
        var costSoFar = new Dictionary<Star, float>();
        cameFrom[start] = null;
        costSoFar[start] = 0;

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();

            if (current == goal)
            {
                break;
            }

            foreach (var next in starGraph[current])
            {
                float newCost = costSoFar[current] + Vector3.Distance(current.transform.position, next.transform.position);
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    float priority = newCost + Vector3.Distance(next.transform.position, goal.transform.position);
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }

        var path = new List<Star>();
        var temp = goal;
        while (temp != null)
        {
            path.Add(temp);
            temp = cameFrom[temp];
        }
        path.Reverse();
        return path;
    }

    void CenterCameraOnStartingStar(Star startingStar)
    {
        if (startingStar != null && mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(startingStar.transform.position.x, startingStar.transform.position.y, mainCamera.transform.position.z);
        }
    }
}
