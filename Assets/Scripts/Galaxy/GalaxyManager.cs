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

    public int numberOfPlayers = 2;
    public int numberOfAI = 3;
    private List<Player> players;
    public List<Star> stars = new List<Star>();

    private Dictionary<Star, List<Star>> starGraph = new Dictionary<Star, List<Star>>();
    private StarNameGenerator starNameGenerator;
    private PathFinding pathFinding; // Nouvelle référence à PathFinding
    private StarGraphManager starGraphManager; // Nouvelle référence à StarGraphManager
    private StarConnectionHandler starConnectionHandler; // Nouvelle référence à StarConnectionHandler
    private StartingStarAssignment startingStarAssignment; // Nouvelle référence à StartingStarAssignment

    void Start()
    {
        starNameGenerator = GetComponent<StarNameGenerator>();
        if (starNameGenerator == null)
        {
            Debug.LogError("StarNameGenerator component is missing!");
            return;
        }

        players = new List<Player>();

        // Initialiser les joueurs
        for (int i = 0; i < numberOfPlayers; i++)
        {
            players.Add(new Player("Player" + (i + 1), GetRandomColor(), false));
        }

        // Initialiser les IA
        for (int i = 0; i < numberOfAI; i++)
        {
            players.Add(new Player("AI" + (i + 1), GetRandomColor(), true));
        }

        GenerateGalaxy();

        startingStarAssignment = GetComponent<StartingStarAssignment>(); // Initialiser StartingStarAssignment
        startingStarAssignment.Initialize(stars); // Passer la liste des étoiles
        startingStarAssignment.AssignStartingStars(players); // Assigner les étoiles de départ

        // Assurez-vous que toutes les étoiles sont ajoutées au graphe
        foreach (var star in stars)
        {
            if (!starGraph.ContainsKey(star))
            {
                starGraph[star] = new List<Star>();
            }
        }

        starGraphManager = GetComponent<StarGraphManager>(); // Initialiser StarGraphManager
        starGraphManager.Initialize(starGraph, stars); // Passer le graphe des étoiles et la liste des étoiles

        starConnectionHandler = GetComponent<StarConnectionHandler>(); // Initialiser StarConnectionHandler
        starConnectionHandler.Initialize(starGraph, stars); // Passer le graphe des étoiles et la liste des étoiles
        starConnectionHandler.ConnectStars();
        starConnectionHandler.EnsureFullConnectivity();

        pathFinding = GetComponent<PathFinding>(); // Initialiser PathFinding
        pathFinding.Initialize(starGraph); // Passer le graphe des étoiles
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

    void CenterCameraOnStartingStar(Star startingStar)
    {
        if (startingStar != null && mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(startingStar.transform.position.x, startingStar.transform.position.y, mainCamera.transform.position.z);
        }
    }

    Color GetRandomColor()
    {
        return new Color(Random.value, Random.value, Random.value);
    }
}
