using UnityEngine;
using DG.Tweening;
using System.Collections;
using System; // Required for Array.IndexOf

namespace BirBBH
{
    public class BBBH_RotateOnce : MonoBehaviour
    {
        [Header("Rotation Settings")]
        [Tooltip("The target angle (in degrees) to rotate to. For sequential rotations, this is an absolute angle on the specified axis.")]
        public float targetRotationAngle = 90.0f;
        public float rotationDuration = 1.0f;
        public Vector3 rotationAxis = Vector3.forward;
        public Ease rotationEase = Ease.OutQuad;
        [Tooltip("Delay AFTER the previous animation in the chain completes, before this one starts.")]
        public float delay = 0f;

        [Header("Tutorial Trigger (Optional - ONLY for the FIRST script)")]
        [Tooltip("If checked, the ENTIRE chain of rotations will wait for this tutorial. Only the first script's settings are used.")]
        public bool triggerAfterTutorial = false;
        public TutorialItem waitForTutorialItem;

        // --- Private Fields ---
        private Quaternion initialLocalRotation;
        private Coroutine controlCoroutine;
        private Tweener currentRotateTween;

        void Awake()
        {
            initialLocalRotation = transform.localRotation;

            if (rotationAxis.sqrMagnitude == 0)
            {
                rotationAxis = Vector3.forward;
            }
            else
            {
                rotationAxis.Normalize();
            }

            if (rotationDuration <= 0)
            {
                rotationDuration = 1.0f;
            }
            delay = Mathf.Max(0, delay);
        }

        void OnEnable()
        {
            // Reset state for all instances.
            transform.localRotation = initialLocalRotation;

            // Stop any coroutines or tweens from a previous state.
            if (controlCoroutine != null) { StopCoroutine(controlCoroutine); controlCoroutine = null; }
            if (currentRotateTween != null && currentRotateTween.IsActive()) { currentRotateTween.Kill(); }
            currentRotateTween = null;

            // --- Chain Logic ---
            var allInstances = GetComponents<BBBH_RotateOnce>();
            int myIndex = Array.IndexOf(allInstances, this);

            // ONLY the first script in the list is responsible for starting the chain.
            if (myIndex == 0)
            {
                controlCoroutine = StartCoroutine(InitialChainStart());
            }
            // Follower scripts (myIndex > 0) do nothing here. They wait to be triggered.
        }

        void OnDisable()
        {
            if (controlCoroutine != null) { StopCoroutine(controlCoroutine); controlCoroutine = null; }
            if (currentRotateTween != null && currentRotateTween.IsActive()) { currentRotateTween.Kill(false); }
            currentRotateTween = null;

            transform.localRotation = initialLocalRotation;
        }

        // Called only by the leader script
        private IEnumerator InitialChainStart()
        {
            if (triggerAfterTutorial && waitForTutorialItem != null)
            {
                yield return null;
                if (!waitForTutorialItem.IsSkipped)
                {
                    if (!waitForTutorialItem.IsActive)
                    {
                        yield return new WaitUntil(() => waitForTutorialItem.IsActive || waitForTutorialItem.IsSkipped);
                    }
                    if (!waitForTutorialItem.IsSkipped)
                    {
                        yield return new WaitUntil(() => !waitForTutorialItem.IsActive || waitForTutorialItem.IsSkipped);
                    }
                }
            }

            // Trigger the first animation in the chain.
            TriggerMyRotation();
        }

        // Public method to be called by the previous script in the chain
        public void TriggerMyRotation()
        {
            controlCoroutine = StartCoroutine(DelayedStartRotation(this.delay));
        }

        private IEnumerator DelayedStartRotation(float waitTime)
        {
            if (waitTime > 0)
            {
                yield return new WaitForSeconds(waitTime);
            }
            StartActualRotation();
        }

        private void StartActualRotation()
        {
            if (currentRotateTween != null && currentRotateTween.IsActive())
            {
                currentRotateTween.Kill();
            }

            // --- THE CRITICAL FIX ---
            // The target rotation is now an ABSOLUTE rotation, not relative to the initial state.
            // This allows for sequential rotations like 0 -> 90, then 90 -> 180.
            // AngleAxis creates a rotation of 'angle' degrees around 'axis'.
            Quaternion targetRotation = Quaternion.AngleAxis(targetRotationAngle, rotationAxis);

            // Use DOLocalRotateQuaternion to rotate to a specific target orientation.
            // This starts from the CURRENT rotation, which is what we want for a chain.
            currentRotateTween = transform.DOLocalRotateQuaternion(targetRotation, rotationDuration)
                .SetEase(rotationEase)
                .OnComplete(OnMyRotationComplete);
        }

        // This is called when THIS script's rotation finishes.
        private void OnMyRotationComplete()
        {
            // --- Hand-off Logic ---
            var allInstances = GetComponents<BBBH_RotateOnce>();
            int myIndex = Array.IndexOf(allInstances, this);

            // Check if there is a "next" script in the list.
            if (myIndex != -1 && myIndex + 1 < allInstances.Length)
            {
                // Trigger the next script's animation.
                allInstances[myIndex + 1].TriggerMyRotation();
            }
        }
    }
}