using UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Interactable that can be climbed while selected.
    /// </summary>
    /// <seealso cref="ClimbProvider"/>
    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    [AddComponentMenu("XR/My Climb Interactable", 11)]
    public class MyClimbInteractable : XRBaseInteractable
    {
        const float k_DefaultMaxInteractionDistance = 0.1f;

        [SerializeField]
        [Tooltip("The climb provider that performs locomotion while this interactable is selected. " +
                 "If no climb provider is configured, will attempt to find one.")]
        MyClimbProvider m_ClimbProvider;

        /// <summary>
        /// The climb provider that performs locomotion while this interactable is selected.
        /// If no climb provider is configured, will attempt to find one.
        /// </summary>
        public MyClimbProvider climbProvider
        {
            get => m_ClimbProvider;
            set => m_ClimbProvider = value;
        }

        [SerializeField]
        [Tooltip("Transform that defines the coordinate space for climb locomotion. " +
                 "Will use this GameObject's Transform by default.")]
        Transform m_ClimbTransform;

        /// <summary>
        /// Transform that defines the coordinate space for climb locomotion. Will use this GameObject's Transform by default.
        /// </summary>
        public Transform climbTransform
        {
            get
            {
                if (m_ClimbTransform == null)
                    m_ClimbTransform = transform;
                return m_ClimbTransform;
            }
            set => m_ClimbTransform = value;
        }

        [SerializeField]
        [Tooltip("Controls whether to apply a distance check when validating hover and select interaction.")]
        bool m_FilterInteractionByDistance = true;

        /// <summary>
        /// Controls whether to apply a distance check when validating hover and select interaction.
        /// </summary>
        /// <seealso cref="maxInteractionDistance"/>
        /// <seealso cref="XRBaseInteractable.distanceCalculationMode"/>
        public bool filterInteractionByDistance
        {
            get => m_FilterInteractionByDistance;
            set => m_FilterInteractionByDistance = value;
        }

        [SerializeField]
        [Tooltip("The maximum distance that an interactor can be from this interactable to begin hover or select.")]
        float m_MaxInteractionDistance = k_DefaultMaxInteractionDistance;

        /// <summary>
        /// The maximum distance that an interactor can be from this interactable to begin hover or select.
        /// Only applies when <see cref="filterInteractionByDistance"/> is <see langword="true"/>.
        /// </summary>
        /// <seealso cref="filterInteractionByDistance"/>
        /// <seealso cref="XRBaseInteractable.distanceCalculationMode"/>
        public float maxInteractionDistance
        {
            get => m_MaxInteractionDistance;
            set => m_MaxInteractionDistance = value;
        }

        [SerializeField]
        [Tooltip("The teleport volume used to assist with movement to a specific destination after ending a climb " +
            "(optional, may be None). Only used if there is a Climb Teleport Interactor in the scene.")]
        TeleportationMultiAnchorVolume m_ClimbAssistanceTeleportVolume;

        /// <summary>
        /// The teleport volume used to assist with movement to a specific destination after ending a climb (optional,
        /// may be <see langword="null"/>). If there is a <see cref="ClimbTeleportInteractor"/> in the scene that
        /// references the same <see cref="ClimbProvider"/> as this interactable, it will interact with the volume while
        /// this interactable is being climbed.
        /// </summary>
        public TeleportationMultiAnchorVolume climbAssistanceTeleportVolume
        {
            get => m_ClimbAssistanceTeleportVolume;
            set => m_ClimbAssistanceTeleportVolume = value;
        }

        [SerializeField]
        [Tooltip("Optional override of locomotion settings specified in the climb provider. " +
                 "Only applies as an override if set to Use Value or if the asset reference is set.")]
        ClimbSettingsDatumProperty m_ClimbSettingsOverride;

        /// <summary>
        /// Optional override of climb locomotion settings specified in the climb provider. Only applies as
        /// an override if <see cref="Unity.XR.CoreUtils.Datums.DatumProperty{TValue, TDatum}.Value"/> is not <see langword="null"/>.
        /// </summary>
        public ClimbSettingsDatumProperty climbSettingsOverride
        {
            get => m_ClimbSettingsOverride;
            set => m_ClimbSettingsOverride = value;
        }

        public List<Transform> attachPoints = new List<Transform>();
        [SerializeField] GameManager gameManager;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnValidate()
        {
            if (m_ClimbTransform == null)
                m_ClimbTransform = transform;
        }

        /// <inheritdoc />
        protected override void Reset()
        {
            base.Reset();

            selectMode = InteractableSelectMode.Multiple;
            m_ClimbTransform = transform;
        }

        /// <inheritdoc />
        protected override void Awake()
        {
            base.Awake();
        }

        /// <inheritdoc />
        public override bool IsHoverableBy(IXRHoverInteractor interactor)
        {
            return base.IsHoverableBy(interactor) && (!m_FilterInteractionByDistance ||
                GetDistanceSqrToInteractor(interactor) <= m_MaxInteractionDistance * m_MaxInteractionDistance);
        }

        /// <inheritdoc />
        public override bool IsSelectableBy(IXRSelectInteractor interactor)
        {
            return base.IsSelectableBy(interactor) && (IsSelected(interactor) || !m_FilterInteractionByDistance ||
                GetDistanceSqrToInteractor(interactor) <= m_MaxInteractionDistance * m_MaxInteractionDistance);
        }

        /// <inheritdoc />
        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            Transform interactor = args.interactorObject.transform;

            if (args.interactableObject.transform.CompareTag("HorizontalHold"))
                return;

            base.OnSelectEntered(args);

            Transform attachPoint = FindClosestAttachPoint(interactor);
            interactor.parent.transform.position = attachPoint.position;

            interactor.parent.GetComponent<HandPhysics>().ClimbGrabFixHand(attachPoint);

            if (m_ClimbProvider != null)
                m_ClimbProvider.StartClimbGrab(this, interactor);

            if (gameManager.proClimbingMode)
                interactor.GetComponent<ControllerStamina>().StartStamina();
        }

        /// <inheritdoc />
        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            Transform interactor = args.interactorObject.transform;

            base.OnSelectExited(args);

            interactor.parent.GetComponent<HandPhysics>().ClimbGrabUnfixHand();

            if (m_ClimbProvider != null)
                m_ClimbProvider.FinishClimbGrab(interactor);

            if (gameManager.proClimbingMode)
                interactor.GetComponent<ControllerStamina>().EndStamina();
        }

        protected override void OnActivated(ActivateEventArgs args)
        {
            Transform interactor = args.interactorObject.transform;

            if (args.interactableObject.transform.CompareTag("VerticalHold"))
                return;

            base.OnActivated(args);

            Transform attachPoint = FindClosestAttachPoint(interactor);
            interactor.parent.transform.position = attachPoint.position;

            interactor.parent.GetComponent<HandPhysics>().ClimbGrabFixHand(attachPoint);

            if (m_ClimbProvider != null)
                m_ClimbProvider.StartClimbGrab(this, interactor);

            if (gameManager.proClimbingMode)
                interactor.GetComponent<ControllerStamina>().StartStamina();
        }

        /// <inheritdoc />
        protected override void OnDeactivated(DeactivateEventArgs args)
        {
            Transform interactor = args.interactorObject.transform;

            base.OnDeactivated(args);

            interactor.parent.GetComponent<HandPhysics>().ClimbGrabUnfixHand();

            if (m_ClimbProvider != null)
                m_ClimbProvider.FinishClimbGrab(interactor);

            if (gameManager.proClimbingMode)
                interactor.GetComponent<ControllerStamina>().EndStamina();
        }

        public void DeactivateHold(DeactivateEventArgs args)
        {
            Transform interactor = args.interactorObject.transform;

            base.OnDeactivated(args);

            interactor.parent.GetComponent<HandPhysics>().ClimbGrabUnfixHand();

            if (m_ClimbProvider != null)
                m_ClimbProvider.FinishClimbGrab(interactor);

            if (gameManager.proClimbingMode)
                interactor.GetComponent<ControllerStamina>().EndStamina();
        }

        Transform FindClosestAttachPoint(Transform interactor)
        {
            if (attachPoints == null || attachPoints.Count == 0)
                return transform;

            Transform closest = attachPoints[0];
            float closestDist = Vector3.Distance(interactor.position, closest.position);

            for (int i = 1; i < attachPoints.Count; i++)
            {
                float distanceToI = Vector3.Distance(interactor.position, attachPoints[i].position);
                if (distanceToI < closestDist)
                {
                    closest = attachPoints[i];
                    closestDist = distanceToI;
                }
            }

            return closest;
        }
    }
}