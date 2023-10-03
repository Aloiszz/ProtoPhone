using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamage
{
    [SerializeField] private float life = 100;
    [SerializeField] private GameObject bloodSheld;

    public uint num;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (life <= 0)
        {
            Instantiate(bloodSheld, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public void Damage(float damage)
    {
        life -= damage;
    }


    void ex1() // calcul complement
    {
        uint a = 0b001110;
        uint b = ~a;
        
        Debug.Log(Convert.ToString(b, 2));
    }

    void ex2() // efface  0 et 1 
    {
        uint a = 0b000011111;
        uint b = 0b111111100 & a;
        
        Debug.Log(Convert.ToString(b, 2));
    }
    
    void ex3() // force 3 et 4 à 1 
    {
        uint a = 0b1110011111;
        uint b = 0b0111111100 | a;
        
        Debug.Log(Convert.ToString(b, 2));
    }
    
    void ex4() // compte combien de 1 et combien de 0
    {
        uint a = 0b1110011111;
        uint b;

        for (int i = 0; i < a; i++)
        {
            Debug.Log("haha");
            //Debug.Log(Convert.ToString(b, 2));
        }
        
        
    }
    
}
