using System.Collections.Generic;
using UnityEngine;

/*
Ce script gère la génération, la connexion et la navigation au sein d'une galaxie 
de jeu. Il initialise les étoiles avec des noms générés de manière procédurale et 
les place sur une carte tout en s'assurant qu'elles ne se chevauchent pas. Le 
script établit un graphe des connexions entre les étoiles et utilise l'algorithme A*
pour la recherche de chemins, facilitant ainsi le déplacement des unités entre les étoiles.
De plus, il assigne des étoiles de départ pour le joueur et les ennemis et centre la caméra
sur l'étoile de départ du joueur.
*/

public class GalaxyManager : MonoBehaviour
{
    public GameObject starPrefab;
    public int numberOfStars = 50;
    public float mapWidth = 100f;
    public float mapHeight = 100f;
    public float minStarDistance = 5f; // Distance minimale entre les étoiles
    public GameObject unitPrefab;
    public Camera mainCamera;  // Assurez-vous que la caméra principale est assignée dans l'inspecteur

    public List<Star> stars = new List<Star>();

    private Dictionary<Star, List<Star>> starGraph = new Dictionary<Star, List<Star>>();
    private StarNameGenerator starNameGenerator;

    void Start()
    {
        starNameGenerator = GetComponent<StarNameGenerator>();
        if (starNameGenerator == null)
        {
            Debug.LogError("StarNameGenerator component is missing!");
            return;
        }

        GenerateGalaxy();
        Star startingStar = AssignStartingStar();
        AssignEnemyStartingStar();  // Ajouter l'étoile de départ pour l'ennemi
        ConnectStars();
        EnsureFullConnectivity();
        CenterCameraOnStartingStar(startingStar);
    }


    void GenerateGalaxy()
    {
        for (int i = 0; i < numberOfStars; i++)
        {
            Vector3 position = new Vector3(Random.Range(-mapWidth / 2, mapWidth / 2), Random.Range(-mapHeight / 2, mapHeight / 2), 0);
            if (IsValidPosition(position))
            {
                GameObject newStar = Instantiate(starPrefab, position, Quaternion.identity);
                newStar.name = "Star_" + i;
                Star starComponent = newStar.GetComponent<Star>();
                if (starComponent != null)
                {
                    starComponent.starName = starNameGenerator.GenerateStarName();
                }
                else
                {
                    Debug.LogError("Star component is missing on the star prefab!");
                }
                stars.Add(starComponent);
                starGraph[starComponent] = new List<Star>();
            }
        }
    }

    bool IsValidPosition(Vector3 position)
    {
        foreach (Star star in stars)
        {
            if (Vector3.Distance(position, star.transform.position) < minStarDistance)
            {
                return false;
            }
        }
        return true;
    }

    void EnsureFullConnectivity()
    {
        List<List<Star>> clusters = GetClusters();
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
                CreateLine(closestA, closestB);
                starGraph[closestA].Add(closestB);
                starGraph[closestB].Add(closestA);
            }

            clusters = GetClusters();
        }
    }

    List<List<Star>> GetClusters()
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
    Star AssignStartingStar()
    {
        if (stars.Count > 0)
        {
            Star startingStar = stars[0];
            startingStar.owner = "Player";
            startingStar.units = 100; // Nombre d'unités de départ
            startingStar.isNeutral = false;
            startingStar.starType = Star.StarType.MotherBaseAllied; // Assigner comme étoile mère alliée
            startingStar.SetInitialSprite();
            StartCoroutine(startingStar.GenerateUnits(15, 5)); // 15 unités toutes les 5 secondes
            return startingStar;
        }
        return null;
    }

    void AssignEnemyStartingStar()
    {
        if (stars.Count > 1)
        {
            Star enemyStartingStar = stars[1];  // Assumons que la deuxième étoile est pour l'ennemi
            enemyStartingStar.owner = "Enemy";
            enemyStartingStar.units = 100;  // Nombre d'unités de départ pour l'ennemi
            enemyStartingStar.isNeutral = false;
            enemyStartingStar.starType = Star.StarType.MotherBaseEnemy; // Assigner comme étoile mère ennemie
            enemyStartingStar.SetInitialSprite();
            StartCoroutine(enemyStartingStar.GenerateUnits(15, 5));  // 15 unités toutes les 5 secondes pour l'étoile ennemie
        }
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

    void CreateLine(Star starA, Star starB)
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
        List<Star> neighbors = GetNeighbors(star);
        foreach (Star neighbor in neighbors)
        {
            UpdateLines(star, neighbor);
        }
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

    public List<Star> GetNeighbors(Star star)
    {
        if (starGraph.ContainsKey(star))
        {
            return starGraph[star];
        }
        return new List<Star>();
    }

}


