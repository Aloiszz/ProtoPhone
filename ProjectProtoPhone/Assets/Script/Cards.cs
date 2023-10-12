using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Cards : MonoBehaviour
{
     public Enemy enemy;
    [SerializeField] private Transform[] waypoint;

    [SerializeField] private WaypointDrawLine WaypointDrawLine;

    private bool isTriggered;
    void Start()
    {
        waypoint = enemy.waypoints;
        WaypointDrawLine = waypoint[0].GetComponentInParent<WaypointDrawLine>();
        
    }

    
    void Update()
    {
        if (isTriggered)
        {
            WaypointDrawLine.ShootRaycast();
        }
    }

    void DrawLineRenderer()
    {
        WaypointDrawLine.LineRenderer.positionCount = waypoint.Length;
        WaypointDrawLine.LineRenderer.positionCount ++;
        
        for (int i = 0; i < waypoint.Length; i++)
        {
            Debug.Log(i);
            if (i == waypoint.Length - 1)
            {
                WaypointDrawLine.LineRenderer.SetPosition(i, waypoint[0].transform.position);
                WaypointDrawLine.LineRenderer.SetPosition(i+1, waypoint[1].transform.position);
            }
            else
            {
                WaypointDrawLine.LineRenderer.SetPosition(i, waypoint[i+1].transform.position);
            }
        }
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            isTriggered = true;
            DrawLineRenderer();
        }
    }
}
