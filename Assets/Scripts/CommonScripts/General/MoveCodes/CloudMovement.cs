using UnityEngine;
using DG.Tweening;
using System.Collections; // Required for Coroutines
//Bulutların Loop halinde hareket etmesini sağlayan kod.

public class CloudMovement : MonoBehaviour
{
    public enum MovementDirection
    {
        LeftToRight,
        RightToLeft
    }

    [Header("Movement Settings")]
    [Tooltip("The direction of the looping movement.")]
    public MovementDirection direction = MovementDirection.LeftToRight;
    [Tooltip("How fast the object moves in world units per second.")]
    public float moveSpeed = 1.0f;
    [Tooltip("Delay before the movement starts (after tutorial condition, if any).")]
    public float delay = 0f;

    [Header("Boundary Settings")]
    [Tooltip("The X-coordinate defining the left boundary of the loop.")]
    public float leftBoundaryX = -10.0f;
    [Tooltip("The X-coordinate defining the right boundary of the loop.")]
    public float rightBoundaryX = 10.0f;

    [Header("Tutorial Trigger (Optional)")]
    [Tooltip("If checked, this object will only start moving after the specified Tutorial Item is clicked/completed or skipped.")]
    public bool triggerAfterTutorial = false;
    [Tooltip("Assign the TutorialItem this movement should wait for.")]
    public TutorialItem waitForTutorialItem;

    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;
    private Vector3 initialLocalScale;

    private Tween currentMoveTween;
    private Coroutine animationCoroutine;

    void Awake()
    {
        initialLocalPosition = transform.localPosition;
        initialLocalRotation = transform.localRotation;
        initialLocalScale = transform.localScale;

        if (moveSpeed <= 0)
        {
            Debug.LogWarning($"bb_LoopMoveObj on {gameObject.name}: moveSpeed is zero or negative. Defaulting to a very small value.");
            moveSpeed = 0.001f;
        }
        if (leftBoundaryX >= rightBoundaryX)
        {
            Debug.LogError($"bb_LoopMoveObj on {gameObject.name}: leftBoundaryX must be less than rightBoundaryX. Movement may be erratic.");
        }
    }

    void OnEnable()
    {
        transform.localPosition = initialLocalPosition;
        transform.localRotation = initialLocalRotation;
        transform.localScale = initialLocalScale;

        KillCurrentTween(); // Kills DOTween animation

        if (animationCoroutine != null) // Stops the waiting/delay coroutine
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }

        if (triggerAfterTutorial && waitForTutorialItem != null)
        {
            animationCoroutine = StartCoroutine(WaitForTutorialAndStartMoving());
        }
        else
        {
            if (delay > 0)
            {
                animationCoroutine = StartCoroutine(DelayedStartMoving(delay));
            }
            else
            {
                InitiateMovement();
            }
        }
    }

    void OnDisable()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
        KillCurrentTween(); // Kills DOTween animation

        // Reset to initial state
        transform.localPosition = initialLocalPosition;
        transform.localRotation = initialLocalRotation;
        transform.localScale = initialLocalScale;
    }

    void KillCurrentTween()
    {
        if (currentMoveTween != null && currentMoveTween.IsActive())
        {
            currentMoveTween.Kill(false);
        }
        currentMoveTween = null;
    }

    private IEnumerator DelayedStartMoving(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        InitiateMovement();
    }

    private IEnumerator WaitForTutorialAndStartMoving()
    {
        yield return null;

        if (waitForTutorialItem == null)
        {
            if (delay > 0) yield return new WaitForSeconds(delay);
            InitiateMovement();
            yield break;
        }

        if (waitForTutorialItem.IsSkipped)
        {
            if (delay > 0) yield return new WaitForSeconds(delay);
            InitiateMovement();
            yield break;
        }

        if (!waitForTutorialItem.IsActive)
        {
            yield return new WaitUntil(() => waitForTutorialItem.IsActive || waitForTutorialItem.IsSkipped);
        }

        if (waitForTutorialItem.IsSkipped)
        {
            if (delay > 0) yield return new WaitForSeconds(delay);
            InitiateMovement();
            yield break;
        }

        yield return new WaitUntil(() => !waitForTutorialItem.IsActive || waitForTutorialItem.IsSkipped);

        if (delay > 0) yield return new WaitForSeconds(delay);
        InitiateMovement();
    }

    // Renamed StartMoving to InitiateMovement to avoid conflict with coroutine name pattern
    void InitiateMovement()
    {
        if (moveSpeed <= 0.0001f || leftBoundaryX >= rightBoundaryX) return;

        // Ensure object is at its initial X before calculating first move,
        // especially important if there was a delay.
        Vector3 currentPos = transform.localPosition;
        currentPos.x = initialLocalPosition.x; // Start from initial X for first calculation
        transform.localPosition = currentPos;

        float currentX = transform.localPosition.x;
        float targetX;
        float distanceToTarget;

        if (direction == MovementDirection.LeftToRight)
        {
            targetX = rightBoundaryX;
            distanceToTarget = targetX - currentX;

            if (currentX >= rightBoundaryX || distanceToTarget <= 0) // If at or past target, or initial calc is non-positive
            {
                TeleportAndContinueLooping(leftBoundaryX, rightBoundaryX);
                return;
            }
        }
        else // RightToLeft
        {
            targetX = leftBoundaryX;
            distanceToTarget = currentX - targetX;

            if (currentX <= leftBoundaryX || distanceToTarget <= 0) // If at or past target, or initial calc is non-positive
            {
                TeleportAndContinueLooping(rightBoundaryX, leftBoundaryX);
                return;
            }
        }

        float duration = Mathf.Abs(distanceToTarget) / moveSpeed;

        KillCurrentTween(); // Ensure no old tween is running
        currentMoveTween = transform.DOLocalMoveX(targetX, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                if (direction == MovementDirection.LeftToRight)
                {
                    TeleportAndContinueLooping(leftBoundaryX, rightBoundaryX);
                }
                else // RightToLeft
                {
                    TeleportAndContinueLooping(rightBoundaryX, leftBoundaryX);
                }
            });
    }

    void TeleportAndContinueLooping(float teleportToX, float loopTargetX)
    {
        Vector3 newPos = transform.localPosition;
        newPos.x = teleportToX;
        transform.localPosition = newPos;

        float distanceForLoop = Mathf.Abs(loopTargetX - teleportToX);
        if (distanceForLoop <= 0.0001f)
        {
            Debug.LogWarning($"bb_LoopMoveObj on {gameObject.name}: Loop distance is effectively zero. Halting loop.");
            return;
        }

        float durationForLoop = distanceForLoop / moveSpeed;

        KillCurrentTween(); // Ensure no old tween is running
        currentMoveTween = transform.DOLocalMoveX(loopTargetX, durationForLoop)
            .SetEase(Ease.Linear)
            .OnComplete(() => TeleportAndContinueLooping(teleportToX, loopTargetX));
    }
}