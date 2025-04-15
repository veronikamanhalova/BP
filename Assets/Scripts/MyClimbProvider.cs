using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Locomotion provider that allows the user to climb a <see cref="ClimbInteractable"/> by selecting it.
    /// Climb locomotion moves the XR Origin counter to movement of the last selecting interactor, with optional
    /// movement constraints along each axis of the interactable.
    /// </summary>
    /// <seealso cref="ClimbInteractable"/>
    [AddComponentMenu("XR/Locomotion/Climb Provider", 11)]
    public class MyClimbProvider : LocomotionProvider
    {
        // These are parallel lists, where each interactor and its grabbed interactable share the same index in each list.
        // The last item in each list represents the most recent selection, which is the only one that actually drives movement.
        readonly List<Transform> m_GrabbingInteractors = new List<Transform>();
        readonly List<MyClimbInteractable> m_GrabbedClimbables = new List<MyClimbInteractable>();

        List<Vector3> m_InteractorAnchorWorldPosition = new List<Vector3>();
        List<Vector3> m_InteractorAnchorClimbSpacePosition = new List<Vector3>();

        [SerializeField] CharacterController characterController;
        //[SerializeField] float climbForce = 10f;
        //[SerializeField] float climbDrag = 5f;

        [SerializeField]
        [Tooltip("Climb locomotion settings. Can be overridden by the Climb Interactable used for locomotion.")]
        ClimbSettingsDatumProperty m_ClimbSettings = new ClimbSettingsDatumProperty(new ClimbSettings());

        /// <summary>
        /// Climb locomotion settings. Can be overridden by the <see cref="ClimbInteractable"/> used for locomotion.
        /// </summary>
        public ClimbSettingsDatumProperty climbSettings
        {
            get => m_ClimbSettings;
            set => m_ClimbSettings = value;
        }

        /// <summary>
        /// The interactable that is currently grabbed and driving movement. This will be <see langword="null"/> if
        /// there is no active climb.
        /// </summary>
        public MyClimbInteractable climbAnchorInteractable
        {
            get
            {
                if (m_GrabbedClimbables.Count > 0)
                    return m_GrabbedClimbables[m_GrabbedClimbables.Count - 1];
                return null;
            }
        }

        /// <summary>
        /// The interactor that is currently grabbing and driving movement. This will be <see langword="null"/> if
        /// there is no active climb.
        /// </summary>
        public Transform climbAnchorInteractor
        {
            get
            {
                if (m_GrabbingInteractors.Count > 0)
                    return m_GrabbingInteractors[m_GrabbingInteractors.Count - 1];
                return null;
            }
        }

        /// <summary>
        /// Calls the methods in its invocation list when the provider updates <see cref="climbAnchorInteractable"/>
        /// and <see cref="climbAnchorInteractor"/>. This can be invoked from either <see cref="StartClimbGrab"/> or
        /// <see cref="FinishClimbGrab"/>. This is not invoked when climb locomotion ends.
        /// </summary>
        public event Action<ClimbProvider> climbAnchorUpdated;

        /// <inheritdoc />
        protected override void Awake()
        {
            base.Awake();
            if (m_ClimbSettings == null || m_ClimbSettings.Value == null)
                m_ClimbSettings = new ClimbSettingsDatumProperty(new ClimbSettings());
        }

        /// <summary>
        /// Starts a grab as part of climbing <paramref name="climbInteractable"/>, using the position of
        /// <paramref name="interactor"/> to drive movement.
        /// </summary>
        /// <param name="climbInteractable">The object to climb.</param>
        /// <param name="interactor">The interactor that initiates the grab and drives movement.</param>
        /// <remarks>
        /// This puts the <see cref="LocomotionProvider.locomotionPhase"/> in the <see cref="LocomotionPhase.Started"/>
        /// state if locomotion has not already started. The phase will then enter the <see cref="LocomotionPhase.Moving"/>
        /// state in the next <see cref="Update"/>.
        /// </remarks>
        public void StartClimbGrab(MyClimbInteractable climbInteractable, Transform interactor)
        {
            var xrOrigin = system.xrOrigin?.Origin;
            if (xrOrigin == null)
                return;

            m_GrabbingInteractors.Add(interactor);
            m_GrabbedClimbables.Add(climbInteractable);

            UpdateClimbAnchor(climbInteractable, interactor);

            if (locomotionPhase != LocomotionPhase.Moving)
                locomotionPhase = LocomotionPhase.Started;
        }

        /// <summary>
        /// Finishes the grab driven by <paramref name="interactor"/>. If this was the most recent grab then movement
        /// will now be driven by the next most recent grab.
        /// </summary>
        /// <param name="interactor">The interactor whose grab to finish.</param>
        /// <remarks>
        /// If there is no other active grab to fall back on, this will put the <see cref="LocomotionProvider.locomotionPhase"/>
        /// in the <see cref="LocomotionPhase.Done"/> state in the next <see cref="Update"/>.
        /// </remarks>
        public void FinishClimbGrab(Transform interactor)
        {
            var interactionIndex = m_GrabbingInteractors.IndexOf(interactor);
            if (interactionIndex < 0)
                return;

            Assert.AreEqual(m_GrabbingInteractors.Count, m_GrabbedClimbables.Count);

            m_GrabbingInteractors.RemoveAt(interactionIndex);
            m_GrabbedClimbables.RemoveAt(interactionIndex);

            m_InteractorAnchorClimbSpacePosition.RemoveAt(interactionIndex);
            m_InteractorAnchorWorldPosition.RemoveAt(interactionIndex);
        }

        void UpdateClimbAnchor(MyClimbInteractable climbInteractable, Transform interactor)
        {
            var climbTransform = climbInteractable.climbTransform;

            if (m_InteractorAnchorClimbSpacePosition.Count == 2)
            {
                Vector3 tmpWorldPos = m_InteractorAnchorWorldPosition[1];
                Vector3 tmpClimbPos = m_InteractorAnchorClimbSpacePosition[1];

                m_InteractorAnchorWorldPosition[0] = tmpWorldPos;
                m_InteractorAnchorClimbSpacePosition[0] = tmpClimbPos;

                m_InteractorAnchorWorldPosition[1] = interactor.position;
                m_InteractorAnchorClimbSpacePosition[1] = climbTransform.InverseTransformPoint(m_InteractorAnchorWorldPosition[1]);
            }
            else
            {
                m_InteractorAnchorWorldPosition.Add(interactor.position);
                m_InteractorAnchorClimbSpacePosition.Add(climbTransform.InverseTransformPoint(m_InteractorAnchorWorldPosition[m_InteractorAnchorWorldPosition.Count - 1]));
            }

            //climbAnchorUpdated?.Invoke(this);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void Update()
        {
            if (locomotionPhase == LocomotionPhase.Done)
            {
                locomotionPhase = LocomotionPhase.Idle;
                return;
            }

            // Use the most recent interaction to drive movement
            if (m_GrabbingInteractors.Count > 0)
            {
                if (locomotionPhase != LocomotionPhase.Moving)
                {
                    if (!BeginLocomotion())
                        return;

                    locomotionPhase = LocomotionPhase.Moving;
                }
                else if (system.RequestExclusiveOperation(this) == RequestResult.Busy)
                {
                    // Need to make sure that we still have exclusive access to the XR Origin, since we can lose access
                    // after the system timeout occurs. If this returns RequestResult.Error, it's not actually a problem -
                    // it just means that we already have exclusive access.
                    return;
                }

                Assert.AreEqual(m_GrabbingInteractors.Count, m_GrabbedClimbables.Count);

                StepClimbMovement();
            }
            else if (locomotionPhase != LocomotionPhase.Idle)
            {
                FinishLocomotion();
            }
        }

        void StepClimbMovement()
        {
            var xrOrigin = system.xrOrigin?.Origin;
            var playerRigidBody = characterController.attachedRigidbody;
            if (xrOrigin != null)
            {
                // Move rig such that climb interactor position stays constant
                var rigTransform = xrOrigin.transform;
                Vector3 movement = Vector3.zero;
                //float drag = 0;

                for (int i = 0; i < m_GrabbedClimbables.Count; i++)
                {
                    var activeClimbSettings = GetActiveClimbSettings(m_GrabbedClimbables[i]);
                    var allowFreeXMovement = activeClimbSettings.allowFreeXMovement;
                    var allowFreeYMovement = activeClimbSettings.allowFreeYMovement;
                    var allowFreeZMovement = activeClimbSettings.allowFreeZMovement;
                    var interactorWorldPosition = m_GrabbingInteractors[i].position;

                    if (allowFreeXMovement && allowFreeYMovement && allowFreeZMovement)
                    {
                        // No need to check position relative to climbable object if movement is unconstrained
                        movement += m_InteractorAnchorWorldPosition[i] - interactorWorldPosition;
                    }
                    else
                    {
                        var climbTransform = m_GrabbedClimbables[i].climbTransform;
                        var interactorClimbSpacePosition = climbTransform.InverseTransformPoint(interactorWorldPosition);
                        var movementInClimbSpace = m_InteractorAnchorClimbSpacePosition[i] - interactorClimbSpacePosition;

                        if (!allowFreeXMovement)
                            movementInClimbSpace.x = 0f;

                        if (!allowFreeYMovement)
                            movementInClimbSpace.y = 0f;

                        if (!allowFreeZMovement)
                            movementInClimbSpace.z = 0f;

                        movement += climbTransform.TransformVector(movementInClimbSpace);
                    }
                    //drag += GetDrag(m_GrabbingInteractors[i]);
                }

                movement /= m_GrabbingInteractors.Count;
                //movement *= climbForce;

                //drag /= m_GrabbingInteractors.Count;

                //characterController.attachedRigidbody.AddForce(movement, ForceMode.Acceleration);
                //characterController.attachedRigidbody.AddForce(drag * -characterController.attachedRigidbody.velocity * climbDrag, ForceMode.Acceleration);

                rigTransform.position += movement;
            }
        }

        //float GetDrag(Transform currentInteractor)
        //{
        //    var controller = currentInteractor.parent.GetComponent<XRController>();

        //    if (controller != null && controller.inputDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 velocity))
        //    {
        //        float drag = 1 / velocity.magnitude + 0.01f;
        //        drag = drag > 1 ? 1 : drag;
        //        drag = drag < 0.03f ? 0.03f : drag;
        //        return drag;
        //    }
        //    Debug.Log("Controller is null");
        //    return 0;
        //}

        public void FinishLocomotion()
        {
            EndLocomotion();
            locomotionPhase = LocomotionPhase.Done;
            m_GrabbingInteractors.Clear();
            m_GrabbedClimbables.Clear();
            m_InteractorAnchorClimbSpacePosition.Clear();
            m_InteractorAnchorWorldPosition.Clear();
        }

        ClimbSettings GetActiveClimbSettings(MyClimbInteractable climbInteractable)
        {
            if (climbInteractable.climbSettingsOverride.Value != null)
                return climbInteractable.climbSettingsOverride;

            return m_ClimbSettings;
        }
    }
}