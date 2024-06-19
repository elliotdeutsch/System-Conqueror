using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public GameObject unitPrefab;
    public PlayerController playerController;
    public float unitSpeed = 2.0f;

    void Start()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        if (unitPrefab == null)
        {
            Debug.LogError("Unit prefab is not assigned in the inspector!");
        }

        if (playerController == null)
        {
            Debug.LogError("PlayerController is not assigned in the inspector or could not be found!");
        }
    }

    public IEnumerator MoveUnits(Star fromStar, List<Star> path, int unitsToSend)
    {
        if (unitPrefab == null)
        {
            Debug.LogError("Unit prefab is not assigned!");
            yield break;
        }

        if (fromStar == null)
        {
            Debug.LogError("FromStar is null!");
            yield break;
        }

        if (path == null || path.Count == 0)
        {
            Debug.LogError("Path is null or empty!");
            yield break;
        }

        fromStar.units -= unitsToSend; // Déduire les unités de l'étoile d'origine
        GameObject unitInstance = Instantiate(unitPrefab, fromStar.transform.position, Quaternion.identity);
        Unit unitScript = unitInstance.GetComponent<Unit>();
        if (unitScript == null)
        {
            Debug.LogError("Unit script is not found on the unitPrefab!");
            yield break;
        }

        unitScript.Initialize(fromStar, path[path.Count - 1], unitsToSend);

        for (int i = 1; i < path.Count; i++)
        {
            Star currentStar = path[i];
            while (unitInstance.transform.position != currentStar.transform.position)
            {
                unitInstance.transform.position = Vector3.MoveTowards(unitInstance.transform.position, currentStar.transform.position, unitSpeed * Time.deltaTime);
                yield return null;
            }

            if (currentStar.owner == "Player" && currentStar != path[path.Count - 1])
            {
                // Si la planète appartient au joueur et n'est pas la destination finale, continuer le chemin
                continue;
            }
            else if (currentStar.owner == "Player" && currentStar == path[path.Count - 1])
            {
                // Transfert entre planètes alliées
                currentStar.units += unitsToSend;
                Destroy(unitInstance);
                yield break;
            }
            else if (currentStar.owner != "Player")
            {
                if (unitsToSend >= currentStar.units)
                {
                    int remainingUnits = unitsToSend - currentStar.units; // Unités restantes après la conquête
                    unitsToSend -= currentStar.units;
                    currentStar.units = remainingUnits; // Les unités excédentaires deviennent les nouvelles unités de la planète conquise
                    currentStar.owner = fromStar.owner;
                    currentStar.SetInitialSprite();
                    if (currentStar.owner == "Player")
                    {
                        StartCoroutine(currentStar.GenerateUnits());
                    }
                }
                else
                {
                    currentStar.units -= unitsToSend;
                    unitsToSend = 0;
                    Destroy(unitInstance);
                    yield break;
                }
            }
        }

        Destroy(unitInstance);
    }
}
