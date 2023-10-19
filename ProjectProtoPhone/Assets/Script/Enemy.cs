using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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
    [HideInInspector] public GameObject ObjectToLookAt;
    
    void Start()
    {
        baseState = state;
        _animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

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
        InteruptDestination();
        if(Vector3.Distance(ObjectToLookAt.transform.position, transform.position) >= distObjectInteraction)
        {
            agent.SetDestination(ObjectToLookAt.transform.position); // se dirige vers la position de l'object
        }
        
        transform.LookAt(ObjectToLookAt.transform); // regarde l'object
        StartCoroutine(waitForCheckObject());
    }

    IEnumerator waitForCheckObject()
    {
        yield return new WaitForSeconds(5);
        ObjectToLookAt = null;
        state = EnemyState.patrol;
        ReloadDestination();
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
