using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using System.Collections;

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
    [Header("Game Objects")]
    public GameObject starPrefab;
    public GameObject unitPrefab;

    [Header("Map Settings")]
    public int numberOfStars = 50;
    public float mapWidth = 100f;
    public float mapHeight = 100f;
    public float minStarDistance = 5f; // Distance minimale entre les étoiles

    [Header("Gameplay Settings")]
    public int numberOfPlayers = 2;
    public int numberOfAI = 3;
    public bool showFarStars = false; // When false, player only sees owned stars and their immediate neighbours

    [Header("Asset References")]
    public Sprite haloSprite;
    public Sprite glowSprite;

    public Camera mainCamera;  // Assurez-vous que la caméra principale est assignée dans l'inspecteur
    public List<Player> players = new List<Player>();

    public List<Star> stars = new List<Star>();

    private Dictionary<Star, List<Star>> starGraph = new Dictionary<Star, List<Star>>();

    private StarNameGenerator starNameGenerator;
    private PathFinding pathFinding; // Nouvelle référence à PathFinding
    private StarGraphManager starGraphManager; // Nouvelle référence à StarGraphManager
    private StarConnectionHandler starConnectionHandler; // Nouvelle référence à StarConnectionHandler
    private StartingStarAssignment startingStarAssignment; // Nouvelle référence à StartingStarAssignment
    public Player controlledPlayer;

    // Références pour l'UI
    public GameObject playerListContainer; // Conteneur pour la liste des joueurs
    public GameObject playerNamePrefab; // Prefab pour afficher le nom d'un joueur

    void Start()
    {
        // Galaxy initialization is triggered from GameSetupUI
    }

    public void InitializeGalaxy()
    {
        starNameGenerator = GetComponent<StarNameGenerator>();
        if (starNameGenerator == null)
        {
            Debug.LogError("StarNameGenerator component is missing!");
            return;
        }

        players = new List<Player>(); // Assurez-vous d'initialiser la liste des joueurs

        // Initialiser les joueurs
        for (int i = 0; i < numberOfPlayers; i++)
        {
            players.Add(new Player("Player" + (i + 1), GetDistinctColor(i, numberOfPlayers + numberOfAI), false));
        }

        // Initialiser les IA
        for (int i = 0; i < numberOfAI; i++)
        {
            players.Add(new Player("AI" + (i + 1), GetDistinctColor(numberOfPlayers + i, numberOfPlayers + numberOfAI), true));
        }

        // Assigner un joueur contrôlé si des joueurs sont définis
        if (numberOfPlayers > 0)
        {
            controlledPlayer = players.First(p => !p.IsAI);
        }

        GenerateGalaxy();

        startingStarAssignment = GetComponent<StartingStarAssignment>(); // Initialiser StartingStarAssignment
        // On initialise juste la liste ici, la distribution se fera après le graphe
        startingStarAssignment.Initialize(stars);

        // Centrer la caméra sur la planète de départ du joueur contrôlé
        CenterCameraOnPlayerStartingStar();

        // Mettre à jour les limites de la caméra
        CameraController cameraController = FindFirstObjectByType<CameraController>();
        if (cameraController != null)
        {
            cameraController.UpdatePanLimits();
        }

        // Assurez-vous que toutes les étoiles sont ajoutées au graphe avant de connecter
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

        // Maintenant que le graphe est prêt, on peut assigner les planètes de départ
        startingStarAssignment.Initialize(stars, starGraphManager);
        startingStarAssignment.AssignStartingStars(players);

        // Si un joueur est contrôlé, le lier au PlayerController
        if (controlledPlayer != null)
        {
            PlayerController playerController = FindFirstObjectByType<PlayerController>();
            if (playerController != null)
            {
                playerController.player = controlledPlayer;
            }
        }

        UpdatePlayerListUI();
        StartCoroutine(DelayedFogOfWar());
    }

    void GenerateGalaxy()
    {
        // Calcul automatique de la taille de la carte
        float mapSize = Mathf.Ceil(Mathf.Sqrt(numberOfStars) * 12f);
        mapWidth = mapSize;
        mapHeight = mapSize;

        starNameGenerator = GetComponent<StarNameGenerator>(); // Ajout de cette ligne
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

    void CenterCameraOnPlayerStartingStar()
    {
        if (controlledPlayer != null && controlledPlayer.Stars.Count > 0)
        {
            Star startingStar = controlledPlayer.Stars[0];
            CenterCameraOnStartingStar(startingStar);
        }
    }

    Color GetDistinctColor(int index, int total)
    {
        float hue = (float)index / total;
        float saturation = 0.8f;
        float value = 0.8f;
        return Color.HSVToRGB(hue, saturation, value);
    }

    public void UpdatePlayerListUI()
    {
        if (playerListContainer == null || playerNamePrefab == null)
        {
            Debug.LogError("Player list container or player name prefab is not assigned!");
            return;
        }

        // Assurez-vous que le PlayerListContainer est actif
        playerListContainer.SetActive(true);

        foreach (Transform child in playerListContainer.transform)
        {
            Destroy(child.gameObject);
        }

        // Trier les joueurs par nombre de planètes possédées (du plus au moins)
        var sortedPlayers = players.OrderByDescending(p => p.Stars.Count).ToList();

        foreach (var player in sortedPlayers)
        {
            GameObject playerNameObject = Instantiate(playerNamePrefab, playerListContainer.transform);
            TextMeshProUGUI textComponent = playerNameObject.GetComponentInChildren<TextMeshProUGUI>(); // Utiliser GetComponentInChildren

            if (textComponent != null)
            {
                int totalStars = stars.Count;
                float percentage = (float)player.Stars.Count / totalStars * 100;
                textComponent.text = $"{player.Name} : {player.Stars.Count} ({percentage:F1}%)" + (player.IsAI ? " (IA)" : "");
                textComponent.text = $"{player.Name}" + (player.IsAI ? " (IA)" : "") + $" : {player.Stars.Count}, ({percentage:F1}%)";
                textComponent.color = player.Color;
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component is missing on the player name prefab!");
            }
        }
    }

    // Met à jour la visibilité des étoiles et des lignes selon l'option choisie
    public void UpdateFogOfWar()
    {
        HashSet<Star> visibleStars = new HashSet<Star>();

        if (showFarStars || controlledPlayer == null)
        {
            // Mode sans fog of war : toutes les étoiles sont considérées comme visibles
            visibleStars.UnionWith(stars);
        }
        else
        {
            // Mode fog of war : seulement les étoiles possédées et leurs voisines
            foreach (Star owned in controlledPlayer.Stars)
            {
                visibleStars.Add(owned);
                if (starGraph.TryGetValue(owned, out List<Star> neigh))
                {
                    foreach (Star n in neigh)
                        visibleStars.Add(n);
                }
            }
        }

        // Toutes les étoiles sont affichées, mais leur état dépend de leur présence dans visibleStars
        foreach (Star s in stars)
        {
            s.SetVisibility(visibleStars.Contains(s));
        }

        LineManager lineManager = FindFirstObjectByType<LineManager>();
        if (lineManager != null)
        {
            lineManager.ForceUpdateAllLines();
        }
    }

    private IEnumerator DelayedFogOfWar()
    {
        yield return null; // attendre la fin du frame, donc tous les Start()
        UpdateFogOfWar();
    }
}
