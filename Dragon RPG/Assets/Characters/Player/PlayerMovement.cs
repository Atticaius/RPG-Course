using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine.AI;

[RequireComponent(typeof(ThirdPersonCharacter))]
public class PlayerMovement : MonoBehaviour
{
    // Object References
    AICharacterControl aiCharacterControl;
    ThirdPersonCharacter thirdPersonCharacter;
    CameraRaycaster cameraRaycaster;
    Camera mainCamera;
    NavMeshAgent navMeshAgent;
    GameObject walkTarget;

    // Layers
    const int walkableLayer = 8;
    const int enemyLayer = 9;
    const int raycastEndStop = -1;

    // Variables
    public String controlMode;
    Vector3 currentClickTarget;
    bool isInDirectMovement = false;

    // Delegate
    public delegate void ChangedControlMode (String text);
    public event ChangedControlMode ControlModeDelegate;

    private void Start ()
    {
        // Object references
        thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        aiCharacterControl = GetComponent<AICharacterControl>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        mainCamera = Camera.main;
        walkTarget = new GameObject("Walk Target");

        // Delegate controls
        cameraRaycaster.notifyMouseClickObservers += ProcessMouseMovement;
        CheckControlMode();
        ControlModeDelegate(controlMode);
    }

    private void LateUpdate ()
    {
        if (Input.GetKeyUp(KeyCode.G))
        {
            // Enable/disable automatic controls
            isInDirectMovement = !isInDirectMovement;
            aiCharacterControl.enabled = !aiCharacterControl.enabled;
            navMeshAgent.enabled = !navMeshAgent.enabled;
            aiCharacterControl.SetTarget(null);

            // Check control mode and send to delegates
            CheckControlMode();
            ControlModeDelegate(controlMode);
        }
    }

    private void CheckControlMode ()
    {
        // Return state of the control mode
        if (isInDirectMovement)
        {
            controlMode = "Keyboard";
        }
        else
        {
            controlMode = "Mouse";
        }
    }

    // Fixed update is called in sync with physics
    private void FixedUpdate ()
    {
        if (isInDirectMovement)
        {
            ProcessDirectMovement();
            controlMode = "Keyboard";
        }
        else
        {
            controlMode = "Mouse";
        }
    }

    void ProcessDirectMovement ()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        var camForward = Vector3.Scale(mainCamera.transform.forward, new Vector3(1, 0, 1)).normalized;
        var move = v * camForward + h * mainCamera.transform.right;

        thirdPersonCharacter.Move(move);
    }

    void ProcessMouseMovement (RaycastHit raycastHit, int layerHit)
    {
        switch (layerHit)
        {
            case walkableLayer:
                walkTarget.transform.position = raycastHit.point;
                aiCharacterControl.SetTarget(walkTarget.transform);
                break;

            case enemyLayer:
                aiCharacterControl.SetTarget(raycastHit.transform);
                break;

            default:
                Debug.Log("Don't know how to handle this");
                break;
        }
    }

}

