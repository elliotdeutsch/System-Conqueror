using System.Collections.Generic;
using UnityEngine;

public class StarNameGenerator : MonoBehaviour
{
    private List<string> prefixes = new List<string> { "Alpha", "Beta", "Delta", "Epsilon", "Zeta", "Vega", "Sirius", "Altair" };
    private List<string> suffixes = new List<string> { "ari", "os", "ion", "a", "or", "us", "ius", "ix" };

    public string GenerateStarName()
    {
        string prefix = prefixes[Random.Range(0, prefixes.Count)];
        string suffix = suffixes[Random.Range(0, suffixes.Count)];
        return prefix + suffix;
    }
}
