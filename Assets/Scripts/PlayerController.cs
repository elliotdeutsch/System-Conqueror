using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public UIManager uiManager;
    public GameObject unitPrefab;
    private List<Star> selectedStars = new List<Star>();
    private Star hoveredStar;
    private int unitsToSend = 10;
    public float unitSpeed = 2.0f;
    private GalaxyManager galaxyManager;

    void Start()
    {
        galaxyManager = FindObjectOfType<GalaxyManager>();
        if (uiManager == null)
        {
            Debug.LogError("UIManager is not assigned in the PlayerController.");
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
                if (star != null)
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
        if (Input.GetKeyDown(KeyCode.O) && hoveredStar != null && selectedStars.Count > 0)
        {
            foreach (Star selectedStar in selectedStars)
            {
                if (selectedStar.owner == "Player")
                {
                    int unitsToSendFromStar = Mathf.Min(unitsToSend, selectedStar.units);

                    if (unitsToSendFromStar > 0)
                    {
                        List<Star> path = galaxyManager.FindPath(selectedStar, hoveredStar);
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
        }
    }

    void CheckHoveredStar()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
        if (hit.collider != null)
        {
            Star star = hit.collider.GetComponent<Star>();
            if (star != null)
            {
                if (hoveredStar != star)
                {
                    hoveredStar = star;
                }
            }
        }
        else
        {
            hoveredStar = null;
        }
    }

    void SelectStar(Star star)
    {
        selectedStars.Add(star);
        star.SetSelected(true);
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
        }
        else
        {
            selectedStars.Add(star);
            star.SetSelected(true);
        }
    }

    void ClearSelection()
    {
        foreach (Star star in selectedStars)
        {
            star.SetSelected(false);
        }
        selectedStars.Clear();
    }
}
