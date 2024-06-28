using System.Collections.Generic;
/* Ce script implémente une file de priorité générique en C#. 
Les éléments sont stockés avec leur priorité associée, et la file permet 
d'ajouter des éléments avec une méthode Enqueue et de retirer l'élément 
avec la plus haute priorité (la plus petite valeur) avec une méthode Dequeue.
Cette implémentation utilise une liste interne pour stocker les éléments, 
et la recherche de l'élément avec la meilleure priorité est effectuée de manière linéaire.
Cette structure de données est utile dans divers algorithmes,
tels que les algorithmes de pathfinding comme A* ou Dijkstra, 
où il est nécessaire de traiter les éléments par ordre de priorité.
*/

public class PriorityQueue<T>
{
    private List<KeyValuePair<T, float>> elements = new List<KeyValuePair<T, float>>();

    public int Count
    {
        get { return elements.Count; }
    }

    public void Enqueue(T item, float priority)
    {
        elements.Add(new KeyValuePair<T, float>(item, priority));
    }

    public T Dequeue()
    {
        int bestIndex = 0;

        for (int i = 0; i < elements.Count; i++)
        {
            if (elements[i].Value < elements[bestIndex].Value)
            {
                bestIndex = i;
            }
        }

        T bestItem = elements[bestIndex].Key;
        elements.RemoveAt(bestIndex);
        return bestItem;
    }
}
