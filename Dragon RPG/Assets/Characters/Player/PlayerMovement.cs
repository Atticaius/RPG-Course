using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof (ThirdPersonCharacter))]
public class PlayerMovement : MonoBehaviour
{
    // Object References
    ThirdPersonCharacter m_Character;   // A reference to the ThirdPersonCharacter on the object
    CameraRaycaster cameraRaycaster;
    Camera mainCamera;

    // Variables
    [SerializeField] float walkMoveStopRadius = 1f;
    public String controlMode;
    Vector3 currentClickTarget;
    bool isInDirectMovement = false;
    
    private void Start()
    {
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        m_Character = GetComponent<ThirdPersonCharacter>();
        mainCamera = Camera.main;
        currentClickTarget = transform.position;
    }

    private void LateUpdate ()
    {
        if (Input.GetKeyUp(KeyCode.G))
        {
            isInDirectMovement = !isInDirectMovement;
        }
    }
    // Fixed update is called in sync with physics
    private void FixedUpdate()
    {
        if (isInDirectMovement)
        {
            ProcessDirectMovement();
            controlMode = "Keyboard";
        } else
        {
            ProcessMouseMovement();
            controlMode = "Mouse";
        }
    }

    void ProcessDirectMovement ()
    {
        currentClickTarget = transform.position;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        var camForward = Vector3.Scale(mainCamera.transform.forward, new Vector3(1, 0, 1)).normalized;
        var move = v * camForward + h * mainCamera.transform.right;

        m_Character.Move(move, false, false);
    }

    void ProcessMouseMovement ()
    {
        if (Input.GetMouseButton(0))
        {
            switch (cameraRaycaster.CurrentLayerHit)
            {
                case Layer.Walkable:
                    currentClickTarget = cameraRaycaster.Hit.point;    
                    break;

                case Layer.Enemy:
                    Debug.Log("Can't move towards enemy yet");
                    break;

                default:
                    Debug.Log("Don't know how to handle this");
                    break;
            }   
        }

        if (Vector3.Distance(currentClickTarget, transform.position) >= walkMoveStopRadius)
        {
            m_Character.Move(currentClickTarget - transform.position, false, false);
        } else
        {
            m_Character.Move(Vector3.zero, false, false);
        }
    }
}

