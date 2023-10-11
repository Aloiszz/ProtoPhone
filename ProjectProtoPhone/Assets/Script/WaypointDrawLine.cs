using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WaypointDrawLine : MonoBehaviour
{
    public enum Colors
    {
        red,
        green,
        blue,
    }
    public Colors waypointsColor;
    private Color _colors;

    [SerializeField] List<Transform> waypoints;
    void Start()
    {
        foreach (Transform i in GetComponentInChildren<Transform>())
        {
            waypoints.Add(i);
        }
    }
    
    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        switch (waypointsColor)
        {
            case Colors.red:
                _colors = Color.red;
                break;
            case Colors.green:
                _colors = Color.green;
                break;
            case Colors.blue:
                _colors = Color.blue;
                break;
        }
        
        for (int i = 0; i < waypoints.Count; i++)
        {
            Handles.color = _colors;
            if (i == waypoints.Count-1)
            {
                Handles.DrawLine(waypoints[i].transform.position, waypoints[0].transform.position, 10);
            }
            else
            {
                Handles.DrawLine(waypoints[i].transform.position, waypoints[i+1].transform.position, 10);
            }
        }
    }
    #endif
}
