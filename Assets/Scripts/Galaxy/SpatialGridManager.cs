using System.Collections.Generic;
using UnityEngine;

public class SpatialGridManager : MonoBehaviour
{
    public static SpatialGridManager Instance;

    [Header("Grid Settings")]
    public float cellSize = 10f;
    public Vector2 gridOrigin = Vector2.zero;

    // Pour chaque joueur, une grille de cellules contenant des étoiles
    private Dictionary<Player, Dictionary<Vector2Int, List<Star>>> playerGrids = new Dictionary<Player, Dictionary<Vector2Int, List<Star>>>();

    void Awake()
    {
        Instance = this;
    }

    // Convertit une position monde en coordonnées de cellule
    private Vector2Int WorldToCell(Vector3 position)
    {
        int x = Mathf.FloorToInt((position.x - gridOrigin.x) / cellSize);
        int y = Mathf.FloorToInt((position.y - gridOrigin.y) / cellSize);
        return new Vector2Int(x, y);
    }

    // Ajoute une étoile à la grille du joueur
    public void AddStar(Player player, Star star)
    {
        if (!playerGrids.ContainsKey(player))
            playerGrids[player] = new Dictionary<Vector2Int, List<Star>>();
        var grid = playerGrids[player];
        Vector2Int cell = WorldToCell(star.transform.position);
        if (!grid.ContainsKey(cell))
            grid[cell] = new List<Star>();
        if (!grid[cell].Contains(star))
            grid[cell].Add(star);
    }

    // Retire une étoile de la grille du joueur
    public void RemoveStar(Player player, Star star)
    {
        if (!playerGrids.ContainsKey(player)) return;
        var grid = playerGrids[player];
        Vector2Int cell = WorldToCell(star.transform.position);
        if (grid.ContainsKey(cell))
            grid[cell].Remove(star);
    }

    // Met à jour la position d'une étoile (si elle bouge, rare)
    public void UpdateStar(Player player, Star star)
    {
        RemoveStar(player, star);
        AddStar(player, star);
    }

    // Récupère toutes les étoiles du joueur dans un rayon donné autour d'une position
    public List<Star> GetStarsInRadius(Player player, Vector3 position, float radius)
    {
        List<Star> result = new List<Star>();
        if (!playerGrids.ContainsKey(player)) return result;
        var grid = playerGrids[player];
        Vector2Int centerCell = WorldToCell(position);
        int cellRadius = Mathf.CeilToInt(radius / cellSize);
        for (int dx = -cellRadius; dx <= cellRadius; dx++)
        {
            for (int dy = -cellRadius; dy <= cellRadius; dy++)
            {
                Vector2Int cell = new Vector2Int(centerCell.x + dx, centerCell.y + dy);
                if (grid.ContainsKey(cell))
                {
                    foreach (var star in grid[cell])
                    {
                        if (Vector3.Distance(star.transform.position, position) <= radius)
                            result.Add(star);
                    }
                }
            }
        }
        return result;
    }
}