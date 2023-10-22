using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class ExtensionMethods 
{
    /// <summary>
    /// Rounds Vector3.
    /// </summary>
    /// <param name="vector3"></param>
    /// <param name="decimalPlaces"></param>
    /// <returns></returns>
    public static Vector3 Round(this Vector3 vector3, int decimalPlaces = 2)
    {
        float multiplier = 1;
        for (int i = 0; i < decimalPlaces; i++)
        {
            multiplier *= 10f;
        }
        return new Vector3(
            Mathf.Round(vector3.x * multiplier) / multiplier,
            Mathf.Round(vector3.y * multiplier) / multiplier,
            Mathf.Round(vector3.z * multiplier) / multiplier);
    }
    
    
    
    /// <summary>
    /// Check if two vector 3 are equal
    /// </summary>
    /// <param name="vector 'a'"></param>
    /// <param name="vector 'b'"></param>
    /// <returns></returns>
    public static bool V3Equal(Vector3 a, Vector3 b){
        return Vector3.SqrMagnitude(a - b) < 0.0001;
    }
}
