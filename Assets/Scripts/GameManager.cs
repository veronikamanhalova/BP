using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
public enum Difficulty
{
    Easy,
    Medium,
    Hard
}
public class GameManager : MonoBehaviour
{
    [SerializeField] GameMenuManager gameMenuManager;
    [SerializeField] WallGenerator wallGenerator;
    [SerializeField] LeaderboardManager leaderboardManager;
    [SerializeField] CharacterController characterController;
    [SerializeField] GameObject leftControllerStamina;
    [SerializeField] GameObject rightControllerStamina;
    [SerializeField] GameObject leftInteractor;
    [SerializeField] GameObject rightInteractor;
    [SerializeField] MyClimbProvider climbProvider;
    [SerializeField] GameObject startPos;
    [SerializeField] GameObject camera;
    [SerializeField] Transform blackoutZone;
    [SerializeField] GameObject topAchor;
    [SerializeField] GameObject climbWallBase;
    [SerializeField] LayerMask rayCastCharacter;

    [NonSerialized] public bool isClimbing = false;
    [NonSerialized] public bool finishedClimb = false;
    [NonSerialized] public bool grounded = false;

    public TextMeshProUGUI timer;
    float timerCntr = 0.0f;
    float minutes = 0.0f;
    float seconds = 0.0f;

    //[NonSerialized] public bool startedClimb = false;
    bool failedClimb = false;

    bool passedBlackoutZone = false;
    bool startScreenBlackout = false;

    Vector3 gravityVelocity = Vector3.zero;
    float groundCheckDistance = 0.2f;
    float gravity = -1.5f;
    public float maxJumpPower = 3f;
    float jumpPower = 0.0f;
    List<Vector3> characterLastPos = new List<Vector3>();
    int velocityAvrgCnt = 5;
    Vector3 characterVelocity;
    Vector3 directionVelocity = Vector3.zero;
    bool jumping = false;

    [NonSerialized] public bool proClimbingMode = false;
    Difficulty difficulty = Difficulty.Easy;

    int cntr = 0;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < velocityAvrgCnt; i++)
        {
            characterLastPos.Add(characterController.transform.position);
        }
        characterVelocity = characterController.velocity;
        leaderboardManager.LoadLeaderboard();
        Reload();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateCharacterVelocity();
        UpdateClimbingStateVariables();

        if (isClimbing && proClimbingMode)
        {
            TimerIncrease();
        }

        if (startScreenBlackout)
        {
            leftInteractor.SetActive(false);
            rightInteractor.SetActive(false);
            camera.GetComponent<BlackoutFade>().Fade();
            startScreenBlackout = false;
        }

        if (failedClimb && grounded)
        {
            Reload();
        }
        else if (grounded)
        {
            timerCntr = 0.0f;
        }

        if (finishedClimb)
        {
            BoardEntry boardEntry = new BoardEntry("Anon" + cntr, timerCntr);
            cntr++;
            leaderboardManager.leaderboard.AddLeaderboardEntry(boardEntry);
            leaderboardManager.ShowLeaderboard();
        }

        topAchor.GetComponent<Collider>().enabled = isClimbing ? true : false;
        ApplyGravity();
        UpdateLimitMovement();
    }

    void CalculateCharacterVelocity()
    {
        characterLastPos.Add(characterController.transform.position);

        Vector3 addedVelocities = Vector3.zero;
        for (int i = 0; i < velocityAvrgCnt; i++)
        {
            addedVelocities += characterLastPos[i + 1] - characterLastPos[i];
        }
        addedVelocities /= velocityAvrgCnt;
        characterVelocity = addedVelocities;

        characterLastPos.RemoveAt(0);
    }

    void TimerIncrease()
    {
        timerCntr += Time.deltaTime;
        minutes = Mathf.FloorToInt(timerCntr / 60.0f);
        seconds = Mathf.FloorToInt(timerCntr - minutes * 60.0f);
        timer.text = string.Format("{0:00}:{1:00}",minutes,seconds);
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

    void Reload()
    {
        // Move the character to the startPos
        characterController.Move(startPos.transform.position - characterController.transform.position);
        //characterController.transform.rotation = startPos.transform.rotation;

        // Reset climb states
        isClimbing = false;
        finishedClimb = false;
        failedClimb = false;

        passedBlackoutZone = false;
        startScreenBlackout = false;

        timerCntr = 0.0f;
        foreach (ControllerStamina stamina in characterController.GetComponentsInChildren<ControllerStamina>())
            stamina.staminaVal = 100.0f;

        leftInteractor.SetActive(true);
        rightInteractor.SetActive(true);
    }

    void UpdateClimbingStateVariables()
    {
        bool prevClimbingState = isClimbing;
        isClimbing = climbProvider.climbAnchorInteractor != null;

        if (characterController.transform.position.y > blackoutZone.transform.position.y)
            passedBlackoutZone = true;

        if (!isClimbing && !grounded && passedBlackoutZone && characterController.transform.position.y <= blackoutZone.transform.position.y)
        {
            startScreenBlackout = true;
            failedClimb = true;
            passedBlackoutZone = false;
        }
    }

    void ApplyGravity()
    {
        if (jumping)
        {
            characterController.Move(directionVelocity * jumpPower);
            jumpPower += gravity * 3 * Time.deltaTime;
            if (jumpPower < 0.0f)
            {
                jumpPower = 0.0f;
                jumping = false;
                gravityVelocity = Vector3.zero;
                grounded = false;
            }
        }
        else if (isClimbing)
        {
            grounded = false;
        }
        else if (grounded)
        {
            Vector3 characterPos = characterController.transform.position;
            bool willCollideWithGround = Physics.Raycast(characterPos, Vector3.down, out RaycastHit hitInfo, groundCheckDistance);

            if (!willCollideWithGround)
            {
                grounded = false;
                directionVelocity = new Vector3(characterVelocity.x / 3, 0, characterVelocity.z / 3);
                gravityVelocity = Vector3.zero;
            }
            gravityVelocity = Vector3.zero;
        }
        else if (!grounded)
        {
            gravityVelocity.y += gravity * Time.deltaTime;
            Vector3 nextPos = characterController.transform.position;
            bool willCollideWithGround = Physics.Raycast(nextPos, Vector3.down, out RaycastHit hitInfo, groundCheckDistance);

            // Raycast nefunguje kdyz se origin nachazi v stejne colideru, dame hrace o kuse vejs a strelime raycast
            nextPos.y -= gravity * Time.deltaTime;
            bool isInsideCollider = Physics.Raycast(nextPos, Vector3.down, out RaycastHit hitInfo2, groundCheckDistance);

            // Colliding Base
            bool isCollidingBase = false;
            Collider[] colliders = Physics.OverlapSphere(characterController.transform.position, 0.3f);
            foreach (var col in colliders)
            {
                if (col.gameObject == climbWallBase)
                    isCollidingBase = true;
            }

            if (willCollideWithGround)
            {
                // Will collide with ground
                // Set Player to the ground
                float moveDown = -Math.Abs(hitInfo.point.y - characterController.transform.position.y) + characterController.skinWidth;
                characterController.Move(new Vector3(0.0f, moveDown, 0.0f));

                gravityVelocity = Vector3.zero;
                grounded = true;
            }
            else if (isInsideCollider)
            {
                // Collider is inside characterController
                characterController.Move(nextPos - characterController.transform.position);

                gravityVelocity = Vector3.zero;
                grounded = true;
            }
            else if (isCollidingBase)
            {
                // Colliding with base
                gravityVelocity = Vector3.zero;
                grounded = true;
            }
            else
            {
                // Falling
                Vector3 sidewaysVelocity = new Vector3(directionVelocity.x / 2f, 0, directionVelocity.z / 2f);

                characterController.Move(sidewaysVelocity + gravityVelocity * Time.deltaTime);
            }
        }
    }

    public void finishClimb()
    {
        finishedClimb = true;

        climbProvider.FinishLocomotion();
    }

    public void newGame()
    {
        wallGenerator.GenerateWall(difficulty);
        gameMenuManager.ToggleMenuVisibility();
        Reload();
    }

    public void exitGame()
    {
        leaderboardManager.SaveLeaderboard();
        Application.Quit();
    }

    public void ChangeCLimbingStyle()
    {
        proClimbingMode = !proClimbingMode;

        leftControllerStamina.SetActive(proClimbingMode);
        rightControllerStamina.SetActive(proClimbingMode);
    }

    public void ChangeDifficulty(int diff)
    {
       difficulty = (Difficulty)diff;
    }

    float CalculateLengthOfVelocityVector(Vector3 vec)
    {
        return (float) Math.Sqrt(Math.Pow(vec.x, 2) + Math.Pow(vec.y, 2) + Math.Pow(vec.z, 2));
    }
    public void Jump()
    {
        directionVelocity = new Vector3(characterVelocity.x / 3f, characterVelocity.y * 3f, characterVelocity.z / 3f);
        jumpPower = Mathf.Clamp(CalculateLengthOfVelocityVector(directionVelocity) * maxJumpPower, 0, maxJumpPower);
        jumping = true;
    }
}
