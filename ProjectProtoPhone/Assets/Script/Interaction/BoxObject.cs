using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxObject : ObjectInteractive, IInteractable
{
    public bool isVisible;
    void Start()
    {
        state = State.Normal;
    }
    void Update()
    {
        switch (state)
        {
            case State.Normal: 
                transform.GetComponent<Renderer>().material.SetColor ("_EmissionColor", new Color(1,1,1,1) * 10f);
                break;
            case State.Destroyed:
                transform.GetComponent<Renderer>().material.SetColor ("_EmissionColor", new Color(1,0,0,1) * 10f);
                break;
            case State.Trapped:
                transform.GetComponent<Renderer>().material.SetColor ("_EmissionColor", new Color(0,0,1,1) * 10f);
                break;
        }
    }

    public void Interact()
    {
        state = State.Destroyed;
        Debug.Log("Je touche une " + transform.name + " elle est maintenant " + state);
    }

    public void IsVisible(bool isVisible)
    {
        this.isVisible = isVisible;
    }
}
