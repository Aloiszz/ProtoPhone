using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxObject : ObjectInteractive
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
            case State.Destoyed:
                transform.GetComponent<Renderer>().material.SetColor ("_EmissionColor", new Color(1,0,0,1) * 10f);
                break;
        }
    }

    public void Interact()
    {
        state = State.Destoyed;
        Debug.Log("Je touche une " + transform.name + " elle est maintenant " + state);
    }

    public void IsVisible(bool isVisible)
    {
        this.isVisible = isVisible;
    }
}
