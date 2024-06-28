using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    private Dictionary<Star, List<Star>> starGraph;

    public void Initialize(Dictionary<Star, List<Star>> graph)
    {
        starGraph = graph;
    }

    public List<Star> FindPath(Star start, Star goal)
    {
        var frontier = new PriorityQueue<Star>();
        frontier.Enqueue(start, 0);

        var cameFrom = new Dictionary<Star, Star>();
        var costSoFar = new Dictionary<Star, float>();
        cameFrom[start] = null;
        costSoFar[start] = 0;

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();

            if (current == goal)
            {
                break;
            }

            foreach (var next in starGraph[current])
            {
                float newCost = costSoFar[current] + Vector3.Distance(current.transform.position, next.transform.position);
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    float priority = newCost + Vector3.Distance(next.transform.position, goal.transform.position);
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }

        var path = new List<Star>();
        var temp = goal;
        while (temp != null)
        {
            path.Add(temp);
            temp = cameFrom[temp];
        }
        path.Reverse();
        return path;
    }
}
