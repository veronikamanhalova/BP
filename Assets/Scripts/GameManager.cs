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
    [SerializeField] MyClimbProvider climbProvider;
    [SerializeField] GameObject startPos;
    [SerializeField] GameObject camera;
    [SerializeField] DetectHeadCollision detector;
    [SerializeField] Transform blackoutZone;
    [SerializeField] GameObject topAchor;

    [NonSerialized] public bool isClimbing = false;
    [NonSerialized] public bool finishedClimb = false;

    public TextMeshProUGUI timer;
    float timerCntr = 0.0f;
    float minutes = 0.0f;
    float seconds = 0.0f;

    public bool startedClimb = false;
    bool failedClimb = false;
    bool passedBlackoutZone = false;
    //bool screenIsBlack = false;
    bool startScreenBlackout = false;
    Vector3 velocity = Vector3.zero;
    float groundCheckDistance = 0.5f;
    bool grounded = false;
    float gravity = -1.5f;

    [NonSerialized] public bool proClimbingMode = false;
    Difficulty difficulty = Difficulty.Easy;

    int cntr = 0;

    // Start is called before the first frame update
    void Start()
    {
        leaderboardManager.LoadLeaderboard();
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

        if (startedClimb && proClimbingMode) 
        {
            TimerIncrease();
        }

        if (startScreenBlackout)
        {
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
        startedClimb = false;

        passedBlackoutZone = false;
        startScreenBlackout = false;

        timerCntr = 0.0f;
        foreach (ControllerStamina stamina in characterController.GetComponentsInChildren<ControllerStamina>())
            stamina.staminaVal = 100.0f;
    }

    void UpdateClimbingStateVariables()
    {
        bool prevClimbingState = isClimbing;
        isClimbing = climbProvider.climbAnchorInteractor != null;

        if (characterController.transform.position.y > blackoutZone.transform.position.y)
            passedBlackoutZone = true;

        if (!prevClimbingState && isClimbing)
        {
            startedClimb = true;
        }

        if (!isClimbing && !grounded && passedBlackoutZone && characterController.transform.position.y <= blackoutZone.transform.position.y)
        {
            startScreenBlackout = true;
            failedClimb = true;
            passedBlackoutZone = false;
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
}
