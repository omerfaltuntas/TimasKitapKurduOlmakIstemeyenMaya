using UnityEngine;
using DG.Tweening;
using System.Collections;

namespace BirBBH
{
    public class BBBH_AutoMoveAndReturn : MonoBehaviour
    {
        public enum MovementType
        {
            Linear,
            CurvedPath
        }

        [Header("Movement Settings")]
        [Tooltip("The type of movement for the forward motion.")]
        public MovementType movementType = MovementType.Linear;
        [Tooltip("The local position this object will move TO automatically.")]
        public Vector3 targetLocalPosition;
        [Tooltip("How high the curve arcs for the forward motion (if CurvedPath is selected).")]
        public float curveHeight = 2.0f;
        [Tooltip("Duration of the move TO the target.")]
        public float moveDuration = 0.5f;
        [Tooltip("Duration of the move back to the initial position.")]
        public float returnDuration = 0.5f;
        [Tooltip("Easing function for the movement.")]
        public Ease moveEase = Ease.OutQuad;

        [Header("Timing")]
        [Tooltip("Initial delay AFTER tutorial/enable before the movement sequence starts.")]
        public float initialDelay = 0f;
        [Tooltip("Delay at the target position before returning.")]
        public float delayAtTarget = 0.5f;

        [Header("Looping")]
        [Tooltip("If greater than 0, the entire move-and-return sequence will loop this many times. -1 for infinite loop.")]
        public int loopCount = 0;
        [Tooltip("Delay between each full loop cycle.")]
        public float delayBetweenLoops = 0f;

        [Header("Tutorial Trigger (Optional)")]
        [Tooltip("If checked, the animation will only start after the specified Tutorial Item is completed or skipped.")]
        public bool triggerAfterTutorial = false;
        public TutorialItem waitForTutorialItem;

        // --- Private Fields ---
        private Vector3 initialLocalPosition;
        private Coroutine controlCoroutine;
        private Sequence currentMoveSequence;

        void Awake()
        {
            initialLocalPosition = transform.localPosition;

            moveDuration = Mathf.Max(0.001f, moveDuration);
            returnDuration = Mathf.Max(0.001f, returnDuration);
            initialDelay = Mathf.Max(0, initialDelay);
            delayAtTarget = Mathf.Max(0, delayAtTarget);
            delayBetweenLoops = Mathf.Max(0, delayBetweenLoops);
        }

        void OnEnable()
        {
            if (controlCoroutine != null) { StopCoroutine(controlCoroutine); controlCoroutine = null; }
            KillCurrentMoveSequence();

            transform.localPosition = initialLocalPosition;

            if (triggerAfterTutorial && waitForTutorialItem != null)
            {
                controlCoroutine = StartCoroutine(WaitForTutorialAndStartAnimation());
            }
            else
            {
                controlCoroutine = StartCoroutine(DelayedStartAnimation(initialDelay));
            }
        }

        void OnDisable()
        {
            if (controlCoroutine != null) { StopCoroutine(controlCoroutine); controlCoroutine = null; }
            KillCurrentMoveSequence();

            transform.localPosition = initialLocalPosition;
        }

        private IEnumerator DelayedStartAnimation(float waitTime)
        {
            if (waitTime > 0)
            {
                yield return new WaitForSeconds(waitTime);
            }
            ProcessMovementSequence();
        }

        private IEnumerator WaitForTutorialAndStartAnimation()
        {
            yield return null;
            if (waitForTutorialItem == null || waitForTutorialItem.IsSkipped)
            {
                yield return StartCoroutine(DelayedStartAnimation(initialDelay));
                yield break;
            }
            if (!waitForTutorialItem.IsActive)
            {
                yield return new WaitUntil(() => waitForTutorialItem.IsActive || waitForTutorialItem.IsSkipped);
            }
            if (waitForTutorialItem.IsSkipped)
            {
                yield return StartCoroutine(DelayedStartAnimation(initialDelay));
                yield break;
            }
            yield return new WaitUntil(() => !waitForTutorialItem.IsActive || waitForTutorialItem.IsSkipped);
            yield return StartCoroutine(DelayedStartAnimation(initialDelay));
        }

        void ProcessMovementSequence()
        {
            KillCurrentMoveSequence();
            currentMoveSequence = DOTween.Sequence();

            // --- THE CORE CHANGE IS HERE ---
            Tweener forwardTween;

            if (movementType == MovementType.Linear)
            {
                // Create a standard linear move tween
                forwardTween = transform.DOLocalMove(targetLocalPosition, moveDuration);
            }
            else // movementType == MovementType.CurvedPath
            {
                // Create a curved path tween
                Vector3[] pathWaypoints = new Vector3[3];
                pathWaypoints[0] = initialLocalPosition; // Start
                // Calculate the apex of the curve
                Vector3 apex = (initialLocalPosition + targetLocalPosition) / 2f;
                apex.y += curveHeight; // Add the height offset
                pathWaypoints[1] = apex; // Control point
                pathWaypoints[2] = targetLocalPosition; // End

                forwardTween = transform.DOLocalPath(pathWaypoints, moveDuration, PathType.CatmullRom)
                                         .SetOptions(false); // False for non-closed path
            }

            forwardTween.SetEase(moveEase); // Apply the ease to the created tween

            // Build the core move-and-return block using the chosen forward tween
            Sequence oneCycle = DOTween.Sequence()
                .Append(forwardTween) // Use the tween we just created
                .AppendInterval(delayAtTarget)
                .Append(transform.DOLocalMove(initialLocalPosition, returnDuration).SetEase(moveEase)); // Return is always linear

            if (loopCount != 0 && delayBetweenLoops > 0)
            {
                oneCycle.AppendInterval(delayBetweenLoops);
            }

            if (loopCount != 0)
            {
                oneCycle.SetLoops(loopCount, LoopType.Restart);
            }

            currentMoveSequence.Append(oneCycle);
        }

        void KillCurrentMoveSequence()
        {
            if (currentMoveSequence != null && currentMoveSequence.IsActive())
            {
                currentMoveSequence.Kill(false);
            }
            currentMoveSequence = null;
        }
    }
}