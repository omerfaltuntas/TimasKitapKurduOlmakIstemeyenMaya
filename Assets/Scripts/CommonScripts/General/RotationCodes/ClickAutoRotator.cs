using UnityEngine;
using DG.Tweening;
using System.Collections;
//Tutoriali üstüne attığımızda tıklamali atmadıgımız takdirde kendi ekseninde 360 derece döndüren kod. Sınırsız
public class ClickAutoRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("The rotation to add over one full loop duration. For a full 360-degree clockwise turn on Z, use (0, 0, -360).")]
    public Vector3 rotationDegrees = new Vector3(0, 0, -360);
    [Tooltip("The duration (in seconds) for one full rotation cycle (e.g., a full 360 degrees).")]
    public float loopDuration = 2.0f;
    [Tooltip("The easing function for the rotation. Linear is recommended for a constant speed.")]
    public Ease rotationEase = Ease.Linear;

    [Header("Start Control")]
    [Tooltip("Delay before the rotation starts (after tutorial condition, if any).")]
    public float delay = 0f;

    [Header("Tutorial Trigger (Optional)")]
    [Tooltip("If checked, the rotation will only start after the specified Tutorial Item is completed or skipped.")]
    public bool triggerAfterTutorial = false;
    public TutorialItem waitForTutorialItem;

    // --- Private Fields ---
    private Quaternion initialLocalRotation;
    private Tween currentRotateTween;
    private Coroutine controlCoroutine;

    void Awake()
    {
        initialLocalRotation = transform.localRotation; // Store the initial rotation

        if (loopDuration <= 0)
        {
            Debug.LogWarning($"[{gameObject.name}/bb_ContinuousRotate]: loopDuration must be positive. Defaulting to 2.0s.");
            loopDuration = 2.0f;
        }
    }

    void OnEnable()
    {
        // Stop any ongoing processes
        if (controlCoroutine != null) { StopCoroutine(controlCoroutine); controlCoroutine = null; }
        KillCurrentTween();

        // ALWAYS reset to the initial state when this object becomes enabled
        transform.localRotation = initialLocalRotation;

        // Start the activation logic
        if (triggerAfterTutorial && waitForTutorialItem != null)
        {
            controlCoroutine = StartCoroutine(WaitForTutorialAndStartRotation());
        }
        else
        {
            if (delay > 0)
            {
                controlCoroutine = StartCoroutine(DelayedStartRotation(delay));
            }
            else
            {
                StartRotation();
            }
        }
    }

    void OnDisable()
    {
        if (controlCoroutine != null) { StopCoroutine(controlCoroutine); controlCoroutine = null; }
        KillCurrentTween();

        // When this object is disabled, reset it to its initial rotation
        transform.localRotation = initialLocalRotation;
    }

    private IEnumerator DelayedStartRotation(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        StartRotation();
    }

    private IEnumerator WaitForTutorialAndStartRotation()
    {
        yield return null;
        if (waitForTutorialItem == null || waitForTutorialItem.IsSkipped)
        {
            if (delay > 0) yield return new WaitForSeconds(delay);
            StartRotation();
            yield break;
        }
        if (!waitForTutorialItem.IsActive)
        {
            yield return new WaitUntil(() => waitForTutorialItem.IsActive || waitForTutorialItem.IsSkipped);
        }
        if (waitForTutorialItem.IsSkipped)
        {
            if (delay > 0) yield return new WaitForSeconds(delay);
            StartRotation();
            yield break;
        }
        yield return new WaitUntil(() => !waitForTutorialItem.IsActive || waitForTutorialItem.IsSkipped);
        if (delay > 0) yield return new WaitForSeconds(delay);
        StartRotation();
    }

    void StartRotation()
    {
        if (loopDuration <= 0) return;

        KillCurrentTween();

        // DOLocalRotate adds the specified rotation to the current rotation over the duration.
        // We set it to be relative and use LoopType.Restart to make it a continuous loop.
        currentRotateTween = transform.DOLocalRotate(rotationDegrees, loopDuration, RotateMode.FastBeyond360)
            .SetEase(rotationEase)
            .SetLoops(-1, LoopType.Restart); // Loop infinitely by restarting the same relative rotation
    }

    void KillCurrentTween()
    {
        if (currentRotateTween != null && currentRotateTween.IsActive())
        {
            currentRotateTween.Kill(false);
        }
        currentRotateTween = null;
    }
}