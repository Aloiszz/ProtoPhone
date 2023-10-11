using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Floor : MonoBehaviour
{
    [SerializeField] Material[] mat;
    public bool isOn;
    void Start()
    {
        
    }

    private void Update()
    {
        if (isOn)
        {
            foreach (var i in mat)
            { 
                i.color = new Color32(195,195,195,255);
            }
            
        }
        else
        {
            foreach (var i in mat)
            {
                i.color = new Color32(195,195,195,15);
            }
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(0);
        if (other.GetComponent<PlayerController>())
        {
            isOn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            isOn = false;
        }
    }
    
}
