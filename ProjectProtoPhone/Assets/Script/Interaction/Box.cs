using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour, IInteractable
{
    public bool isVisible;
    public ObjectInteractive.State state;
    
    void Start()
    {
        state = ObjectInteractive.State.Normal;
    }
    void Update()
    {
        switch (state)
        {
            case ObjectInteractive.State.Normal: 
                transform.GetComponent<Renderer>().material.SetColor ("_EmissionColor", new Color(1,1,1,1) * 10f);
                break;
            case ObjectInteractive.State.Destroyed:
                transform.GetComponent<Renderer>().material.SetColor ("_EmissionColor", new Color(1,0,0,1) * 10f);
                break;
        }
    }

    public void Interact()
    {
        state = ObjectInteractive.State.Destroyed;
        Debug.Log("Je touche une " + transform.name + " elle est maintenant " + state);
    }

    public void IsVisible(bool isVisible)
    {
        this.isVisible = isVisible;
    }
}
