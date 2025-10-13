using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
//Atadığımız waypointler üzerinde gidip gelen kod.

public class WithWayPoint : MonoBehaviour
{
    [Header("Maymun Parcalari")]
    public Transform body;
    public Transform arm;
    public Transform leg1;
    public Transform leg2;

    [Header("Hareket Noktalari")]
    public List<Transform> wavePoints = new List<Transform>(); // waypoint objeleri sahneden atanacak
    public float moveDurationPerSegment = 1.5f;

    [Header("Limb Animasyonu")]
    public float limbSwingAngle = 25f;
    public float limbSwingDuration = 0.5f;

    private Vector3 initialBodyPos;
    private Quaternion initialArmRot;
    private Quaternion initialLeg1Rot;
    private Quaternion initialLeg2Rot;

    private bool started = false;
    private Sequence moveSequence;
    private Tween armTween;
    private Tween leg1Tween;
    private Tween leg2Tween;

    private void Start()
    {
        if (body == null || arm == null || leg1 == null || leg2 == null) return;
        if (wavePoints == null || wavePoints.Count < 2) return;

        initialBodyPos = body.localPosition;
        initialArmRot = arm.localRotation;
        initialLeg1Rot = leg1.localRotation;
        initialLeg2Rot = leg2.localRotation;
    }

    private void OnMouseDown()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (started) return;

        started = true;

        // Hareket sekansı
        moveSequence = DOTween.Sequence();

        for (int i = 0; i < wavePoints.Count; i++)
        {
            Transform point = wavePoints[i];
            moveSequence.Append(body.DOLocalMove(point.localPosition, moveDurationPerSegment).SetEase(Ease.InOutSine));
        }

        for (int i = wavePoints.Count - 2; i >= 0; i--) // geri dön
        {
            Transform point = wavePoints[i];
            moveSequence.Append(body.DOLocalMove(point.localPosition, moveDurationPerSegment).SetEase(Ease.InOutSine));
        }

        moveSequence.SetLoops(-1);

        // Kollar-bacaklar loop
        armTween = arm.DOLocalRotate(new Vector3(0, 0, limbSwingAngle), limbSwingDuration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        leg1Tween = leg1.DOLocalRotate(new Vector3(0, 0, -limbSwingAngle), limbSwingDuration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        leg2Tween = leg2.DOLocalRotate(new Vector3(0, 0, limbSwingAngle), limbSwingDuration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDisable()
    {
        if (moveSequence != null) moveSequence.Kill();
        if (armTween != null) armTween.Kill();
        if (leg1Tween != null) leg1Tween.Kill();
        if (leg2Tween != null) leg2Tween.Kill();

        started = false;

        body.localPosition = initialBodyPos;
        arm.localRotation = initialArmRot;
        leg1.localRotation = initialLeg1Rot;
        leg2.localRotation = initialLeg2Rot;
    }

}
