using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPushBack : MonoBehaviour
{
    [SerializeField] Transform CameraTransform;
    [SerializeField] GameObject pushbackObject;
    [SerializeField] LayerMask layer;
    [SerializeField] GameManager gameManager;
    CharacterController characterController;
    float pushbackStrength = 1.0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }
    bool IsColliding()
    {
        bool colliding = false;
        Collider[] colliders = Physics.OverlapSphere(CameraTransform.transform.position, 0.3f, layer);
        foreach (var col in colliders)
        {
            if (col.gameObject == pushbackObject)
                colliding = true;
        }
        return colliding;  
    }

    void Update()
    {
        if (IsColliding())
        {
            Debug.Log("Colliding");
            Vector3 moveVecDir = (new Vector3 (characterController.transform.position.x, 0.0f, characterController.transform.position.z) - new Vector3 (pushbackObject.transform.position.x, 0.0f, pushbackObject.transform.position.z)).normalized;
            gameManager.directionVelocity = Vector3.zero;
            characterController.Move(moveVecDir * pushbackStrength);
        }
    }
}