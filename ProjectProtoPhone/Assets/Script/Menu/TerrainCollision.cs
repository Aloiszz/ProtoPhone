using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCollision : MonoBehaviour
{
    public bool hasTouched;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        hasTouched = true;
    }

    private void OnTriggerExit(Collider other)
    {
        hasTouched = false;
    }
}
