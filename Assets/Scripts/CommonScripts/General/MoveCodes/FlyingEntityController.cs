using UnityEngine;
using DG.Tweening;
using System.Collections;
//Objeyi sahne dışına cıkmadan dolastırma kodu.

public class FlyingEntityController : MonoBehaviour
{
    [Header("Flight Zone & Behavior")]
    public Camera boundaryCamera;
    [Range(0, 0.5f)]
    public float screenPadding = 0.1f;
    public float minSpeed = 1.0f;
    public float maxSpeed = 3.0f;
    public float minPauseDuration = 0.5f;
    public float maxPauseDuration = 2.0f;

    [Header("Rotation")]
    [Tooltip("If checked, the object will only flip horizontally on the X-axis.")]
    public bool useXAxisFlipOnly = true;
    [Tooltip("How long the object takes to turn.")]
    public float turnDuration = 0.2f;
    [Tooltip("The direction the sprite is facing in its default pose. (1,0) for Right, (-1,0) for Left.")]
    public Vector2 spriteForwardDirection = Vector2.right;

    [Header("Wing Animation")]
    public bool animateWings = true;
    public Transform leftWing;
    public Transform rightWing;
    public float wingFlapAngle = 45f;
    public float wingFlapSpeed = 10f;

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

    private Coroutine controlCoroutine;
    private Coroutine flightCoroutine;
    private Sequence wingFlapSequence;
    private Sequence currentFlightSequence;

    void Awake()
    {
        initialLocalPosition = transform.localPosition;
        initialLocalRotation = transform.localRotation;
        initialLocalScale = transform.localScale;

        if (boundaryCamera == null)
        {
            boundaryCamera = Camera.main;
            if (boundaryCamera == null) { enabled = false; return; }
        }

        // Normalize the forward direction to be safe
        spriteForwardDirection.Normalize();

        rotationCorrection = Quaternion.FromToRotation(spriteForwardDirection, Vector2.up);
        turnDuration = Mathf.Max(0.01f, turnDuration);
    }

    void OnEnable()
    {
        // ... (OnEnable remains the same)
        StopAllCoroutines();
        KillAllTweens();

        transform.localPosition = initialLocalPosition;
        transform.localRotation = initialLocalRotation;
        transform.localScale = initialLocalScale;

        if (triggerAfterTutorial && waitForTutorialItem != null)
        {
            controlCoroutine = StartCoroutine(WaitForTutorialAndStartFlight());
        }
        else
        {
            controlCoroutine = StartCoroutine(DelayedStartFlight(initialDelay));
        }
    }

    void OnDisable()
    {
        // ... (OnDisable remains the same)
        StopAllCoroutines();
        KillAllTweens();

        transform.localPosition = initialLocalPosition;
        transform.localRotation = initialLocalRotation;
        transform.localScale = initialLocalScale;
    }

    void StopAllCoroutines()
    {
        if (controlCoroutine != null) { StopCoroutine(controlCoroutine); controlCoroutine = null; }
        if (flightCoroutine != null) { StopCoroutine(flightCoroutine); flightCoroutine = null; }
    }

    void KillAllTweens()
    {
        if (wingFlapSequence != null && wingFlapSequence.IsActive()) wingFlapSequence.Kill(false);
        if (currentFlightSequence != null && currentFlightSequence.IsActive()) currentFlightSequence.Kill(false);
        wingFlapSequence = null;
        currentFlightSequence = null;
        transform.DOKill();
    }

    private IEnumerator DelayedStartFlight(float waitTime)
    {
        if (waitTime > 0) yield return new WaitForSeconds(waitTime);
        StartFlight();
    }

    private IEnumerator WaitForTutorialAndStartFlight()
    {
        // ... (WaitForTutorialAndStartFlight remains the same)
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
        if (animateWings && leftWing != null && rightWing != null)
        {
            StartWingAnimation();
        }
        if (flightCoroutine != null) StopCoroutine(flightCoroutine);
        flightCoroutine = StartCoroutine(FlightLoop());
    }

    void StartWingAnimation()
    {
        // ... (StartWingAnimation remains the same)
        if (wingFlapSequence != null && wingFlapSequence.IsActive()) wingFlapSequence.Kill();

        wingFlapSequence = DOTween.Sequence();
        float flapDuration = 1f / wingFlapSpeed;

        var leftWingTween = leftWing.DOLocalRotate(new Vector3(0, wingFlapAngle, 0), flapDuration).SetLoops(-1, LoopType.Yoyo);
        var rightWingTween = rightWing.DOLocalRotate(new Vector3(0, -wingFlapAngle, 0), flapDuration).SetLoops(-1, LoopType.Yoyo);

        wingFlapSequence.Append(leftWingTween).Join(rightWingTween);
    }

    IEnumerator FlightLoop()
    {
        while (true)
        {
            Vector3 randomDestination = GetRandomPointInView();

            if (currentFlightSequence != null && currentFlightSequence.IsActive()) currentFlightSequence.Kill();
            currentFlightSequence = DOTween.Sequence();

            float distance = Vector3.Distance(transform.position, randomDestination);
            float speed = Random.Range(minSpeed, maxSpeed);
            float moveDuration = distance / speed;

            Vector3 direction = (randomDestination - transform.position).normalized;

            if (useXAxisFlipOnly)
            {
                // --- THE FIX IS HERE ---
                // This logic now correctly uses the spriteForwardDirection
                float targetXScale = transform.localScale.x; // Start with current scale

                // Determine the sign of the sprite's intended forward direction (1 for right, -1 for left)
                float forwardSign = Mathf.Sign(spriteForwardDirection.x);

                // Logic: If the sprite needs to move left (direction.x < 0) but is designed to face right (forwardSign > 0), it must flip.
                // Or, if it needs to move right (direction.x > 0) but is designed to face left (forwardSign < 0), it must also flip.
                if ((direction.x < 0 && forwardSign > 0) || (direction.x > 0 && forwardSign < 0))
                {
                    // Flip to the opposite of the initial scale
                    targetXScale = -initialLocalScale.x;
                }
                else
                {
                    // Stay at the default initial scale
                    targetXScale = initialLocalScale.x;
                }

                // Only create the flip animation if the scale actually needs to change
                if (!Mathf.Approximately(transform.localScale.x, targetXScale))
                {
                    var flipTween = transform.DOScaleX(targetXScale, turnDuration).SetEase(Ease.OutSine);
                    currentFlightSequence.Insert(0, flipTween);
                }
            }
            else // Original free rotation logic
            {
                Quaternion targetRotation = initialLocalRotation;
                if (direction != Vector3.zero)
                {
                    Quaternion flightRotation = Quaternion.LookRotation(Vector3.forward, direction);
                    targetRotation = flightRotation * rotationCorrection;
                }
                var rotateTween = transform.DORotateQuaternion(targetRotation, this.turnDuration).SetEase(Ease.InOutSine);
                currentFlightSequence.Insert(0, rotateTween);
            }

            var moveTween = transform.DOLocalMove(randomDestination, moveDuration).SetEase(Ease.Linear);
            currentFlightSequence.Insert(0, moveTween);

            yield return currentFlightSequence.WaitForCompletion();

            float pauseDuration = Random.Range(minPauseDuration, maxPauseDuration);
            yield return new WaitForSeconds(pauseDuration);
        }
    }

    Vector3 GetRandomPointInView()
    {
        // ... (GetRandomPointInView remains the same)
        float randomX = Random.Range(screenPadding, 1 - screenPadding);
        float randomY = Random.Range(screenPadding, 1 - screenPadding);
        Vector3 viewportPoint = new Vector3(randomX, randomY, Mathf.Abs(transform.position.z - boundaryCamera.transform.position.z));
        return boundaryCamera.ViewportToWorldPoint(viewportPoint);
    }
}