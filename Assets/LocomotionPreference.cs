using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LocomotionPreference : MonoBehaviour
{
    [SerializeField] GameObject turn;
    [SerializeField] GameObject move;
    bool isSnapTurning = true;
    public void ChangeTurnStyle()
    {
        isSnapTurning = !isSnapTurning;
        turn.GetComponent<ActionBasedSnapTurnProvider>().enabled = isSnapTurning;
        turn.GetComponent<ActionBasedContinuousTurnProvider>().enabled = !isSnapTurning;
    }

    public void ChangeMovingSpeed(float val)
    {
        move.GetComponent<ActionBasedContinuousMoveProvider>().moveSpeed = val;
    }
}
