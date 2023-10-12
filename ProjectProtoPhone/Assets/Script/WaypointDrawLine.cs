using System;
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
    [HideInInspector] public LineRenderer LineRenderer;
    void Start()
    {
        LineRenderer = GetComponent<LineRenderer>();
        foreach (Transform i in GetComponentInChildren<Transform>())
        {
            waypoints.Add(i);
        }
    }

    private void Update()
    {
    }

    public void ShootRaycast()
    {
        for (int i = 0; i < waypoints.Count; i++)
        {
            if (i == waypoints.Count-1)
            {
                RaycastHit hit;
                if (Physics.Linecast(waypoints[i].transform.position, waypoints[0].transform.position, out hit))
                {
                    hit.transform.GetComponent<PlayerController>().isUndercover = false;
                    Debug.DrawLine(waypoints[i].transform.position, waypoints[0].transform.position, Color.red, 10);
                    if (hit.transform.GetComponent<PlayerController>())
                    {
                        hit.transform.GetComponent<PlayerController>().isUndercover = true;
                    }
                }
            }
            else
            {
                RaycastHit hit;
                if (Physics.Linecast(waypoints[i].transform.position, waypoints[i+1].transform.position, out hit))
                {
                    hit.transform.GetComponent<PlayerController>().isUndercover = false;
                    Debug.DrawLine(waypoints[i].transform.position, waypoints[i+1].transform.position, Color.red, 10);
                    if (hit.transform.GetComponent<PlayerController>())
                    {
                        hit.transform.GetComponent<PlayerController>().isUndercover = true;
                    }
                }
            }
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
