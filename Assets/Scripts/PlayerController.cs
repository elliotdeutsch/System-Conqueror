using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;

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
    public List<Star> SelectedStars => selectedStars;
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

    // Référence pour afficher l'état du fog of war
    [SerializeField] private TextMeshProUGUI fogOfWarStatusText;

    // Ajout : lignes d'approvisionnement (chaque segment du chemin)
    private Dictionary<(Star, Star), bool> supplyLines = new Dictionary<(Star, Star), bool>();
    // Ajout : visuels des lignes d'approvisionnement (par segment)
    private Dictionary<(Star, Star), GameObject> supplyLineVisuals = new Dictionary<(Star, Star), GameObject>();

    void Start()
    {
        galaxyManager = FindFirstObjectByType<GalaxyManager>();
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
            line.startColor = new Color(1, 1, 1, 0.5f); // Couleur blanche semi-transparente
            line.endColor = new Color(1, 1, 1, 0.5f); // Couleur blanche semi-transparente
            line.enabled = false; // Désactiver initialement
            hoverLines.Add(line);
        }

        // Initialiser l'affichage du fog of war
        UpdateFogOfWarDisplay();
        StartCoroutine(SupplyLineRoutine()); // Démarre la coroutine d'approvisionnement
    }

    public void UpdateFogOfWarDisplay()
    {
        if (fogOfWarStatusText != null && galaxyManager != null)
        {
            fogOfWarStatusText.text = $"Fog of War: {(galaxyManager.showFarStars ? "Désactivé" : "Activé")} (Appuyez sur F pour basculer)";
        }
    }

    void Update()
    {
        if (DeveloperConsole.IsConsoleOpen)
            return;
        HandleMouseClick();
        HandleUnitSend();
        CheckHoveredStar();
        HandleFogOfWarToggle();
        HandleSupplyLineInput();
        UpdateSupplyLineVisuals(); // Ajout pour mettre à jour les lignes visuelles
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

        if (Input.GetKeyDown(KeyCode.Q))
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
                PathFinding pathFinding = FindFirstObjectByType<PathFinding>();
                List<Star> path = pathFinding.FindPath(selectedStar, hoveredStar);
                if (path.Count > 0)
                {
                    StartCoroutine(FindFirstObjectByType<UnitManager>().MoveUnits(selectedStar, path, unitsToSendFromStar));
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
            uiManager.SetDisplayedStar(star);
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
        foreach (var star in selectedStars)
        {
            star.SetSelected(false);
            star.selectedEffect.SetActive(false); // Enlever l'effet de sélection
        }
        selectedStars.Clear();
        if (uiManager != null)
        {
            uiManager.SetDisplayedStar(null);
        }
    }

    void HandleFogOfWarToggle()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (galaxyManager != null)
            {
                galaxyManager.showFarStars = !galaxyManager.showFarStars;
                galaxyManager.UpdateFogOfWar();
                Debug.Log($"Fog of War: {(galaxyManager.showFarStars ? "Désactivé" : "Activé")}");

                // Mettre à jour l'affichage du texte
                UpdateFogOfWarDisplay();

                // Forcer la mise à jour de toutes les lignes
                LineManager lineManager = FindFirstObjectByType<LineManager>();
                if (lineManager != null)
                {
                    lineManager.ForceUpdateAllLines();
                }
            }
        }
    }

    void HandleSupplyLineInput()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (selectedStars.Count > 0)
            {
                if (hoveredStar != null)
                {
                    foreach (var selected in selectedStars)
                    {
                        if (selected == hoveredStar) continue;
                        PathFinding pathFinding = FindFirstObjectByType<PathFinding>();
                        List<Star> path = pathFinding.FindPath(selected, hoveredStar);
                        if (path.Count > 1)
                        {
                            for (int i = 0; i < path.Count - 1; i++)
                            {
                                var seg = (path[i], path[i + 1]);
                                supplyLines[seg] = true;
                                CreateSupplyLineVisual(path[i], path[i + 1]);
                            }
                        }
                    }
                }
                else
                {
                    // Annuler toutes les lignes d'approvisionnement pour les planètes sélectionnées
                    List<(Star, Star)> toRemove = new List<(Star, Star)>();
                    foreach (var seg in supplyLines.Keys)
                    {
                        if (selectedStars.Contains(seg.Item1))
                        {
                            toRemove.Add(seg);
                        }
                    }
                    foreach (var seg in toRemove)
                    {
                        supplyLines.Remove(seg);
                        RemoveSupplyLineVisual(seg.Item1, seg.Item2);
                    }
                }
            }
        }
    }

    void CreateSupplyLineVisual(Star from, Star to)
    {
        RemoveSupplyLineVisual(from, to); // Nettoyage si déjà existant
        GameObject lineObj = new GameObject($"SupplyLine_{from.starName}_to_{to.starName}");
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        Color neonColor = Color.cyan; // Couleur néon par défaut, à personnaliser
        lr.startColor = neonColor;
        lr.endColor = neonColor;
        lr.startWidth = 0.25f;
        lr.endWidth = 0.25f;
        lr.numCapVertices = 8;
        lr.numCornerVertices = 8;
        // Glow/Emission
        lr.material.EnableKeyword("_EMISSION");
        lr.material.SetColor("_EmissionColor", neonColor * 2f);
        supplyLineVisuals[(from, to)] = lineObj;
    }

    void RemoveSupplyLineVisual(Star from, Star to)
    {
        var key = (from, to);
        if (supplyLineVisuals.ContainsKey(key))
        {
            Destroy(supplyLineVisuals[key]);
            supplyLineVisuals.Remove(key);
        }
    }

    void UpdateSupplyLineVisuals()
    {
        foreach (var kvp in supplyLineVisuals)
        {
            Star from = kvp.Key.Item1;
            Star to = kvp.Key.Item2;
            GameObject lineObj = kvp.Value;
            if (from != null && to != null)
            {
                LineRenderer lr = lineObj.GetComponent<LineRenderer>();
                lr.SetPosition(0, from.transform.position);
                lr.SetPosition(1, to.transform.position);
            }
        }
    }

    // Coroutine d'approvisionnement automatique
    private IEnumerator SupplyLineRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            foreach (var seg in supplyLines.Keys)
            {
                Star source = seg.Item1;
                Star target = seg.Item2;
                if (source != null && target != null && source.Owner == player && source.units > 0)
                {
                    // Envoie direct sur le segment
                    int unitsToSend = source.units;
                    if (unitsToSend > 0)
                    {
                        PathFinding pathFinding = FindFirstObjectByType<PathFinding>();
                        List<Star> path = new List<Star> { source, target };
                        StartCoroutine(FindFirstObjectByType<UnitManager>().MoveUnits(source, path, unitsToSend));
                    }
                }
            }
        }
    }
}
