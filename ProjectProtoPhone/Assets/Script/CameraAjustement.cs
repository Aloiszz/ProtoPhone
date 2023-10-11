using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

public class CameraAjustement : MonoBehaviour
{

    private Camera cam;
    [SerializeField] CinemachineVirtualCamera camera;
    [SerializeField] CinemachineVirtualCamera camera2;
    [SerializeField] private Slider sliderZoomAjustement;
    [SerializeField] private Slider sliderChangeCamera;

    [SerializeField] private Image moveJoystick;

    private PlayerInput playerInput;
    private InputAction touchPosition;
    void Awake()
    {
        cam = Camera.main;
        playerInput = GetComponent<PlayerInput>();
        touchPosition = playerInput.actions["TouchPosition"];
        
        camera.Priority = 10;
        camera2.Priority = 8;
    }
    
    private void OnEnable()
    {
        touchPosition.performed += TouchedPosition;
        moveJoystick.GetComponent<OnScreenStick>().enabled = true;
    }

    private void OnDisable()
    {
        touchPosition.performed -= TouchedPosition;
        moveJoystick.GetComponent<OnScreenStick>().enabled = false;
    }

    void TouchedPosition(InputAction.CallbackContext ctx)
    {
        moveJoystick.rectTransform.position = ctx.ReadValue<Vector2>();
    }
   
    void Update()
    {
        CameraZoomAjustement();
        ChangeCamera();
        ActiveCamera();
    }

    public void CameraZoomAjustement()
    {
        camera.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = sliderZoomAjustement.value;
    }
    
    public void ChangeCamera()
    {
        if (sliderChangeCamera.value > 0.5f)
        {
            camera.Priority = 8;
            camera2.Priority = 10;
        }
        else
        {
            camera.Priority = 10;
            camera2.Priority = 8;
        }
    }
    
    public CinemachineVirtualCamera ActiveCamera()
    {
        return (CinemachineVirtualCamera)CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera;
    }
}
