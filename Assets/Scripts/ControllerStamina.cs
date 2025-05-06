using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class ControllerStamina : MonoBehaviour
{
    [SerializeField] MyClimbProvider climbProvider;
    [SerializeField] Transform interactor;
    [SerializeField] HandPhysics handModel;
    [SerializeField] Image staminaBar;
    [NonSerialized] public float staminaVal = 100.0f;
    float maxStaminaVal = 100.0f;
    bool handIsClimbing = false;

    // Update is called once per frame
    void Update()
    {
        if (handIsClimbing)
        {
            staminaVal -= Time.deltaTime * 10;
            if (staminaVal < 0)
            {
                EndHandClimbing();
            }

            if (staminaVal < maxStaminaVal / 5)
            {
                GetComponentInChildren<XRDirectInteractor>().SendHapticImpulse(0.2f,1.0f);
            }
        }
        else
        {
            staminaVal += Time.deltaTime * 20;
            if (staminaVal > maxStaminaVal)
                staminaVal = maxStaminaVal;
        }

        staminaBar.fillAmount = staminaVal / 100;
    }

    public void EndHandClimbing()
    {
        climbProvider.FinishClimbGrab(interactor);

        handModel.ClimbGrabUnfixHand();

        if (handIsClimbing)
            EndStamina();
    }

    public void StartStamina()
    {
        handIsClimbing = true;
    }
    public void EndStamina()
    {
        handIsClimbing = false;
    }

}
