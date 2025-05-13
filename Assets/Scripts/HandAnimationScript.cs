using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimationScript : MonoBehaviour
{
    public InputActionReference horizontalGrabInputActionReference;
    public InputActionReference verticalGrabInputActionReference;

    private Animator handAnimator;
    private float horizontalValue;
    private float verticalValue;

    // Start is called before the first frame update
    void Start()
    {
        handAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimateHorizontalGrip();
        AnimateVerticalGrip();
    }

    private void AnimateHorizontalGrip()
    {
        horizontalValue = horizontalGrabInputActionReference.action.ReadValue<float>();
        handAnimator.SetFloat("HorizontalGrab", horizontalValue);
    }

    private void AnimateVerticalGrip()
    {
        verticalValue = verticalGrabInputActionReference.action.ReadValue<float>();
        handAnimator.SetFloat("VerticalGrab", verticalValue);
    }
}
