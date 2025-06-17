using UnityEngine;
using System.Collections.Generic;

/* 
Ce script contrôle les interactions du joueur avec les étoiles dans le jeu. 
Il permet de sélectionner des étoiles, d'envoyer des unités de ces étoiles 
sélectionnées vers une étoile cible, et de visualiser ces actions avec des effets 
visuels. Le script utilise des raycasts pour détecter les clics de souris 
et les étoiles survolées, et il gère l'interface utilisateur pour afficher 
les informations pertinentes sur les étoiles sélectionnées.
*/

public class PlayerController : MonoBehaviour
{
    public Player player; // Référence à l'objet Player du joueur

    public UIManager uiManager;
    private List<Star> selectedStars = new List<Star>();
    private Star hoveredStar;
    // Valeurs par défaut pour l'envoi rapide avec les touches A et E
    [SerializeField] private int unitsForKeyA = 10;
    [SerializeField] private int unitsForKeyE = 100;

    public int UnitsForKeyA => unitsForKeyA;
    public int UnitsForKeyE => unitsForKeyE;

    public void SetUnitsForKeyA(int value)
    {
        unitsForKeyA = Mathf.Max(0, value);
    }

    public void SetUnitsForKeyE(int value)
    {
        unitsForKeyE = Mathf.Max(0, value);
    }
    public float unitSpeed = 2.0f;
    private GalaxyManager galaxyManager;
    private LineRenderer hoverLine;
    private List<LineRenderer> hoverLines = new List<LineRenderer>();

    void Start()
    {
        galaxyManager = FindObjectOfType<GalaxyManager>();
        if (uiManager == null)
        {
            Debug.LogError("UIManager is not assigned in the PlayerController.");
        }

        // Initialiser les lignes temporaires
        for (int i = 0; i < 100; i++) // Par exemple, initialiser 100 lignes
        {
            LineRenderer line = new GameObject("HoverLine").AddComponent<LineRenderer>();
            line.positionCount = 2;
            line.startWidth = 0.2f;
            line.endWidth = 0.2f;
            line.startColor = Color.white;
            line.endColor = Color.white;
            line.enabled = false; // Désactiver initialement
            hoverLines.Add(line);
        }
    }
    void Update()
    {
        HandleMouseClick();
        HandleUnitSend();
        CheckHoveredStar();
    }

    void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit.collider != null)
            {
                Star star = hit.collider.GetComponent<Star>();
                if (star != null && star.Owner == player)
                {
                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    {
                        ToggleSelectStar(star);
                    }
                    else
                    {
                        ClearSelection();
                        SelectStar(star);
                    }
                }
            }
        }
    }

    void HandleUnitSend()
    {
        if (hoveredStar == null || selectedStars.Count == 0)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            SendUnitsToHovered(unitsForKeyA);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            SendUnitsToHovered(unitsForKeyE);
        }
    }

    void SendUnitsToHovered(int amount)
    {
        foreach (Star selectedStar in selectedStars)
        {
            if (selectedStar.Owner != player)
            {
                continue;
            }

            int unitsToSendFromStar = Mathf.Min(amount, selectedStar.units);

            if (unitsToSendFromStar > 0)
            {
                PathFinding pathFinding = FindObjectOfType<PathFinding>();
                List<Star> path = pathFinding.FindPath(selectedStar, hoveredStar);
                if (path.Count > 0)
                {
                    StartCoroutine(FindObjectOfType<UnitManager>().MoveUnits(selectedStar, path, unitsToSendFromStar));
                }
                else
                {
                    Debug.LogWarning("No path found!");
                }
            }
            else
            {
                Debug.LogWarning("No units to send!");
            }
        }
    }

    void CheckHoveredStar()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
        if (hit.collider != null)
        {
            Star star = hit.collider.GetComponent<Star>();
            if (star != null && star != hoveredStar)
            {
                hoveredStar = star;
                UpdateHoverLines();
            }
        }
        else
        {
            hoveredStar = null;
            foreach (var line in hoverLines)
            {
                line.enabled = false; // Désactiver toutes les lignes temporaires lorsque aucune étoile n'est survolée
            }
        }
    }

    void UpdateHoverLines()
    {
        int i = 0;
        foreach (var selectedStar in selectedStars)
        {
            if (hoveredStar != null && selectedStar != null)
            {
                hoverLines[i].SetPosition(0, selectedStar.transform.position);
                hoverLines[i].SetPosition(1, hoveredStar.transform.position);
                hoverLines[i].enabled = true;

                // Activer l'émission pour l'effet néon
                hoverLines[i].material.EnableKeyword("_EMISSION");
                hoverLines[i].material.SetColor("_EmissionColor", Color.white * 2.0f); // Ajustez l'intensité ici

                i++;
            }
        }
        // Désactiver les lignes non utilisées
        for (; i < hoverLines.Count; i++)
        {
            hoverLines[i].enabled = false;
        }
    }

    void SelectStar(Star star)
    {
        selectedStars.Add(star);
        star.SetSelected(true);
        star.selectedEffect.SetActive(true); // Activer l'effet de sélection

        if (uiManager != null)
        {
            uiManager.UpdateSystemInfo(star);
        }
        else
        {
            Debug.LogWarning("UIManager or selectedStar is not set.");
        }
    }

    void ToggleSelectStar(Star star)
    {
        if (selectedStars.Contains(star))
        {
            selectedStars.Remove(star);
            star.SetSelected(false);
            star.selectedEffect.SetActive(false); // Enlever l'effet de sélection

        }
        else
        {
            selectedStars.Add(star);
            star.SetSelected(true);
            star.selectedEffect.SetActive(true); // Activer l'effet de sélection

        }
    }

    void ClearSelection()
    {
        foreach (Star star in selectedStars)
        {
            star.SetSelected(false);
            star.selectedEffect.SetActive(false); // Désactiver l'effet de sélection
        }
        selectedStars.Clear();
    }
}
