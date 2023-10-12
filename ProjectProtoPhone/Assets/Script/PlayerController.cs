using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    public bool isCovered;
    public bool isUndercover;
    
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    public float playerSpeed = 2.0f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;

    public PlayerInput playerInput;
    [SerializeField] private float damage = 100;

    public static PlayerController instance;
    
    public Camera cam;
    public NavMeshAgent agent;
    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        controller = gameObject.AddComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        isUndercover = false;
    }


    void Update()
    {
        Ray();
        Movement();
        /*if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
            }
        }*/
    }


    void Ray()
    {
        Debug.DrawRay(transform.position, transform.forward * 2.5f, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2.5f))
        {
            if (hit.transform.GetComponent<IDamage>() != null && isCovered)
            {
                hit.transform.GetComponent<IDamage>().Damage(damage);
            }
        }
    }
    
    
    void Movement()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0,input.y);
        controller.Move(move * Time.deltaTime * playerSpeed);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
}
