using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteractive : MonoBehaviour
{

    public bool isInspected;// une fois inspecter la boite n'a plus besoin d'etre checker
    public enum State
    {
        Normal,
        Destroyed,
        Trapped
    }
    public State state;
    
    
}
