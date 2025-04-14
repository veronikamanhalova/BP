using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GameManager : MonoBehaviour
{
    [SerializeField] CharacterController characterController;
    [SerializeField] ClimbProvider climbProvider;
    [SerializeField] GameObject startPos;
    [SerializeField] GameObject camera;
    [SerializeField] DetectHeadCollision detector;
    [SerializeField] Transform blackoutZone;
    [SerializeField] GameObject topAchor;

    [NonSerialized] public bool isClimbing = false;
    [NonSerialized] public bool finishedClimb = false;
    bool failedClimb = false;
    bool passedBlackout = false;
    //bool screenIsBlack = false;
    bool startBlackout = false;
    public float gravity = -1.5f;
    Vector3 velocity = Vector3.zero;
    float groundCheckDistance = 0.5f;

    bool grounded = false;

    // Start is called before the first frame update
    void Start()
    {
        Reload();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateClimbingStateVariables();

        //if (isClimbing)
        //{
        //    UpdateCameraCollisionScreen();
        //}

        if (startBlackout)
        {
            camera.GetComponent<BlackoutFade>().Fade();
            startBlackout = false;
        }

        if (failedClimb && grounded)
        {
            Reload();
        }

        if (finishedClimb)
        {
            //todo
        }

        topAchor.GetComponent<Collider>().enabled = isClimbing ? true : false;
        ApplyGravity();
        UpdateLimitMovement();
    }

    void UpdateLimitMovement()
    {
        Vector3 newPos = characterController.transform.position;
        if (newPos.z > 86)
            newPos.z = 86;
        else if (newPos.z < -70)
            newPos.z = -70;

        if (newPos.x < -120)
            newPos.x = -120;
        else if (newPos.x > 40)
            newPos.x = 40;

        if (newPos != characterController.transform.position)
        {
            Vector3 moveVector = newPos - characterController.transform.position;
            characterController.Move(moveVector);
        }
    }

    //void UpdateCameraCollisionScreen()
    //{
    //    detector.CallDetection();
    //    if (detector.colliderHits.Count > 0)
    //    {
    //        if (!screenIsBlack)
    //        {
    //            screenIsBlack = true;
    //            camera.GetComponent<BlackoutFade>().TurnBlack();
    //        }
    //    }
    //    else
    //    {
    //        if (screenIsBlack)
    //        {
    //            screenIsBlack = false;
    //            camera.GetComponent<BlackoutFade>().TurnClear();
    //        }
    //    }
    //}

    void Reload()
    {
        Vector3 moveVector = startPos.transform.position - characterController.transform.position;

        // Move the character instantly
        characterController.Move(moveVector);
        //characterController.transform.rotation = startPos.transform.rotation;

        // Reset climb states
        isClimbing = false;
        finishedClimb = false;
        failedClimb = false;

        passedBlackout = false;
        startBlackout = false;
    }

    void UpdateClimbingStateVariables()
    {
        bool prevClimbingState = isClimbing;
        isClimbing = climbProvider.climbAnchorInteractor != null;

        if (characterController.transform.position.y > blackoutZone.transform.position.y)
            passedBlackout = true;

        if (!isClimbing && !grounded && passedBlackout && characterController.transform.position.y <= blackoutZone.transform.position.y)
        {
            startBlackout = true;
            failedClimb = true;
            passedBlackout = false;
        }
    }

    void ApplyGravity()
    {
        if (grounded || isClimbing)
        {
            velocity = Vector3.zero;
            bool willCollideWithGround = Physics.Raycast(characterController.transform.position, Vector3.down, out RaycastHit hitInfo, groundCheckDistance);

            if (!willCollideWithGround || isClimbing)
            {
                grounded = false;
            }
        }
        else if (!grounded)
        {
            velocity.y += gravity * Time.deltaTime;
            Vector3 nextPos = characterController.transform.position + (velocity * Time.deltaTime);

            bool willCollideWithGround = Physics.Raycast(nextPos, Vector3.down, out RaycastHit hitInfo, groundCheckDistance);

            if (willCollideWithGround)
            {
                // Set Player to the ground
                velocity.y = -Math.Abs(hitInfo.point.y - characterController.transform.position.y) + characterController.skinWidth;
                characterController.Move(velocity);
                velocity = Vector3.zero;
                grounded = true;
            }
            else
            {
                // Keep falling
                characterController.Move(velocity * Time.deltaTime);
            }
        }
    }

    public void finishClimb()
    {
        finishedClimb = true;

        climbProvider.FinishLocomotion();
    }
}
