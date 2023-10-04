using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interacter : MonoBehaviour
{
  [SerializeField] private float playerInteractiongDistance;
  private GameObject player;

  private void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        
        CheckDistanceWPlayer();
    }

    private void CheckDistanceWPlayer()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < playerInteractiongDistance)
        {
            ShowInteractUI();
        }
    }

    private void ShowInteractUI()
    {
        
    }
}
