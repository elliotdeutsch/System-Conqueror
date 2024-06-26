using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyAI : MonoBehaviour
{
    // Intervalle de vérification en secondes
    public float checkInterval = 5f;
    // Seuil d'attaque en pourcentage
    public float attackThreshold = 0.35f;
    public GalaxyManager galaxyManager;
    public UnitManager unitManager;

    void Start()
    {
        // Initialisation des références aux gestionnaires de galaxie et d'unités
        galaxyManager = FindObjectOfType<GalaxyManager>();
        unitManager = FindObjectOfType<UnitManager>();
        // Démarrage de la coroutine de vérification des attaques
        StartCoroutine(CheckForAttacks());
    }

    IEnumerator CheckForAttacks()
    {
        // Boucle infinie pour vérifier périodiquement les conditions d'attaque
        while (true)
        {
            // Pause pour l'intervalle de vérification
            yield return new WaitForSeconds(checkInterval);
            // Parcours des étoiles pour les planètes ennemies
            foreach (Star star in galaxyManager.stars)
            {
                if (star.owner == "Enemy")
                {
                    // Tentative d'attaque si la planète appartient à l'ennemi
                    TryAttack(star);
                }
            }
        }
    }

    void TryAttack(Star enemyStar)
    {
        List<Star> neighboringStars = galaxyManager.GetNeighbors(enemyStar);
        Star targetStar = null;
        int minUnitsRequired = int.MaxValue;

        // Priorité 1 : Planètes neutres
        foreach (Star neighbor in neighboringStars)
        {
            if (neighbor.owner == "Neutral")
            {
                int requiredUnits = Mathf.CeilToInt(neighbor.units * 1.2f) + 10; // 20% + 10 unités de plus que les unités de la planète neutre
                int availableUnits = enemyStar.units;

                // Vérifier que l'IA garde au moins 10 unités après l'attaque et qu'elle a les unités nécessaires
                if (availableUnits >= requiredUnits + 10)
                {
                    if (requiredUnits > 0 && requiredUnits < minUnitsRequired)
                    {
                        minUnitsRequired = requiredUnits;
                        targetStar = neighbor;
                    }
                }
            }
        }

        // Priorité 2 : Planètes du joueur
        if (targetStar == null)
        {
            foreach (Star neighbor in neighboringStars)
            {
                if (neighbor.owner != "Enemy" && neighbor.owner != "Neutral")
                {
                    int unitsToSend = Mathf.CeilToInt(neighbor.units * (1 + attackThreshold));
                    int availableUnits = enemyStar.units - 10;

                    // Vérifier que l'IA garde au moins 10 unités après l'attaque et qu'elle a les unités nécessaires
                    if (unitsToSend > 0 && availableUnits >= unitsToSend)
                    {
                        if (unitsToSend < minUnitsRequired)
                        {
                            minUnitsRequired = unitsToSend;
                            targetStar = neighbor;
                        }
                    }
                }
            }
        }

        // Si une cible est trouvée, envoyer les unités
        if (targetStar != null && enemyStar.units >= minUnitsRequired + 10)
        {
            List<Star> path = galaxyManager.FindPath(enemyStar, targetStar);
            if (path.Count > 0)
            {
                // Envoie des unités le long du chemin trouvé
                StartCoroutine(unitManager.MoveUnits(enemyStar, path, minUnitsRequired));
            }
        }
    }
}
