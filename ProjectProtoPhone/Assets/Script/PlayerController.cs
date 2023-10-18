using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public bool isCovered;
    public bool isUndercover;
    
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    public float playerSpeed = 2.0f;
    private float initPlayerSeed;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;

    public PlayerInput playerInput;
    [SerializeField] private float damage = 100;

    [Header("Roll")]
    [SerializeField] private float rollForce;
    [SerializeField] private float rollDuration;
    private bool isRolling;
    
    [Header("Push")]
    public float pushForce;
    public float pushDuration;

    public TextMeshProUGUI debugTmp;

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
        initPlayerSeed = playerSpeed;
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
        
        //Debug
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerRoll();
        }
        
    }

    // Called with a btn in the UI
    public void PlayerRoll()
    {
        if (isRolling) return;
        isRolling = true;
        playerSpeed += rollForce;
        StartCoroutine(RollCd());
    }

    private IEnumerator RollCd()
    {
        yield return new WaitForSeconds(rollDuration);
        StopRolling();
    }

    private void StopRolling()
    {
        playerSpeed = initPlayerSeed;
        isRolling = false;
    }

    private void Ray()
    {
        Debug.DrawRay(transform.position, transform.forward * 2, Color.red);
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2f))
        {
            if (hit.transform.GetComponent<Enemy>() != null && isCovered)
            {
                hit.transform.GetComponent<Enemy>().GiveKey();
            }

            if (hit.transform.GetComponent<IInteractable>() != null)
            {
                hit.transform.GetComponent<IInteractable>().Interact();
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
