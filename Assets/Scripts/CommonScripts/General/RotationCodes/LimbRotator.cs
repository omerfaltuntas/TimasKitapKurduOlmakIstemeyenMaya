using UnityEngine;
using DG.Tweening;
using System.Collections;
//Bu kod herhangi bir uzvun rotasyon ile hareket etmesini sağlar. 
public class LimbRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationAngle = 30.0f;
    public float oneWayRotationDuration = 1.0f; // Duration for one side of the yoyo (e.g., from +angle to -angle)
    public Vector3 rotationAxis = Vector3.forward;
    public Ease rotationEase = Ease.InOutSine;
    public float delay = 0f; // Delay before the entire sequence starts

    [Header("Loop Duration (Optional)")]
    [Tooltip("If true, the yoyo loop will run for 'Total Loop Duration' and then stop.")]
    public bool loopForSpecificDuration = false;
    [Tooltip("Total time (seconds) the yoyo part of the animation should loop. Starts timing AFTER the initial swing.")]
    public float totalLoopDuration = 5.0f;


    [Header("Tutorial Trigger (Optional)")]
    public bool triggerAfterTutorial = false;
    public TutorialItem waitForTutorialItem;

    private Quaternion initialLocalRotation;
    private Sequence currentRotateSequence;
    private Coroutine controlCoroutine; // Manages tutorial wait, initial delay, and timed loop stop

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

        oneWayRotationDuration = Mathf.Max(0.01f, oneWayRotationDuration); // Must be positive
        delay = Mathf.Max(0, delay);
        totalLoopDuration = Mathf.Max(0.01f, totalLoopDuration);
    }

    void OnEnable()
    {
        transform.localRotation = initialLocalRotation;
        KillAndClearSequence(); // Kills DOTween sequence
        StopControlCoroutine();   // Stops our management coroutine

        if (triggerAfterTutorial && waitForTutorialItem != null)
        {
            controlCoroutine = StartCoroutine(WaitForTutorialAndStartRotationLogic());
        }
        else
        {
            controlCoroutine = StartCoroutine(DelayedStartRotationLogic(delay));
        }
    }

    void OnDisable()
    {
        StopControlCoroutine();
        KillAndClearSequence();
        transform.localRotation = initialLocalRotation; // Ensure reset
    }

    void KillAndClearSequence()
    {
        if (currentRotateSequence != null && currentRotateSequence.IsActive())
        {
            currentRotateSequence.Kill(false); // false: kill immediately
        }
        currentRotateSequence = null;
    }

    void StopControlCoroutine()
    {
        if (controlCoroutine != null)
        {
            StopCoroutine(controlCoroutine);
            controlCoroutine = null;
        }
    }

    // This coroutine now handles the initial delay, then starts the sequence,
    // and if needed, schedules the sequence to stop.
    private IEnumerator DelayedStartRotationLogic(float waitTime)
    {
        if (waitTime > 0)
        {
            yield return new WaitForSeconds(waitTime);
        }

        StartActualRotationSequence(); // Start the DOTween sequence

        if (loopForSpecificDuration && currentRotateSequence != null && currentRotateSequence.IsActive())
        {
            // The timed stop should consider the duration of the initial swing
            // if totalLoopDuration is meant for the *entire* animation.
            // If totalLoopDuration is only for the yoyo part, then:
            // yield return new WaitForSeconds(totalLoopDuration);
            //
            // Let's assume totalLoopDuration applies AFTER the initial swing completes.
            // The initial swing takes oneWayRotationDuration / 2.0f.
            // The looping yoyo part starts after that.

            // Wait for the initial swing to complete
            float initialSwingDuration = oneWayRotationDuration / 2.0f;
            yield return new WaitForSeconds(initialSwingDuration);

            // Now wait for the specified totalLoopDuration for the yoyo part
            if (currentRotateSequence != null && currentRotateSequence.IsActive()) // Check again, sequence might have been killed
            {
                yield return new WaitForSeconds(totalLoopDuration);

                // After totalLoopDuration, kill the sequence if it's still running
                if (currentRotateSequence != null && currentRotateSequence.IsActive())
                {
                    currentRotateSequence.Kill(false); // Kill the sequence
                    currentRotateSequence = null;
                    // Object will remain at its current rotation.
                    // If you need it to go back to initialLocalRotation:
                    // transform.DOLocalRotateQuaternion(initialLocalRotation, 0.5f).SetEase(rotationEase);
                }
            }
        }
        controlCoroutine = null; // Mark this management coroutine as done
    }

    private IEnumerator WaitForTutorialAndStartRotationLogic()
    {
        yield return null;

        if (waitForTutorialItem == null)
        {
            yield return StartCoroutine(DelayedStartRotationLogic(delay));
            yield break;
        }

        if (waitForTutorialItem.IsSkipped)
        {
            yield return StartCoroutine(DelayedStartRotationLogic(delay));
            yield break;
        }

        if (!waitForTutorialItem.IsActive)
        {
            yield return new WaitUntil(() => waitForTutorialItem.IsActive || waitForTutorialItem.IsSkipped);
        }

        if (waitForTutorialItem.IsSkipped)
        {
            yield return StartCoroutine(DelayedStartRotationLogic(delay));
            yield break;
        }

        yield return new WaitUntil(() => !waitForTutorialItem.IsActive || waitForTutorialItem.IsSkipped);

        // Tutorial done, now apply the delay and the rest of the logic
        yield return StartCoroutine(DelayedStartRotationLogic(delay));
    }

    // This creates the DOTween sequence
    private void StartActualRotationSequence()
    {
        KillAndClearSequence(); // Ensure no old sequence is running
        transform.localRotation = initialLocalRotation; // Reset before starting

        currentRotateSequence = DOTween.Sequence();

        Quaternion targetPositiveRotation = initialLocalRotation * Quaternion.AngleAxis(rotationAngle, rotationAxis);
        Quaternion targetNegativeRotation = initialLocalRotation * Quaternion.AngleAxis(-rotationAngle, rotationAxis);

        // Part 1: Initial swing to one extent
        currentRotateSequence.Append(transform.DOLocalRotateQuaternion(targetPositiveRotation, oneWayRotationDuration / 2.0f)
            .SetEase(rotationEase));

        // Part 2: Yoyo loop between extents
        // This part will be infinite if loopForSpecificDuration is false
        Tween yoyoTween = transform.DOLocalRotateQuaternion(targetNegativeRotation, oneWayRotationDuration)
            .SetEase(rotationEase);

        if (!loopForSpecificDuration) // Only set infinite loops if not time-limited
        {
            yoyoTween.SetLoops(-1, LoopType.Yoyo);
        }
        // If loopForSpecificDuration is true, we don't set .SetLoops(-1).
        // The yoyoTween will play once (P -> N -> P) unless the sequence is killed earlier.
        // The coroutine DelayedStartRotationLogic will handle killing the entire sequence.
        // For the yoyo to behave more like a continuous loop that gets cut short,
        // it *should* have SetLoops(-1, LoopType.Yoyo). The coroutine handles stopping it.

        yoyoTween.SetLoops(-1, LoopType.Yoyo); // Always set to loop; coroutine will stop it if timed.

        currentRotateSequence.Append(yoyoTween);
    }
}