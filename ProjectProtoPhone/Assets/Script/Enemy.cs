using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IDamage
{
    public enum EnemyState
    {
        still,
        patrol, // Palier 0
        Search,
        Suspiscious,
        alert1, // Palier 1
        checkObject
    }
    public EnemyState state;
    
    public enum EnemyStress
    {
        Normal,
        Worried,
        Panic
    }

    public EnemyStress stress;
    
    public bool canGiveCard;
    private bool doOnce;
    [HideInInspector] public EnemyState baseState;

    [SerializeField] private float life = 100;
    [SerializeField] private GameObject bloodSheld;

    private NavMeshAgent agent;
    public Transform[] waypoints;
    private int waypointIndex;
    private Vector3 target;


    [SerializeField] private float FireRate = 10; // The number of bullets fired per second
    private float lastfired;
    [SerializeField] private GameObject Bullet;
    private Animator _animator;

    [Header("Object interaction")] [SerializeField]
    private float distObjectInteraction = 1;

    public GameObject ObjectToLookAt;

    [Header("Canvas")] public TextMeshProUGUI txtDialogue;
    public TextMeshProUGUI txtStressLevel;
    [HideInInspector] public int indexStressLevel;

    [Space] public Image SuspisionSetBar;
    [SerializeField] private float fillSpeed;
    [SerializeField] private float alertDur;

    void Start()
    {
        baseState = state;
        _animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        txtStressLevel.text = "" + indexStressLevel;

        ReloadDestination();
        indexStressLevel = 0;
        Stress();
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
            GiveKey();
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
            case EnemyState.Search:
                Search();
                break;
            case EnemyState.Suspiscious:
                Suspiscious();
                break;
            
        }

        Suspiscion();
    }

    private void Suspiscion() // Gestion de la suspiscion d'une unité
    {
        if (!GetComponentInChildren<LineOfSight>().isHiden)
        {
            SuspisionSetBar.fillAmount += fillSpeed * Time.deltaTime;
        }
        else
        {
            SuspisionSetBar.fillAmount -= alertDur * Time.deltaTime;
        }

        switch (SuspisionSetBar.fillAmount)
        {
            case > .75f: // supérieur
                SuspisionSetBar.color = new Color(1, 0, 0);
                break;

            case < .75f:
                SuspisionSetBar.color = new Color(1, .64f, 0);
                break;
        }
    }
    private void Suspiscious()
    {
        txtDialogue.text = "Qui va la ??";
    }


    void StateStill()
    {
        ReloadDestination();
        _animator.enabled = true;
    }


    #region Stress

    [ContextMenu("AddStress")]
    public void AddStress()
    {
        indexStressLevel++;
        Stress();
    }
    
    [ContextMenu("ReduceStress")]
    public void ReduceStress()
    {
        indexStressLevel--;
        Stress();
    }
    
    public void Stress() // Gestion du stress de l'unité
    {
        switch (indexStressLevel)
        {
            case 0:
                //state = baseState;
                txtStressLevel.text = "" + EnemyStress.Normal;
                stress = EnemyStress.Normal;
                break;
            case 1:
                txtStressLevel.text = "" + EnemyStress.Worried;
                stress = EnemyStress.Worried;
                break;
            case 2:
                txtStressLevel.text = "" + EnemyStress.Panic;
                stress = EnemyStress.Panic;
                break;
        }
    }

    #endregion
    

    #region Patrol

    void StatePatrol()
    {
        switch (stress)
        {
            case EnemyStress.Normal:
                PatrolNormal();
                break;
            case EnemyStress.Worried:
                PatrolWorried();
                break;
            case EnemyStress.Panic:
                PatrolPanic();
                break;
        }
    }

    void PatrolNormal()
    {
        _animator.enabled = false;
        if (Vector3.Distance(transform.position, target) < 1 && waypoints.Length != 0)
        {
            IterateWaypointIndex();
            Destination();
        }
    }

    void PatrolWorried()
    {
        _animator.enabled = true;
    }

    void PatrolPanic()
    {
        _animator.enabled = true;
        if (Vector3.Distance(transform.position, target) < 1 && waypoints.Length != 0)
        {
            IterateWaypointIndex();
            Destination();
        }
    }
    
    #endregion
    void Destination() // se rend a la prochaine localisation de sa patrouille
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
            rbBullet.AddForce((PlayerController.instance.transform.position - transform.position).normalized * 50,
                ForceMode.Impulse);
        }
    }


    void Search()
    {
        txtDialogue.text = "Ou est t'il ??";
    }

    public void LetsCheckLastSeenPos(Vector3 lastPos)
    {
        //InteruptDestination();
        if ((Vector3.Distance(transform.position, lastPos) < 1)) // reload the patrol
        {
            agent.SetDestination(lastPos);
            Debug.Log("ici");
            StartCoroutine(CheckAroundUnit());
        }
    }

    IEnumerator CheckAroundUnit()
    {
        state = EnemyState.still;
        yield return new WaitForSeconds(3);
        state = EnemyState.patrol;
    }
    
    void CheckObject()
    {
        Debug.Log("Oh tient une caisse est détruite");
        txtDialogue.text = "Une " + ObjectToLookAt.name + " détruites ?";
        InteruptDestination();
        if (Vector3.Distance(ObjectToLookAt.transform.position, transform.position) >=
            distObjectInteraction) //Distance a garder entre l'object et l'enemy
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


    public void Push(float pushForce, float pushDuration)
    {
        GetComponent<NavMeshAgent>().enabled = false;
        GetComponentInChildren<LineOfSight>().StopCoroutine(GetComponentInChildren<LineOfSight>().detect_player);
        GetComponentInChildren<SphereCollider>().enabled = false;
        
        Rigidbody rb = transform.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.AddForce((transform.position - PlayerController.instance.transform.position).normalized * pushForce , ForceMode.Impulse);
        
        txtDialogue.text = "Rompiche ...";
        StartCoroutine(waitToWakeUp(pushDuration));
    }

    IEnumerator waitToWakeUp(float duration)
    {
        yield return new WaitForSeconds(duration);
        GetComponent<NavMeshAgent>().enabled = true;
        GetComponentInChildren<SphereCollider>().enabled = true;
        Destroy(GetComponent<Rigidbody>());
        ReloadDestination();
        txtDialogue.text = "Au boulot";
    }
}

