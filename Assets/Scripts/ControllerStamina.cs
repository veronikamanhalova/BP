using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class ControllerStamina : MonoBehaviour
{
    [SerializeField] MyClimbProvider climbProvider;
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
                EndStamina();
                climbProvider.FinishClimbGrab(transform);
            }

            if (staminaVal < maxStaminaVal / 5)
            {
                GetComponent<XRDirectInteractor>().SendHapticImpulse(0.2f,1.0f);
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

    public void StartStamina()
    {
        handIsClimbing = true;
    }
    public void EndStamina()
    {
        handIsClimbing = false;
    }
}
