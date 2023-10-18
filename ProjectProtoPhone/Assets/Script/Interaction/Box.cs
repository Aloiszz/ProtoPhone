using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour, IInteractable
{
    public enum BoxState
    {
        Normal,
        Destoyed,
        Trapped
    }
    
    public BoxState state;
    void Start()
    {
        state = BoxState.Normal;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case BoxState.Normal: 
                transform.GetComponent<Renderer>().material.SetColor ("_EmissionColor", new Color(1,1,1,1) * 10f);
                break;
            case BoxState.Destoyed:
                transform.GetComponent<Renderer>().material.SetColor ("_EmissionColor", new Color(1,0,0,1) * 10f);
                break;
        }
    }

    public void Interact()
    {
        state = BoxState.Destoyed;
        Debug.Log("Je touche une " + transform.name + " elle est maintenant " + state);
    }
}
