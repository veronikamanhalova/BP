using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LocomotionPreference : MonoBehaviour
{
    [SerializeField] ActionBasedSnapTurnProvider snapTurn;
    [SerializeField] ActionBasedContinuousTurnProvider continuousTurn;
    [SerializeField] GameObject move;
    bool isSnapTurning = true;
    public void ChangeTurnStyle()
    {
        isSnapTurning = !isSnapTurning;

        if (isSnapTurning)
        {
            continuousTurn.enabled = false;
            snapTurn.enabled = true;
        }
        else
        {
            snapTurn.enabled = false;
            continuousTurn.enabled = true;
        }
    }

    public void ChangeMovingSpeed(float val)
    {
        move.GetComponent<ActionBasedContinuousMoveProvider>().moveSpeed = val;
    }
}
