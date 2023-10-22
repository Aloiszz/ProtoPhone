using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMover : MonoBehaviour
{
    public float speed = 100;
    public TerrainCollision terrainMover;
    public float distanceToSpawn = 1000;
    public Transform pointTospawn;
    void Start()
    {
        //terrainMover = GetComponentInChildren<TerrainCollision>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!terrainMover.hasTouched)
        {
            transform.position += -Vector3.right * speed * Time.deltaTime;
        }
        else
        {
            transform.position = pointTospawn.transform.position;
        }
        
    }
}
