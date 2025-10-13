using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
//Top veya nesne zıplatma kodu.

public class BallJump : MonoBehaviour
{
    public enum HorizontalDirection
    {
        GoRight,
        GoLeft
    }

    [Header("Bounce Settings")]
    [Tooltip("The direction the ball will travel horizontally.")]
    public HorizontalDirection bounceDirection = HorizontalDirection.GoRight;
    [Tooltip("The number of times the ball will bounce before leaving.")]
    public int bounceCount = 4;
    [Tooltip("The height of the very first bounce.")]
    public float initialBounceHeight = 5.0f;
    [Tooltip("How much each bounce's height is reduced. 0.6 means each bounce is 60% as high as the last.")]
    [Range(0.1f, 0.9f)]
    public float bounceDecay = 0.6f;
    [Tooltip("The duration of a single bounce (up and down). This will be the duration of the FIRST bounce.")]
    public float durationPerBounce = 0.5f;
    [Tooltip("How much each bounce's duration is reduced. 0.8 means each bounce is 80% as fast as the last.")]
    [Range(0.1f, 1f)]
    public float durationDecay = 0.8f; // NEW FIELD
    [Tooltip("The total horizontal distance the ball will travel (always a positive number).")]
    public float totalHorizontalDrift = 3.0f;

    [Header("Exit Animation")]
    [Tooltip("How far the ball travels horizontally to leave the scene after its last bounce.")]
    public float leaveSceneDistance = 10f;
    [Tooltip("How long the final 'leave scene' animation takes.")]
    public float leaveDuration = 1.0f;

    [Header("Tutorial Trigger (for Clickability)")]
    public bool lockClickUntilTutorial = false;
    public TutorialItem waitForTutorialItem;
    [Tooltip("Delay after tutorial completion before this object becomes clickable.")]
    public float delayAfterTutorialUntilClickable = 0f;

    // --- Private Fields ---
    private Vector3 initialLocalPosition;
    private Coroutine controlCoroutine;
    private Sequence bounceSequence;
    private bool isClickableNow = false;
    private bool hasBeenClickedThisEnableCycle = false;

    void Awake()
    {
        initialLocalPosition = transform.localPosition;
        bounceCount = Mathf.Max(1, bounceCount);
        durationPerBounce = Mathf.Max(0.1f, durationPerBounce);
    }

    void OnEnable()
    {
        if (controlCoroutine != null) StopCoroutine(controlCoroutine);
        KillCurrentSequence();

        transform.localPosition = initialLocalPosition;

        hasBeenClickedThisEnableCycle = false;
        isClickableNow = !lockClickUntilTutorial;

        if (lockClickUntilTutorial && waitForTutorialItem != null)
        {
            controlCoroutine = StartCoroutine(ActivateClickAfterTutorial());
        }
        else if (delayAfterTutorialUntilClickable > 0)
        {
            isClickableNow = false;
            controlCoroutine = StartCoroutine(DelayedClickActivation(delayAfterTutorialUntilClickable));
        }
    }

    void OnDisable()
    {
        if (controlCoroutine != null) StopCoroutine(controlCoroutine);
        KillCurrentSequence();

        transform.localPosition = initialLocalPosition;
    }

    private IEnumerator DelayedClickActivation(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        isClickableNow = true;
    }

    private IEnumerator ActivateClickAfterTutorial()
    {
        yield return null;
        if (waitForTutorialItem == null || waitForTutorialItem.IsSkipped)
        {
            yield return StartCoroutine(DelayedClickActivation(delayAfterTutorialUntilClickable));
            yield break;
        }
        if (!waitForTutorialItem.IsActive)
        {
            yield return new WaitUntil(() => waitForTutorialItem.IsActive || waitForTutorialItem.IsSkipped);
        }
        if (waitForTutorialItem.IsSkipped)
        {
            yield return StartCoroutine(DelayedClickActivation(delayAfterTutorialUntilClickable));
            yield break;
        }
        yield return new WaitUntil(() => !waitForTutorialItem.IsActive || waitForTutorialItem.IsSkipped);
        yield return StartCoroutine(DelayedClickActivation(delayAfterTutorialUntilClickable));
    }

    void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!enabled || !isClickableNow || hasBeenClickedThisEnableCycle) return;

            hasBeenClickedThisEnableCycle = true;
            StartBounceAnimation();
        }
    }

    void StartBounceAnimation()
    {
        KillCurrentSequence();
        bounceSequence = DOTween.Sequence();

        float directionMultiplier = (bounceDirection == HorizontalDirection.GoRight) ? 1f : -1f;
        float horizontalMagnitude = Mathf.Abs(totalHorizontalDrift);

        float horizontalDriftPerBounce = (horizontalMagnitude / bounceCount) * directionMultiplier;

        // --- MODIFIED LOGIC ---
        float currentBounceHeight = initialBounceHeight;
        float currentBounceDuration = durationPerBounce; // Start with the initial duration
        Vector3 lastPosition = initialLocalPosition;

        // Build the bounce chain
        for (int i = 0; i < bounceCount; i++)
        {
            Vector3 nextPosition = new Vector3(
                lastPosition.x + horizontalDriftPerBounce,
                initialLocalPosition.y,
                initialLocalPosition.z
            );

            // Use the CURRENT bounce duration for this jump
            bounceSequence.Append(
                transform.DOLocalJump(nextPosition, currentBounceHeight, 1, currentBounceDuration)
                    .SetEase(Ease.OutFlash)
            );

            // Decay height and duration for the next bounce
            currentBounceHeight *= bounceDecay;
            currentBounceDuration *= durationDecay; // Decay the duration
            lastPosition = nextPosition;
        }
        // --- END MODIFIED LOGIC ---

        // Build the "leave scene" animation
        Vector3 leavePosition = new Vector3(
            lastPosition.x + (leaveSceneDistance * directionMultiplier),
            lastPosition.y - 2f,
            lastPosition.z
        );

        bounceSequence.Append(
            transform.DOLocalJump(leavePosition, currentBounceHeight, 1, leaveDuration)
                .SetEase(Ease.InQuad)
        );
    }

    void KillCurrentSequence()
    {
        if (bounceSequence != null && bounceSequence.IsActive())
        {
            bounceSequence.Kill(false);
        }
        bounceSequence = null;
    }
}