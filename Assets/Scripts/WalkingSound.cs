using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WalkingSound : MonoBehaviour
{
    ActionBasedContinuousMoveProvider moveProvider;
    [SerializeField] AudioSource audioSource;
    [SerializeField] GameManager gameManager;
    bool wasWalking = false;
    void Start()
    {
        moveProvider = GetComponent<ActionBasedContinuousMoveProvider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (moveProvider.leftHandMoveAction.action.ReadValue<Vector2>() != Vector2.zero && !wasWalking && gameManager.grounded)
        {
            audioSource.Play();
            wasWalking = true;
        }
        else if ((moveProvider.leftHandMoveAction.action.ReadValue<Vector2>() == Vector2.zero && wasWalking) || !gameManager.grounded)
        {
            audioSource.Stop();
            wasWalking = false;
        }
    }
}
