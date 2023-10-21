using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IDamage
{
    public enum EnemyState
    {
        still,
        patrol, // Palier 0
        alert1, // Palier 1
        checkObject
    }
    
    public EnemyState state;
    public bool canGiveCard;
    private bool doOnce;
    [HideInInspector]public EnemyState baseState;
    
    [SerializeField] private float life = 100;
    [SerializeField] private GameObject bloodSheld;

    private NavMeshAgent agent;
    public Transform[] waypoints;
    private int waypointIndex;
    private Vector3 target;


    [SerializeField] private float FireRate  = 10;  // The number of bullets fired per second
     private float lastfired; 
    [SerializeField] private GameObject Bullet;
    private Animator _animator;

    [Header("Object interaction")] 
    [SerializeField] private float distObjectInteraction = 1;
     public GameObject ObjectToLookAt;

     [Header("Canvas")] 
     public TextMeshProUGUI txtDialogue;
     public TextMeshProUGUI txtStressLevel;
     [HideInInspector] public int indexStressLevel;
    
    void Start()
    {
        baseState = state;
        _animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        
        txtStressLevel.text = "" + indexStressLevel;

        ReloadDestination();
    }

    public void ReloadDestination()
    {
        if (waypoints.Length != 0)
        {
            Destination();
        }
    }
    
    void Update()
    {
        
        if (life <= 0)
        {
            Instantiate(bloodSheld, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        
        switch (state)
        {
            case EnemyState.still:
                StateStill();
                break;
            case EnemyState.patrol:
                StatePatrol();
                break;
            case EnemyState.alert1:
                StateAlert();
                break;
            case EnemyState.checkObject:
                CheckObject();
                break;
        }
        txtStressLevel.text = "" + indexStressLevel;
    }


    void StateStill()
    {
        ReloadDestination();
        _animator.enabled = true;
    }

    
    #region Patrol

    void StatePatrol()
    {
        _animator.enabled = false;
        if (Vector3.Distance(transform.position, target) < 1 && waypoints.Length != 0)
        {
            IterateWaypointIndex();
            Destination();
        }
    }
    
    void Destination()
    {
        target = waypoints[waypointIndex].position;
        agent.SetDestination(target);
    }

    void IterateWaypointIndex()
    {
        waypointIndex++;
        if (waypointIndex == waypoints.Length)
        {
            waypointIndex = 0;
        }
    }

    public void InteruptDestination()
    {
        agent.ResetPath();
    }

    #endregion
    
    
    void StateAlert()
    {
        txtDialogue.text = "EH TOI LA !";
        _animator.enabled = false;
        agent.SetDestination(PlayerController.instance.transform.position);
        Shoot();
    }

    void Shoot()
    {
        if (Time.time - lastfired > 1 / FireRate)
        {
            lastfired = Time.time;
            GameObject bullet = Instantiate(Bullet, transform.position, Quaternion.identity);
            Rigidbody rbBullet = bullet.GetComponent<Rigidbody>();
            rbBullet.AddForce((PlayerController.instance.transform.position - transform.position).normalized * 50, ForceMode.Impulse);
        }
    }
    

    void CheckObject()
    {
        Debug.Log("Oh tient une caisse est détruite");
        txtDialogue.text = "Une " + ObjectToLookAt.name + " détruites ?";
        InteruptDestination();
        if(Vector3.Distance(ObjectToLookAt.transform.position, transform.position) >= distObjectInteraction) //Distance a garder entre l'object et l'enemy
        {
            agent.SetDestination(ObjectToLookAt.transform.position); // se dirige vers la position de l'object
            transform.LookAt(ObjectToLookAt.transform); // regarde l'object
            txtDialogue.text = "??";
        }
        
        StartCoroutine(waitForCheckObject());
    }

    IEnumerator waitForCheckObject()
    {
        ObjectToLookAt.GetComponent<ObjectInteractive>().isInspected = true;
        yield return new WaitForSeconds(5);
        txtDialogue.text = "...";
        ObjectToLookAt = null;
        state = EnemyState.patrol;
        ReloadDestination();
        ObjectToLookAt = null;
    }
    
    
    public void Damage(float damage)
    {
        life -= damage;
    }

    public void GiveKey()
    {
        if (canGiveCard && !doOnce)
        {
            GameObject blueCard = Instantiate(GameManager.instance.BlueCard, transform.position, Quaternion.identity);
            blueCard.GetComponent<Cards>().enemy = GetComponent<Enemy>();
            doOnce = true;
        }
    }
}
