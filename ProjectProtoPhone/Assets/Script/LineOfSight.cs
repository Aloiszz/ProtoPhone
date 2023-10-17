using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

[RequireComponent( typeof( SphereCollider ))]


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
    
    private void Awake()
    {
        detection_collider = this.GetComponent<SphereCollider>();
        _enemy = GetComponentInParent<Enemy>();
    } 

    private void Start()
    {
        _enemyBaseState = _enemy.state;
    }

    private void OnTriggerEnter( Collider other )
    {
        if ( other.tag == "Player" )
        {
            target = other.gameObject;
            detect_player = StartCoroutine( DetectPlayer() );
            player_collider = other;
        }
    }

    private void OnTriggerExit( Collider other )
    {
        if ( other.tag == "Player" )
        {
            target = null;
            //_enemy.ReloadDestination();
            StopCoroutine( detect_player );
        }
    }

    
    IEnumerator DetectPlayer()
    {
        while ( true )
        {
            yield return new WaitForSeconds( detection_delay );
        
            points = GetBoundingPoints( player_collider.bounds );
        
            int points_hidden = 0;

            foreach ( Vector3 point in points )
            {
                Vector3 target_direction = point - this.transform.position;
                float target_distance = Vector3.Distance( this.transform.position, point );
                float target_angle = Vector3.Angle( target_direction, this.transform.forward );

                if ( IsPointCovered( target_direction, target_distance ) || target_angle > fov)
                    ++points_hidden;
            }

            if (points_hidden >= points.Length)// player is hidden
            {
                isHiden = true;
                PlayerController.instance.isCovered = true;
                sideeys.color = new Color (1, 1, 1, .2f);

                switch (_enemyBaseState)
                {
                    case Enemy.EnemyState.still:
                        _enemy.state = Enemy.EnemyState.still;
                        break;
                    case Enemy.EnemyState.patrol:
                        _enemy.state = Enemy.EnemyState.patrol;
                        _enemy.InteruptDestination();
                        _enemy.ReloadDestination();
                        break;
                } 
            }
    
            else// player is visible
            {
                if (!PlayerController.instance.isUndercover)
                {
                    isHiden = false;
                    PlayerController.instance.isCovered = false;
                    sideeys.color = new Color (1, 0, 0, .2f);
                    _enemy.state = Enemy.EnemyState.alert1; // Les enemey sont alerté 
                }
            }
        
        }
    }

    private bool IsPointCovered( Vector3 target_direction, float target_distance )
    {
        RaycastHit[] hits = Physics.RaycastAll( this.transform.position, target_direction, detection_collider.radius );
        
        foreach ( RaycastHit hit in hits )
        {
            if ( hit.transform.gameObject.layer == LayerMask.NameToLayer( "Cover" ) )
            {
                float cover_distance = Vector3.Distance( this.transform.position, hit.point );  

                if ( cover_distance < target_distance )
                    return true;
            }
            
        }
        
        return false;
    }

    
    private Vector3[] GetBoundingPoints( Bounds bounds )
    {
        Vector3[] bounding_points = 
        {
            bounds.min,
            bounds.max,
            new Vector3( bounds.min.x, bounds.min.y, bounds.max.z ),
            new Vector3( bounds.min.x, bounds.max.y, bounds.min.z ),
            new Vector3( bounds.max.x, bounds.min.y, bounds.min.z ),
            new Vector3( bounds.min.x, bounds.max.y, bounds.max.z ),
            new Vector3( bounds.max.x, bounds.min.y, bounds.max.z ),
            new Vector3( bounds.max.x, bounds.max.y, bounds.min.z )
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
        
        Gizmos.DrawLineList(points);

        if (target != null)
        {
            Gizmos.DrawLine(transform.position, target.transform.position);
        }

        if (Application.isPlaying)
        {
            Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, detection_collider.radius);
            Debug.DrawLine(transform.position, transform.position + DirectionFromAngle(transform.eulerAngles.y , fov) * detection_collider.radius, Color.yellow);
            Debug.DrawLine(transform.position, transform.position + DirectionFromAngle(transform.eulerAngles.y , -fov) * detection_collider.radius, Color.yellow);
        }
    }
    #endif

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
    
}