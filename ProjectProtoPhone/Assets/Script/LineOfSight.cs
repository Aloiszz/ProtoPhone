using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class LineOfSight : MonoBehaviour
{
    private Enemy _enemy;
    private Enemy.EnemyState _enemyBaseState;

    [SerializeField] private GameObject target;
    [SerializeField] private float detection_delay = 0.5f;

    private Collider player_collider;
    private SphereCollider detection_collider;
    private Bounds player_bounds;
    private Coroutine detect_player;
    public bool isHiden;
    public SpriteRenderer sideeys;

    public float fov = 70;
    private Vector3[] points;

    private bool canCheckLastPos;
    Vector3 value;
    
    [Header("Object interaction")] public List<GameObject> boxTarget;
    public List<Collider> boxCollider;
    public PointList ListOfPointLists = new PointList();

    private void Awake()
    {
        detection_collider = this.GetComponent<SphereCollider>();
        _enemy = GetComponentInParent<Enemy>();
    }

    private void Start()
    {
        _enemyBaseState = _enemy.state;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            target = other.gameObject;
            detect_player = StartCoroutine(DetectPlayer());
            player_collider = other;
        }

        if (other.tag == "Box")
        {
            boxTarget.Add(other.gameObject);
            boxCollider.Add(other);
            StartCoroutine(DetectBox());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            target = null;
            //_enemy.ReloadDestination();
            StopCoroutine(detect_player);
            isHiden = true;
        }

        if (other.tag == "Box")
        {
            //ListOfPointLists.list.RemoveAt(0);
            foreach (var collider in boxCollider)
            {
                collider.GetComponent<IInteractable>().IsVisible(false);
                if (ListOfPointLists.list[boxCollider.IndexOf(collider)].name == other.name)
                {
                    ListOfPointLists.list.RemoveAt(boxCollider.IndexOf(collider));
                }
            }

            boxTarget.Remove(other.gameObject);
            boxCollider.Remove(other);
            StopCoroutine(DetectBox());
        }
    }

    IEnumerator DetectPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(detection_delay);

            points = GetBoundingPoints(player_collider.bounds);

            int points_hidden = 0;

            foreach (Vector3 point in points)
            {
                Vector3 target_direction = point - this.transform.position;
                float target_distance = Vector3.Distance(this.transform.position, point);
                float target_angle = Vector3.Angle(target_direction, this.transform.forward);

                if (IsPointCovered(target_direction, target_distance) || target_angle > fov)
                    ++points_hidden;
            }

            if (points_hidden >= points.Length) // player is hidden
            {
                isHiden = true;
                PlayerController.instance.isCovered = true;
                //sideeys.color = new Color(1, 1, 1, .2f);

                switch (_enemyBaseState)
                {
                    case Enemy.EnemyState.still:
                        if (_enemy.state != Enemy.EnemyState.checkObject)
                        {
                            _enemy.state = Enemy.EnemyState.still;
                        }

                        break;
                    
                    case Enemy.EnemyState.patrol:
                        /*if (_enemy.state != Enemy.EnemyState.checkObject)
                        {
                            _enemy.state = Enemy.EnemyState.patrol;
                            _enemy.InteruptDestination();
                            _enemy.ReloadDestination();
                        }*/
                        if (canCheckLastPos)
                        {
                            canCheckLastPos = false;
                            _enemy.state = Enemy.EnemyState.Search;
                            value = points[0];
                        }
                        _enemy.LetsCheckLastSeenPos(value);
                        break;
                    
                    case Enemy.EnemyState.alert1:
                        break;
                    
                    case Enemy.EnemyState.checkObject:
                        break;
                }
            }
            else // player is visible
            {
                if (!PlayerController.instance.isUndercover)
                {
                    isHiden = false;
                    PlayerController.instance.isCovered = false;
                    sideeys.color = new Color(1, .5f, 0, .2f);
                    _enemy.state = Enemy.EnemyState.Suspiscious;

                    if (_enemy.SuspisionSetBar.fillAmount == 1)
                    {
                        sideeys.color = new Color(1, 0, 0, .2f);
                        _enemy.state = Enemy.EnemyState.alert1; // Les enemey sont alerté 
                        canCheckLastPos = true;
                        _enemy.AddStress(1);
                    }
                }
            }
        }
    }


    IEnumerator DetectBox()
    {
        while (true)
        {
            yield return new WaitForSeconds(detection_delay);

            foreach (var collider in boxCollider)
            {
                if (ListOfPointLists.list.Count <= boxCollider.Count)
                {
                    ListOfPointLists.list.Add(new Point());
                }

                ListOfPointLists.list[boxCollider.IndexOf(collider)].name = collider.name;
                ListOfPointLists.list[boxCollider.IndexOf(collider)].boxPoints = GetBoundingPoints(collider.bounds);

                foreach (Vector3 point in ListOfPointLists.list[boxCollider.IndexOf(collider)].boxPoints)
                {
                    Vector3 target_direction = point - this.transform.position;
                    float target_distance = Vector3.Distance(this.transform.position, point);
                    float target_angle = Vector3.Angle(target_direction, this.transform.forward);

                    if (IsPointCovered(target_direction, target_distance) || target_angle > fov)
                        ++ListOfPointLists.list[boxCollider.IndexOf(collider)].pointHiden;
                }

                if (ListOfPointLists.list[boxCollider.IndexOf(collider)].pointHiden >=
                    ListOfPointLists.list[boxCollider.IndexOf(collider)].boxPoints.Length)
                {
                    ListOfPointLists.list[boxCollider.IndexOf(collider)].isObjectHidden = true;
                    foreach (var i in boxCollider) // Set this object not visible to the enemy
                    {
                        //Debug.Log("Je ne vois pas la boite " + i.transform.name);
                        i.GetComponent<IInteractable>().IsVisible(false);
                    }
                }
                else
                {
                    ListOfPointLists.list[boxCollider.IndexOf(collider)].isObjectHidden = false;
                    foreach (var i in boxCollider) // Set this object visible by the enemy
                    {
                        //Debug.Log("Je vois la boite " + i.transform.name);
                        i.GetComponent<IInteractable>().IsVisible(true);
                    }

                    foreach (var i in boxTarget) // State Machine of the ObjectInteraction
                    {
                        switch (i.GetComponent<ObjectInteractive>().state)
                        {
                            case ObjectInteractive.State.Normal:
                                break;

                            case ObjectInteractive.State.Destroyed:
                                sideeys.color = new Color(1, .5f, 0, .2f);
                                _enemy.AddStress(1);
                                
                                if (!i.GetComponent<ObjectInteractive>()
                                        .isInspected) // si l'object n'est pas encore inspecté
                                {
                                    if (!PlayerController.instance.isCovered)
                                    {
                                        _enemy.state = Enemy.EnemyState.alert1;
                                    }
                                    else
                                    {
                                        _enemy.state = Enemy.EnemyState.checkObject;
                                        _enemy.ObjectToLookAt = i;
                                    }
                                }

                                break;

                            case ObjectInteractive.State.Trapped:
                                break;
                        }
                    }
                }
            }
        }
    }

    private bool IsPointCovered(Vector3 target_direction, float target_distance)
    {
        RaycastHit[] hits = Physics.RaycastAll(this.transform.position, target_direction, detection_collider.radius);

        foreach (RaycastHit hit in hits)
        {
            Debug.DrawRay(transform.position, target_direction, Color.red);
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Cover"))
            {
                float cover_distance = Vector3.Distance(this.transform.position, hit.point);

                if (cover_distance < target_distance)
                    return true;
            }
        }

        return false;
    }

    private Vector3[] GetBoundingPoints(Bounds bounds)
    {
        Vector3[] bounding_points =
        {
            bounds.min,
            bounds.max,
            new Vector3(bounds.min.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.min.z),
            new Vector3(bounds.max.x, bounds.min.y, bounds.min.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.max.z),
            new Vector3(bounds.max.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.max.x, bounds.max.y, bounds.min.z)
        };

        return bounding_points;
    }


#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (isHiden)
        {
            Gizmos.color = Color.blue;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        if (target != null)
        {
            Gizmos.DrawLine(transform.position, target.transform.position);
        }

        Gizmos.DrawLineList(points);


        foreach (var collider in boxCollider)
        {
            Gizmos.DrawLineList(ListOfPointLists.list[boxCollider.IndexOf(collider)].boxPoints);
        }

        if (boxTarget.Count != null)
        {
            foreach (var boxCollider in boxTarget)
            {
                if (ListOfPointLists.list[boxTarget.IndexOf(boxCollider)].isObjectHidden)
                    Gizmos.color = Color.blue;
                else Gizmos.color = Color.red;

                Gizmos.DrawLine(transform.position, boxCollider.transform.position);
            }
        }


        if (Application.isPlaying)
        {
            Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, detection_collider.radius);
            Debug.DrawLine(transform.position,
                transform.position + DirectionFromAngle(transform.eulerAngles.y, fov) * detection_collider.radius,
                Color.yellow);
            Debug.DrawLine(transform.position,
                transform.position + DirectionFromAngle(transform.eulerAngles.y, -fov) * detection_collider.radius,
                Color.yellow);
        }
    }
#endif

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees) // Calculate the direction from angles
    {
        angleInDegrees += eulerY;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}

[System.Serializable]
public class Point
{
    public string name;
    public Vector3[] boxPoints;
    public int pointHiden;
    public bool isObjectHidden;
}

[System.Serializable]
public class PointList
{
    public List<Point> list;
}