using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlaySoundEffectOnTrigger : MonoBehaviour
{
    [SerializeField] AudioClip clip;
    AudioSource source;
    XRDirectInteractor interactor;
    bool wasActive = false;
    bool wasSelecting = false;
    void Start()
    {
        source = GetComponent<AudioSource>();
        interactor = GetComponent<XRDirectInteractor>();
    }

    void Update()
    {
        if ((interactor.shouldActivate && !wasActive) || (interactor.hasSelection && !wasSelecting))
        {
            source.PlayOneShot(clip);
        }
        wasActive = interactor.shouldActivate;
        wasSelecting = interactor.hasSelection;
    }
}
