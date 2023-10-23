using System;
using System.Collections;
using System.Net.Http.Headers;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using DG.Tweening;
using Unity.Collections;

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

    public Playeraction playerInput;
    private InputAction Move;
    private InputAction Touch;
    
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
        playerInput = new Playeraction();
    }

    private void Start()
    { 
        initPlayerSeed = playerSpeed;
        isCovered = true;
    }
    
    
    void OnEnable()
    {
        Move = playerInput.Player.Move;
        Move.Enable();

        Touch = playerInput.Player.Touch;
        Touch.Enable();
        Touch.performed += TouchPressed;
    }
    void OnDisable()
    {
        Move.Disable();
        Touch.Enable();
    }

    public LayerMask mask;
    public bool isClosedEnough;
    void Update()
    {
        Ray();
        Movement();
        
        //Debug
        if (Input.GetKeyDown(KeyCode.Space))
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

    private void OnDrawGizmos()
    {
        Gizmos.color=Color.red;
    }

    private void Ray()
    {
        Debug.DrawRay(transform.position, transform.forward * 2, Color.red);
        
        RaycastHit hit;
        bool detected = (Physics.Raycast(transform.position, transform.forward, out hit, 2f, mask));
        isClosedEnough = detected;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2f))
        {
            if (hit.transform.GetComponent<Enemy>() != null && isCovered && isRolling)
            {
                hit.transform.GetComponent<Enemy>().Push(pushForce, pushDuration);
            }

            if (hit.transform.GetComponent<IInteractable>() != null && isRolling)
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

        Vector2 input = Move.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0,input.y);
        controller.Move(move * Time.deltaTime * playerSpeed);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    void TouchPressed(InputAction.CallbackContext ctx) // Kill l'ennemy
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit2;
        if (Physics.Raycast(ray, out hit2, 1000, mask))
        {
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2);
            if (hit2.transform.GetComponent<Enemy>() != null && isClosedEnough)
            {
                hit2.transform.GetComponent<Enemy>().Damage(100);
            }
        }
    }
}
