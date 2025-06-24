using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Ce script implémente une intelligence artificielle basique pour les ennemis dans 
le jeu. L'IA vérifie périodiquement si des attaques peuvent être lancées contre 
des étoiles neutres ou appartenant au joueur. Elle prend des décisions en fonction 
du nombre d'unités disponibles et des unités nécessaires pour garantir une attaque réussie.
Le script utilise des coroutines pour gérer les vérifications périodiques et les mouvements
des unités, ce qui permet de réaliser ces opérations de manière asynchrone et non bloquante.
Ce comportement ajoute une dimension stratégique au jeu en permettant aux ennemis de réagir
et d'agir de manière autonome.
*/

public class BasicEnemyAI : MonoBehaviour
{
    // Intervalle de vérification en secondes
    public float checkInterval = 5f;
    public float attackThreshold = 0.35f;
    public GalaxyManager galaxyManager;
    public UnitManager unitManager;
    private StarGraphManager starGraphManager;
    private PathFinding pathFinding;

    // Cooldown de 3 secondes entre les attaques pour chaque IA
    private Dictionary<Player, int> lastAttackTime = new Dictionary<Player, int>();
    private bool gameStarted = false;

    void Start()
    {
        // Initialisation des références aux gestionnaires de galaxie et d'unités
        galaxyManager = FindFirstObjectByType<GalaxyManager>();
        unitManager = FindFirstObjectByType<UnitManager>();
        starGraphManager = FindFirstObjectByType<StarGraphManager>();
        pathFinding = FindFirstObjectByType<PathFinding>();

        // S'abonner au timer global pour les vérifications
        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.OnFiveSecondInterval += OnFiveSecondInterval;
        }
    }

    void OnDestroy()
    {
        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.OnFiveSecondInterval -= OnFiveSecondInterval;
        }
    }

    // Méthode appelée toutes les 5 secondes par le timer global
    void OnFiveSecondInterval()
    {
        if (!gameStarted)
        {
            // Attendre 5 secondes de jeu avant de commencer
            if (GameTimer.Instance.currentTime >= 5)
            {
                gameStarted = true;
            }
            else
            {
                return; // Ne pas attaquer avant 5s
            }
        }

        // Vérifier les attaques pour toutes les IA
        foreach (Star star in galaxyManager.stars)
        {
            if (star.Owner != null && star.Owner.IsAI)
            {
                TryAttack(star);
            }
        }
    }

    void TryAttack(Star enemyStar)
    {
        // Vérifier le cooldown de 3 secondes pour cette IA
        if (lastAttackTime.ContainsKey(enemyStar.Owner))
        {
            int timeSinceLastAttack = GameTimer.Instance.currentTime - lastAttackTime[enemyStar.Owner];
            if (timeSinceLastAttack < 3)
            {
                return; // Cooldown actif
            }
        }

        List<Star> neighboringStars = starGraphManager.GetNeighbors(enemyStar);
        Star targetStar = null;
        int minUnitsRequired = int.MaxValue;

        // Priorité 1 : Planètes neutres
        foreach (Star neighbor in neighboringStars)
        {
            if (neighbor.Owner == null) // Étoile neutre
            {
                // Calculer le temps de trajet estimé (1 seconde par segment)
                int pathLength = 1; // Distance minimale pour un voisin
                int estimatedTravelTime = pathLength;

                // Calculer les unités qui seront générées pendant le trajet
                int unitsGeneratedDuringTravel = estimatedTravelTime * 2; // 2 unités par seconde

                // Calculer les unités nécessaires avec bonus pour la génération pendant le trajet
                int baseRequiredUnits = Mathf.CeilToInt(neighbor.units * 1.3f) + 12;
                int totalRequiredUnits = baseRequiredUnits + unitsGeneratedDuringTravel;

                int availableUnits = enemyStar.units;

                // Vérifier que l'IA garde au moins 10 unités après l'attaque et qu'elle a les unités nécessaires
                if (availableUnits >= totalRequiredUnits + 10)
                {
                    if (totalRequiredUnits > 0 && totalRequiredUnits < minUnitsRequired)
                    {
                        minUnitsRequired = totalRequiredUnits;
                        targetStar = neighbor;
                    }
                }
            }
        }

        // Priorité 2 : Planètes du joueur ou d'autres IA
        if (targetStar == null)
        {
            foreach (Star neighbor in neighboringStars)
            {
                if (neighbor.Owner != null && neighbor.Owner != enemyStar.Owner)
                {
                    // Calculer le temps de trajet estimé (1 seconde par segment)
                    int pathLength = 1; // Distance minimale pour un voisin
                    int estimatedTravelTime = pathLength;

                    // Calculer les unités qui seront générées pendant le trajet
                    int unitsGeneratedDuringTravel = estimatedTravelTime * 2; // 2 unités par seconde

                    // Calculer les unités nécessaires avec bonus pour la génération pendant le trajet
                    int baseUnitsToSend = Mathf.CeilToInt(neighbor.units * (1 + attackThreshold + 0.05f));
                    int totalUnitsToSend = baseUnitsToSend + unitsGeneratedDuringTravel;

                    int availableUnits = enemyStar.units - 10;

                    // Vérifier que l'IA garde au moins 10 unités après l'attaque et qu'elle a les unités nécessaires
                    if (totalUnitsToSend > 0 && availableUnits >= totalUnitsToSend)
                    {
                        if (totalUnitsToSend < minUnitsRequired)
                        {
                            minUnitsRequired = totalUnitsToSend;
                            targetStar = neighbor;
                        }
                    }
                }
            }
        }

        // Si une cible est trouvée, envoyer les unités
        if (targetStar != null && enemyStar.units >= minUnitsRequired + 10)
        {
            // Enregistrer le temps de cette attaque
            lastAttackTime[enemyStar.Owner] = GameTimer.Instance.currentTime;

            List<Star> path = pathFinding.FindPath(enemyStar, targetStar);
            if (path.Count > 0)
            {
                // Envoie des unités le long du chemin trouvé
                StartCoroutine(unitManager.MoveUnits(enemyStar, path, minUnitsRequired));
            }
        }
    }
}
