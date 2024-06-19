using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private Star origin;
    private Star destination;
    private int unitCount;

    public void Initialize(Star origin, Star destination, int unitCount)
    {
        this.origin = origin;
        this.destination = destination;
        this.unitCount = unitCount;
    }
}
