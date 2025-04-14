using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ControllerStamina : MonoBehaviour
{
    [SerializeField] MyClimbProvider climbProvider;
    public float staminaVal = 100.0f;
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
                climbProvider.FinishClimbGrab(GetComponent<XRDirectInteractor>());
            }
        }
        else
        {
            staminaVal += Time.deltaTime * 20;
            if (staminaVal > maxStaminaVal)
                staminaVal = maxStaminaVal;
        }

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
