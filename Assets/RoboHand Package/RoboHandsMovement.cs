using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class RoboHandsMovement : LocomotionProvider
{
    public float speed = 1.0f; // Normal movement speed

    public float jumpSpeed = 24.0f; // original Jumping force (can be overriden in GUI)

    public static bool LinearFall; // The switch for whether the player falls with acceleration or not

    private float BaseJumpSpeed; // Non-Decayed Jump speed

    public float jumpSpeedDecay; // Amount that DecayedJumpSpeed decreases every update

    public float DecayedjumpSpeed;// Variating Jump Speed, gradually decays to slow down the player

    public float gravityMultiplier; // random multiplier that makes gravity stronger.

    private bool JumpButton; // Tells whether or not the Jump Button was just pressed.

    private bool OldJumpButton; // Tells whether or not the Jump Button was pressed in the last update.

    private bool groundedPlayer; // Tells if the player charactercontroller is grounded or not.

    private bool isJumping; // Tells if the player charactercontroller is jumping or not. modified by a bunch of sections of this script.

    public bool topOfJump; // Tells is the player has reached the stall point at the top of a jump.

    private bool normalgrav = true; // should gravity be applied normally, or are we jumping/climbing?

    public static Vector3 PlayerThrow; // The velocity of the player when throwing themselves off of a wall.

    private float SubtractiveVelocity = 0; // Gradual normal gravity. decreases slowly til it reaches -9.81.

    public float gravityDecaySpeed = 0.2f; // the amount SubtractiveVelocity decreases on every update.

    private float previousyvalue; // Tells the value of the y position the player had on the previous update.

    private float currentyvalue; // Tells the value of the y position of the player currently.

    private GameObject XRRig; // The player



    private Vector3 PlayerMovement;

    public List<XRController> controllers = null;

    private CharacterController characterController = null;

    private GameObject head = null;

    public XRController controller;

    //Climbing Script stuff

    private CharacterController character;

    public static XRController ClimbingHand;

    private bool climbLock = false; // Not currently working, meant to stop smooth locomotion with the left joystick

    private Vector3 climbvel; // The player's climbing velocity, saved during climbing and used as jumping force when done climbing.

    public static LocomotionSystem locomove; // the locomotion script

    private bool wasClimbing = false; // Tells whether or not the player was climbing in the last update. Set to false by default.

    

    protected override void Awake()
    {
        characterController = GetComponent<CharacterController>();
        head = GetComponent<XRRig>().cameraGameObject;
    }


    // Start is called before the first frame update
    private void Start()
    {
        XRRig = GameObject.Find("XR Rig");
        BaseJumpSpeed = jumpSpeed;
        PositionController();
        groundedPlayer = characterController.isGrounded;

        character = GetComponent<CharacterController>();
       
        locomove = GetComponent<LocomotionSystem>();

    }

    // Update is called once per frame
    private void FixedUpdate()
    {

        groundedPlayer = characterController.isGrounded;

        if (ClimbingHand)
        {
            climbLock = true;
            locomove.enabled = false;
            wasClimbing = true;
            isJumping = false;
            Climb();

        }
        else
        {
            climbLock = false;
            locomove.enabled = true;

      
        }

        PositionController();
        CheckForInput();

        if (climbLock == false && wasClimbing == false)
        {
            jumpSpeed = BaseJumpSpeed;
            JumpGravity();
        }
        if (climbLock == false && wasClimbing == true)
        {
            jumpSpeed = -climbvel.y * 0.18f;

            JumpGravity();
        }

    }

    private void PositionController()
    {
        // Get the head in local, playspace ground
        float headHeight = Mathf.Clamp(head.transform.localPosition.y, 1, 2);
        characterController.height = headHeight;

        // Cut in half, add skin
        Vector3 newCenter = Vector3.zero;
        newCenter.y = characterController.height / 2;
        newCenter.y += characterController.skinWidth;

        //Let's move the capsule in local space aswell
        newCenter.x = head.transform.localPosition.x;
        newCenter.z = head.transform.localPosition.z;

        // apply
        characterController.center = newCenter;
    }

    private void CheckForInput()
    {
        foreach (XRController controller in controllers)
        {
            if (controller.enableInputActions)
                CheckForMovement(controller.inputDevice);
        }
    }

    private void CheckForMovement(InputDevice device)
    {
        if (climbLock == false)
        {
            if (device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 position))
                StartMove(position);
        }
    }

    private void StartMove(Vector2 position)
    {

        // Apply the touch position to the head's forward vector
        Vector3 direction = new Vector3(position.x, 0, position.y);
        Vector3 headRotation = new Vector3(0, head.transform.eulerAngles.y, 0);

        // Rotate the input direction by the horizontal head rotation
        direction = Quaternion.Euler(headRotation) * direction;

        // apply speed and move
        Vector3 movement = direction * speed;
        characterController.Move(movement * Time.deltaTime);

    }



    private void JumpGravity()
    {
        OldJumpButton = JumpButton;
        bool OldGravity = normalgrav;
        previousyvalue = currentyvalue;


        if (controller.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool Jump)) // Checks for controller's primary button and sets the public variable accordingly
        {
            JumpButton = Jump;
        }

        if (wasClimbing == false) // Decides whether or not the jump is a wall jump or a button jump
        {
            // Attempts to jump the player if a button is pressed
            if (JumpButton == true && groundedPlayer == true && OldJumpButton == false && isJumping == false) // first jump, jumpspeed hasn't decayed yet.
            {
                PlayerMovement.y = (jumpSpeed * -1.0f * Physics.gravity.y);
                isJumping = true; // during the next update, this will send us into the else ifs below
                normalgrav = false;
                DecayedjumpSpeed = jumpSpeed - jumpSpeedDecay; // sets the decayedjumpspeed variable by subtracting a decimal from jumpspeed
            }
            else if (isJumping == true) // now that DecayedJumpSpeed is set, we'll use it in this section to jump a little less this update
            {
                PlayerMovement.y = (DecayedjumpSpeed * -1.0f * Physics.gravity.y);
                DecayedjumpSpeed = DecayedjumpSpeed - jumpSpeedDecay;

                if (DecayedjumpSpeed < 0.05f && DecayedjumpSpeed > -0.05f) // Arbitrary values used here to respresent the stalling at the top of a jump, don't want the gravity checker to switch on during that stall.
                {
                    topOfJump = true;
                }
                else
                {
                    topOfJump = false;
                }
            }
            else if ( isJumping == false && groundedPlayer == false && normalgrav == true) // this is for falling off ledges. This first update won't fall much, then start gradually falling til SubtractiveVelocity is at -9.81
            {
                if (LinearFall == false)
                {
                    if (SubtractiveVelocity != -9.8f || SubtractiveVelocity < -9.8f) // checks to see if we're yet at maximum fall velocity (-9.81) if not, speeds up falling by gravitydecayspeed's value
                    {
                        SubtractiveVelocity = SubtractiveVelocity - gravityDecaySpeed;
                    }


                    PlayerMovement = new Vector3(0, SubtractiveVelocity * gravityMultiplier, 0); // uses the new SubtractiveVelocity value to make the character fall if in midair and not jumping.
                }
                else 
                {
                    PlayerMovement = new Vector3(0, Physics.gravity.y * gravityMultiplier, 0);
                }
            }
        }
        else // Attempts to Jump the Player using the climbing velocity;
        {
            normalgrav = false;
            if ( isJumping == false)
            {
                PlayerMovement.y = (jumpSpeed * -1.0f * Physics.gravity.y);
                isJumping = true;
                DecayedjumpSpeed = jumpSpeed - jumpSpeedDecay;
            }
            else if (isJumping == true)
            {
                PlayerMovement.y = (DecayedjumpSpeed * -1.0f * Physics.gravity.y);
                DecayedjumpSpeed = DecayedjumpSpeed - jumpSpeedDecay;
            }
           
        }
        
        characterController.Move(PlayerMovement * Time.deltaTime);

        currentyvalue = XRRig.transform.position.y;

        if (groundedPlayer == false && currentyvalue == previousyvalue && wasClimbing == false)
        {


            if (isJumping == false && topOfJump == false) // not jumping and not at the millisecond or two you stall at the top of a jump
            {
                jumpSpeed = 0.45f;
                normalgrav = true;
            }
            else if (isJumping == true && topOfJump == false) // Not stalling at the top of a jump (hitting the ceiling)
            {
                isJumping = false;
                jumpSpeed = 0.45f;
                normalgrav = true;
            }
            
        }
        

        groundedPlayer = characterController.isGrounded;

        //Debug.Log("isGrounded? = " + groundedPlayer);

        if (groundedPlayer == true)
        {
            isJumping = false;
            jumpSpeed = 0.45f;
            wasClimbing = false;
            normalgrav = true;
            SubtractiveVelocity = 0f;
        }

    }

    void Climb()
    {
        InputDevices.GetDeviceAtXRNode(ClimbingHand.controllerNode).TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 velocity);
        climbvel = velocity;
        character.Move(transform.rotation * -velocity * Time.deltaTime);
    }

    public void ToggleLinearFall()
    { 
    
    }

}
