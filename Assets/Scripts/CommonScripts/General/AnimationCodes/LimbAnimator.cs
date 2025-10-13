using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
// Bu script, karakterin uzuvlarini (kafa, kol, bacak, ayak) tıklamayla hareket ettiren bir animasyon yöneticisidir.

public class LimbAnimator : MonoBehaviour
{
    [Header("Body Parts (opsiyonel)")]
    public Transform head;       // Kafa
    public Transform armLeft;    // Sol kol
    public Transform armRight;   // Sag kol
    public Transform legLeft;    // Sol bacak
    public Transform legRight;   // Sag bacak
    public Transform footLeft;   // Sol ayak
    public Transform footRight;  // Sag ayak

    [Header("Swing Settings")]
    public float swingAngle = 12f;     // Kollar, bacaklar icin aci
    public float swingTime = 0.3f;     // Gidis-donus suresi
    public Ease swingEase = Ease.InOutSine;

    [Header("Head Shake Settings")]
    public float headShakeAngle = 6f;  // Kafa sallama acisi
    public float headShakeTime = 0.25f;

    // Baslangic rotasyonlari
    private Vector3 headStartRot;
    private Vector3 armLeftStartRot;
    private Vector3 armRightStartRot;
    private Vector3 legLeftStartRot;
    private Vector3 legRightStartRot;
    private Vector3 footLeftStartRot;
    private Vector3 footRightStartRot;

    private void Start()
    {
        if (head != null) headStartRot = head.localEulerAngles;
        if (armLeft != null) armLeftStartRot = armLeft.localEulerAngles;
        if (armRight != null) armRightStartRot = armRight.localEulerAngles;
        if (legLeft != null) legLeftStartRot = legLeft.localEulerAngles;
        if (legRight != null) legRightStartRot = legRight.localEulerAngles;
        if (footLeft != null) footLeftStartRot = footLeft.localEulerAngles;
        if (footRight != null) footRightStartRot = footRight.localEulerAngles;
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DOTween.Kill(transform);

            // Kafa sallama
            if (head != null && !DOTween.IsTweening(head))
            {
                head.DOLocalRotate(new Vector3(0, 0, headShakeAngle), headShakeTime)
                    .SetEase(swingEase)
                    .SetLoops(-1, LoopType.Yoyo);
            }

            // Kollar
            if (armLeft != null && !DOTween.IsTweening(armLeft))
            {
                armLeft.DOLocalRotate(new Vector3(0, 0, swingAngle), swingTime)
                    .SetEase(swingEase)
                    .SetLoops(-1, LoopType.Yoyo);
            }
            if (armRight != null && !DOTween.IsTweening(armRight))
            {
                armRight.DOLocalRotate(new Vector3(0, 0, -swingAngle), swingTime)
                    .SetEase(swingEase)
                    .SetLoops(-1, LoopType.Yoyo);
            }

            // Bacaklar
            if (legLeft != null && !DOTween.IsTweening(legLeft))
            {
                legLeft.DOLocalRotate(new Vector3(0, 0, -swingAngle), swingTime)
                    .SetEase(swingEase)
                    .SetLoops(-1, LoopType.Yoyo);
            }
            if (legRight != null && !DOTween.IsTweening(legRight))
            {
                legRight.DOLocalRotate(new Vector3(0, 0, swingAngle), swingTime)
                    .SetEase(swingEase)
                    .SetLoops(-1, LoopType.Yoyo);
            }

            // Ayaklar
            if (footLeft != null && !DOTween.IsTweening(footLeft))
            {
                footLeft.DOLocalRotate(new Vector3(0, 0, swingAngle * 0.5f), swingTime)
                    .SetEase(swingEase)
                    .SetLoops(-1, LoopType.Yoyo);
            }
            if (footRight != null && !DOTween.IsTweening(footRight))
            {
                footRight.DOLocalRotate(new Vector3(0, 0, -swingAngle * 0.5f), swingTime)
                    .SetEase(swingEase)
                    .SetLoops(-1, LoopType.Yoyo);
            }
        }
    }

    private void OnDisable()
    {
        DOTween.Kill(transform);

        if (head != null)
        {
            head.DOKill();
            head.localEulerAngles = headStartRot;
        }
        if (armLeft != null)
        {
            armLeft.DOKill();
            armLeft.localEulerAngles = armLeftStartRot;
        }
        if (armRight != null)
        {
            armRight.DOKill();
            armRight.localEulerAngles = armRightStartRot;
        }
        if (legLeft != null)
        {
            legLeft.DOKill();
            legLeft.localEulerAngles = legLeftStartRot;
        }
        if (legRight != null)
        {
            legRight.DOKill();
            legRight.localEulerAngles = legRightStartRot;
        }
        if (footLeft != null)
        {
            footLeft.DOKill();
            footLeft.localEulerAngles = footLeftStartRot;
        }
        if (footRight != null)
        {
            footRight.DOKill();
            footRight.localEulerAngles = footRightStartRot;
        }
    }
}
