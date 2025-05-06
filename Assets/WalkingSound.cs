using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WalkingSound : MonoBehaviour
{
    ActionBasedContinuousMoveProvider moveProvider;
    [SerializeField] AudioSource audioSource;
    bool walking = false;
    void Start()
    {
        moveProvider = GetComponent<ActionBasedContinuousMoveProvider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (moveProvider.leftHandMoveAction.action.ReadValue<Vector2>() != Vector2.zero && !walking)
        {
            audioSource.Play();
            walking = true;
        }
        else if (moveProvider.leftHandMoveAction.action.ReadValue<Vector2>() == Vector2.zero && walking)
        {
            audioSource.Stop();
            walking = false;
        }
    }
}
