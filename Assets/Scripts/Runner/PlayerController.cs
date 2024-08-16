using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace TempleRun.Player {

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float initialPlayerSpeed = 4f;
    [SerializeField]
    private float maximumPlayerSpeed = 30f;
    [SerializeField]
    private float playerSpeedIncreaseRate = .1f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float initialGravityValue = -9.81f;
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private LayerMask turnLayer;
    [SerializeField]
    private LayerMask turnFailedLayer;

    [SerializeField]
    private TakeDamageScript takeDamageScript;

    [SerializeField]
    private GameObject fadeCanvas; // Referenz auf das Canvas-GameObject

    private FadeInOut fadeInOutScript;


    private float playerSpeed;
    private float gravity;
    private Vector3 movementDirection = Vector3.forward;
    private Vector3 playerVelocity;

    private PlayerInput playerInput;
    private InputAction turnAction; 
    private InputAction leanAction;
    private InputAction jumpAction;
    private InputAction slideAction;

    private bool turningActive = true;

    private CharacterController controller;

    [SerializeField]
    private UnityEvent<Vector3> turnEvent;

    private void Awake() {
        playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();
        turnAction = playerInput.actions["Turn"];
        leanAction = playerInput.actions["Lean"];
        jumpAction = playerInput.actions["Jump"];
        slideAction = playerInput.actions["Slide"];

        // Zugriff auf das FadeInOut-Skript im Canvas-GameObject
        if (fadeCanvas != null)
            {
                fadeInOutScript = fadeCanvas.GetComponent<FadeInOut>();
            }
        }

    private void OnEnable() {
        turnAction.performed += PlayerTurn;
        //leanAction.performed += PlayerTurn;
        jumpAction.performed += PlayerJump;
    }

    private void OnDisable() {
        turnAction.performed -= PlayerTurn;
        jumpAction.performed -= PlayerJump;
    }

    private void Start() {
        gravity = initialGravityValue;
        playerSpeed = initialPlayerSpeed;
    }

    // Method that is called by pressing the left or right arrow key
    private void PlayerTurn(InputAction.CallbackContext context) {
        Debug.Log("Player Turn: " + context.ReadValue<float>());
        Vector3? turnPosition = CheckTurn(context.ReadValue<float>());
        if (!turnPosition.HasValue)
        {
            return;
        }
        Vector3 targetDirection = Quaternion.AngleAxis(90 * context.ReadValue<float>(), Vector3.up) * movementDirection;
        Debug.Log("New Target Direction: " + targetDirection);
        Debug.Log("Current Movement Direction: " + movementDirection);

        if (fadeInOutScript != null)
        {
            FadeInOut.Instance.setColor(Color.black);
            fadeInOutScript.StartCoroutine(fadeInOutScript.FadeInAndOut());
        }
        StartCoroutine(TurnPlayerWithDelay(0.5f,targetDirection, context.ReadValue<float>(), turnPosition));
    }

    public void ForcePlayerTurn(float direction) {
        Debug.Log("Forcing Player Turn in Player Controller");
        Vector3? turnPosition = CheckForcedTurn();
        if (!turnPosition.HasValue || turningActive == false)
        {
            return;
        }
        Vector3 targetDirection = Quaternion.AngleAxis(90 * direction, Vector3.up) * movementDirection;
        if(GameSyncManager.Instance) {
            GameSyncManager.Instance.RpcDecreaseRunnerHealth(20);
        }
        if (fadeInOutScript)
        {
            fadeInOutScript.setColor(Color.red);
            StartCoroutine(fadeInOutScript.FadeInAndOut());
        }
        StartCoroutine(TurnPlayerWithDelay(0.5f,targetDirection, direction, turnPosition));
     }

    private void PlayerJump(InputAction.CallbackContext context) {
        if (IsGrounded())
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * gravity * -3f);
            controller.Move(playerVelocity * Time.deltaTime);
        }
    }

    // Method that is called to turn the Player to another corridor direction
    public void PlayerLean(float direction) {
        Vector3? turnPosition = CheckTurn(direction);
        if (!turnPosition.HasValue || turningActive == false)
        {
            return;
        }
        Debug.Log("Invoke Player turn by Leaning");
        Vector3 targetDirection = Quaternion.AngleAxis(90 * direction, Vector3.up) * movementDirection;
        StartCoroutine(freezeTurning());

        if (fadeInOutScript != null)
        {
            fadeInOutScript.setColor(Color.black);
            fadeInOutScript.StartCoroutine(fadeInOutScript.FadeInAndOut());
        }
        StartCoroutine(TurnPlayerWithDelay(0.5f,targetDirection, direction, turnPosition));
    }

    private IEnumerator TurnPlayerWithDelay(float delay, Vector3 targetDirection, float direction, Vector3? turnPosition) {
        yield return new WaitForSeconds(delay);
        turnEvent.Invoke(targetDirection);
        Turn(direction, turnPosition.Value); 
    }

    private void Update() {
        controller.Move(transform.forward * playerSpeed * Time.deltaTime);

        if (IsGrounded() && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private bool IsGrounded(float length = .2f) {
        Vector3 raycastOriginFirst = transform.position;
        raycastOriginFirst.y -= controller.height / 2f;
        raycastOriginFirst.y += .1f;
        Vector3 raycastOriginSecond = raycastOriginFirst;
        raycastOriginFirst -= transform.forward * .2f;
        raycastOriginSecond += transform.forward * .2f;

        Debug.DrawLine(raycastOriginFirst, Vector3.down, Color.green, 2f);
        Debug.DrawLine(raycastOriginSecond, Vector3.down, Color.red, 2f);
        
        if (Physics.Raycast(raycastOriginFirst, Vector3.down, out RaycastHit hit, length, groundLayer)
            || Physics.Raycast(raycastOriginSecond, Vector3.down, out RaycastHit hit2, length, groundLayer)
            )
        {
            return true;
        }
        return false;
    }

    private Vector3? CheckTurn(float turnValue) {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, .1f, turnLayer);
        if (hitColliders.Length != 0)
        {
            Tile tile = hitColliders[0].transform.parent.GetComponent<Tile>();
            TileType type = tile.type;
            if ((type == TileType.LEFT && turnValue == -1) ||
                (type == TileType.RIGHT && turnValue == 1) ||
                (type == TileType.SIDEWAYS)) {
                    Debug.Log(tile.pivot.position);
                    return tile.pivot.position;
            }
        }
        return null;
    }

    private Vector3? CheckForcedTurn() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, .5f, turnFailedLayer);
        if (hitColliders.Length != 0)
        {
            Tile tile = hitColliders[0].transform.parent.GetComponent<Tile>();
            TileType type = tile.type;
            if ((type == TileType.LEFT) ||
                (type == TileType.RIGHT) ||
                (type == TileType.SIDEWAYS)) {
                    return tile.pivot.position;
            }
        }
        return null;
    }

    private void Turn(float turnValue, Vector3 turnPosition) {
        Debug.Log("Current Player Rotation: " + transform.rotation );
        Vector3 tempPlayerPosition = new Vector3(turnPosition.x, transform.position.y, turnPosition.z);
        controller.enabled = false; 
        transform.position = tempPlayerPosition;
        controller.enabled = true;
        Quaternion targetRotation = transform.rotation * Quaternion.Euler(0, 90 * turnValue, 0);
        Debug.Log("Target Rotation: " + targetRotation );
        transform.rotation = targetRotation;
        movementDirection = transform.forward.normalized;
    }

    private IEnumerator freezeTurning() {
        turningActive = false;
        yield return new WaitForSeconds(2);
        turningActive = true;
    }

}
}
