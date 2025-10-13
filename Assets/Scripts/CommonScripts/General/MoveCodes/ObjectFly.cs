using UnityEngine;
using DG.Tweening;
using System.Collections;
//Objenin (Kus sinek ari vs. ) ihtimal vererek sahne dısına cıkarak ucurma kodudur.

public class ObjectFly : MonoBehaviour
{
    public enum RotationStyle { FreeRotate, FlipOnXAxis }
    private enum FlightState { Idle, WaitingForTutorial, Flying }

    [Header("Flight Zone & Behavior")]
    public Camera boundaryCamera;
    public float offscreenMargin = 2.0f;
    [Range(0, 1)]
    public float chanceToFlyOffscreen = 0.25f;
    public float minSpeed = 2.0f;
    public float maxSpeed = 4.0f;
    public float minOnscreenPause = 1.0f;
    public float maxOnscreenPause = 3.0f;
    public float minOffscreenPause = 2.0f;
    public float maxOffscreenPause = 5.0f;

    [Header("Rotation")]
    public RotationStyle rotationStyle = RotationStyle.FlipOnXAxis;
    public float turnDuration = 0.2f;
    public Vector2 spriteForwardDirection = Vector2.right;

    [Header("Wing Animation")]
    public bool animateWings = true;
    public Transform leftWing;
    public Transform rightWing;
    public float wingFlapAngle = 20f;
    public float wingFlapSpeed = 15f;
    public bool pauseFlappingOnGlide = true;

    [Header("Start Control")]
    public float initialDelay = 0.5f;

    [Header("Tutorial Trigger (Optional)")]
    public bool triggerAfterTutorial = false;
    public TutorialItem waitForTutorialItem;

    // --- Private Fields ---
    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;
    private Vector3 initialLocalScale;
    private Quaternion rotationCorrection;
    private Transform parentTransform;

    private FlightState currentState = FlightState.Idle;
    private Coroutine controlCoroutine;
    private Sequence wingFlapSequence;
    private Sequence currentFlightSequence;

    void Awake()
    {
        initialLocalPosition = transform.localPosition;
        initialLocalRotation = transform.localRotation;
        initialLocalScale = transform.localScale;

        parentTransform = transform.parent;

        if (boundaryCamera == null)
        {
            boundaryCamera = Camera.main;
            if (boundaryCamera == null) { enabled = false; return; }
        }

        rotationCorrection = Quaternion.FromToRotation(spriteForwardDirection, Vector2.up);
    }

    void OnEnable()
    {
        // --- THE DEFINITIVE RESET ---
        // 1. Force stop all previous activity.
        StopAllCoroutines();
        KillAllTweens();

        // 2. Force reset transform to its initial state.
        transform.localPosition = initialLocalPosition;
        transform.localRotation = initialLocalRotation;
        transform.localScale = initialLocalScale;

        // 3. Reset the state machine.
        currentState = FlightState.Idle;

        // 4. Begin the startup logic.
        if (triggerAfterTutorial && waitForTutorialItem != null)
        {
            currentState = FlightState.WaitingForTutorial;
            controlCoroutine = StartCoroutine(WaitForTutorialAndStartFlight());
        }
        else
        {
            // This will become the flight coroutine.
            controlCoroutine = StartCoroutine(DelayedStartFlight(initialDelay));
        }
    }

    void OnDisable()
    {
        // On disable, perform a full, aggressive cleanup.
        StopAllCoroutines();
        KillAllTweens();

        // Also reset the transform state just in case.
        if (this != null && this.gameObject != null)
        {
            transform.localPosition = initialLocalPosition;
            transform.localRotation = initialLocalRotation;
            transform.localScale = initialLocalScale;
        }
    }

    void KillAllTweens()
    {
        // This is more robust. It kills specific sequences and then any remaining tweens on the transform.
        if (wingFlapSequence != null && wingFlapSequence.IsActive()) wingFlapSequence.Kill(false);
        if (currentFlightSequence != null && currentFlightSequence.IsActive()) currentFlightSequence.Kill(false);
        transform.DOKill(); // This is a crucial catch-all
        wingFlapSequence = null;
        currentFlightSequence = null;
    }

    private IEnumerator DelayedStartFlight(float waitTime)
    {
        currentState = FlightState.WaitingForTutorial; // Technically waiting for delay
        if (waitTime > 0) yield return new WaitForSeconds(waitTime);
        StartFlight();
    }

    private IEnumerator WaitForTutorialAndStartFlight()
    {
        yield return null;
        if (waitForTutorialItem == null || waitForTutorialItem.IsSkipped)
        {
            yield return StartCoroutine(DelayedStartFlight(initialDelay));
            yield break;
        }
        if (!waitForTutorialItem.IsActive)
        {
            yield return new WaitUntil(() => waitForTutorialItem.IsActive || waitForTutorialItem.IsSkipped);
        }
        if (waitForTutorialItem.IsSkipped)
        {
            yield return StartCoroutine(DelayedStartFlight(initialDelay));
            yield break;
        }
        yield return new WaitUntil(() => !waitForTutorialItem.IsActive || waitForTutorialItem.IsSkipped);
        yield return StartCoroutine(DelayedStartFlight(initialDelay));
    }

    void StartFlight()
    {
        // Ensure we don't start multiple flight loops.
        if (currentState == FlightState.Flying) return;
        currentState = FlightState.Flying;

        // The 'controlCoroutine' has finished its job of waiting.
        // Now, we start the actual flight coroutine.
        controlCoroutine = StartCoroutine(FlightLoop());
    }

    void ControlWingAnimation(bool shouldFlap)
    {
        if (!animateWings || leftWing == null || rightWing == null) return;
        if (shouldFlap)
        {
            if (wingFlapSequence == null || !wingFlapSequence.IsActive() || !wingFlapSequence.IsPlaying())
            {
                if (wingFlapSequence == null || !wingFlapSequence.IsActive())
                {
                    wingFlapSequence = DOTween.Sequence();
                    float flapDuration = 1f / wingFlapSpeed;
                    var leftWingTween = leftWing.DOLocalRotate(new Vector3(0, 0, wingFlapAngle), flapDuration).SetLoops(-1, LoopType.Yoyo);
                    var rightWingTween = rightWing.DOLocalRotate(new Vector3(0, 0, -wingFlapAngle), flapDuration).SetLoops(-1, LoopType.Yoyo);
                    wingFlapSequence.Append(leftWingTween).Join(rightWingTween);
                }
                wingFlapSequence.Play();
            }
        }
        else
        {
            if (wingFlapSequence != null && wingFlapSequence.IsPlaying())
            {
                wingFlapSequence.Pause();
                leftWing.DOLocalRotate(Vector3.zero, 0.2f);
                rightWing.DOLocalRotate(Vector3.zero, 0.2f);
            }
        }
    }

    IEnumerator FlightLoop()
    {
        if (animateWings) ControlWingAnimation(true); // Start flapping as we begin the loop

        while (true)
        {
            Vector3 worldDestination;
            bool goingOffscreen = Random.value < chanceToFlyOffscreen;

            if (goingOffscreen) { worldDestination = GetRandomPointOffscreen(); }
            else { worldDestination = GetRandomPointInView(); }

            Vector3 localDestination = parentTransform != null ? parentTransform.InverseTransformPoint(worldDestination) : worldDestination;
            float distance = Vector3.Distance(transform.localPosition, localDestination);
            float speed = Random.Range(minSpeed, maxSpeed);
            float moveDuration = distance / speed;
            Vector3 direction = (localDestination - transform.localPosition).normalized;

            if (currentFlightSequence != null && currentFlightSequence.IsActive()) currentFlightSequence.Kill();
            currentFlightSequence = DOTween.Sequence();

            if (rotationStyle == RotationStyle.FlipOnXAxis)
            {
                float targetXScale = initialLocalScale.x;
                if (spriteForwardDirection.x > 0)
                {
                    if (direction.x < -0.1f) targetXScale = -initialLocalScale.x;
                    else if (direction.x > 0.1f) targetXScale = initialLocalScale.x;
                }
                else
                {
                    if (direction.x > 0.1f) targetXScale = -initialLocalScale.x;
                    else if (direction.x < -0.1f) targetXScale = initialLocalScale.x;
                }
                var flipTween = transform.DOScaleX(targetXScale, turnDuration);
                currentFlightSequence.Insert(0, flipTween);
            }
            else
            {
                Quaternion targetRotation = initialLocalRotation;
                if (direction != Vector3.zero)
                {
                    Quaternion flightRotation = Quaternion.LookRotation(Vector3.forward, direction);
                    targetRotation = flightRotation * rotationCorrection;
                }
                var rotateTween = transform.DORotateQuaternion(targetRotation, turnDuration);
                currentFlightSequence.Insert(0, rotateTween);
            }

            var moveTween = transform.DOLocalMove(localDestination, moveDuration).SetEase(Ease.InOutSine);
            currentFlightSequence.Insert(0, moveTween);

            if (pauseFlappingOnGlide && moveDuration > 1.5f)
            {
                DOVirtual.DelayedCall(0.5f, () => ControlWingAnimation(false), false);
            }
            else
            {
                ControlWingAnimation(true);
            }

            yield return currentFlightSequence.WaitForCompletion();

            if (goingOffscreen)
            {
                float offscreenPause = Random.Range(minOffscreenPause, maxOffscreenPause);
                yield return new WaitForSeconds(offscreenPause);
                Vector3 nextWorldPos = GetRandomPointOffscreen();
                transform.localPosition = parentTransform != null ? parentTransform.InverseTransformPoint(nextWorldPos) : nextWorldPos;
            }
            else
            {
                ControlWingAnimation(false);
                float onscreenPause = Random.Range(minOnscreenPause, maxOnscreenPause);
                yield return new WaitForSeconds(onscreenPause);
            }
        }
    }
    Vector3 GetRandomPointInView()
    {
        Vector3 viewportPoint = new Vector3(Random.value, Random.value, Mathf.Abs(transform.position.z - boundaryCamera.transform.position.z));
        return boundaryCamera.ViewportToWorldPoint(viewportPoint);
    }

    Vector3 GetRandomPointOffscreen()
    {
        int edge = Random.Range(0, 4);
        Vector3 viewportPoint = Vector3.zero;
        switch (edge)
        {
            case 0: viewportPoint = new Vector3(-offscreenMargin, Random.value, 0); break;
            case 1: viewportPoint = new Vector3(1 + offscreenMargin, Random.value, 0); break;
            case 2: viewportPoint = new Vector3(Random.value, 1 + offscreenMargin, 0); break;
            case 3: viewportPoint = new Vector3(Random.value, -offscreenMargin, 0); break;
        }
        viewportPoint.z = Mathf.Abs(transform.position.z - boundaryCamera.transform.position.z);
        return boundaryCamera.ViewportToWorldPoint(viewportPoint);
    }
}