using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnitManager : MonoBehaviour
{
    public GameObject unitPrefab;
    public PlayerController playerController;
    public GalaxyManager galaxyManager;
    public float unitSpeed = 2.0f;
    public TextMeshPro textMesh;

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

        if (galaxyManager == null)
        {
            galaxyManager = FindObjectOfType<GalaxyManager>();
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

        if (fromStar.owner == "Player")
        {
            unitsToSend = Mathf.Min(unitsToSend, fromStar.units);
        }
        else if (fromStar.owner == "Enemy")
        {
            unitsToSend = Mathf.Min(unitsToSend, fromStar.units - 10);
        }

        if (unitsToSend <= 0)
        {
            Debug.LogWarning("Not enough units to send!");
            yield break;
        }

        fromStar.units -= unitsToSend;

        GameObject unitInstance = Instantiate(unitPrefab, fromStar.transform.position, Quaternion.identity);
        Unit unitScript = unitInstance.GetComponent<Unit>();
        if (unitScript == null)
        {
            Debug.LogError("Unit script is not found on the unitPrefab!");
            yield break;
        }

        // Ajoutez un GameObject enfant pour TextMeshPro
        GameObject textObject = new GameObject("UnitText");
        textObject.transform.SetParent(unitInstance.transform);
        TextMeshPro textMesh = textObject.AddComponent<TextMeshPro>();
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.fontSize = 5;
        textMesh.text = unitsToSend.ToString();

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(0, 0.5f, 0);
        rectTransform.sizeDelta = new Vector2(1, 1);

        unitScript.textMesh = textMesh; // Associez le composant texte à l'unité
        unitScript.Initialize(fromStar, path[path.Count - 1], unitsToSend);

        for (int i = 1; i < path.Count; i++)
        {
            Star currentStar = path[i];
            while (unitInstance.transform.position != currentStar.transform.position)
            {
                unitInstance.transform.position = Vector3.MoveTowards(unitInstance.transform.position, currentStar.transform.position, unitSpeed * Time.deltaTime);
                yield return null;
            }

            if (currentStar.owner == fromStar.owner && currentStar != path[path.Count - 1])
            {
                continue;
            }
            else if (currentStar.owner == fromStar.owner && currentStar == path[path.Count - 1])
            {
                currentStar.units += unitsToSend;
                Destroy(unitInstance);
                yield break;
            }
            else if (currentStar.owner != fromStar.owner)
            {
                if (unitsToSend > currentStar.units)
                {
                    currentStar.Conquer(fromStar, unitsToSend);
                    Destroy(unitInstance);
                    yield break;
                }
                else
                {
                    currentStar.units -= unitsToSend;
                    Destroy(unitInstance);
                    yield break;
                }
            }
        }

        Destroy(unitInstance);
    }


    public void SendUnits(Star fromStar, Star toStar, int unitsToSend)
    {
        List<Star> path = galaxyManager.FindPath(fromStar, toStar); // Utiliser galaxyManager ici
        if (path.Count > 0)
        {
            StartCoroutine(MoveUnits(fromStar, path, unitsToSend));
        }
    }
}
