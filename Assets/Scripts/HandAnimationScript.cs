using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimationScript : MonoBehaviour
{
    public InputActionReference gripInputActionReference;

    private Animator _handAnimator;
    private float _gripValue;

    // Start is called before the first frame update
    void Start()
    {
        _handAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimateGrip();
    }

    private void AnimateGrip()
    {
        _gripValue = gripInputActionReference.action.ReadValue<float>();
        _handAnimator.SetFloat("Grip", _gripValue);
    }
}
