using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{
    public static Vector3 parallel(Vector3 v, Vector3 n) 
    {
        return Vector3.Dot(v, n) *n;
    }

    public static Vector3 perpendicular(Vector3 v, Vector3 n)
    {
        return v - parallel(v, n);
    }
}
