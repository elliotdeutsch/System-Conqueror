using System.Collections.Generic;
using UnityEngine;

public class StartingStarAssignment : MonoBehaviour
{
    private List<Star> stars;

    public void Initialize(List<Star> starList)
    {
        stars = starList;
    }

    public Star AssignStartingStar()
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

    public void AssignEnemyStartingStar()
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
}
