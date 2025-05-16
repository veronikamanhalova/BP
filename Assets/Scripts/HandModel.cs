using UnityEngine;

public class HandModel : MonoBehaviour
{
    [SerializeField] GameObject hand;

    public void ClimbGrabFixHand(Transform attachPoint)
    {
        hand.transform.SetParent(attachPoint, false);
    }

    public void ClimbGrabUnfixHand()
    {
        hand.transform.position = transform.position;
        hand.transform.rotation = transform.rotation;
        hand.transform.SetParent(transform, false);
    }
}
