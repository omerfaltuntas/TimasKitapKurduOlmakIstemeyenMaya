using UnityEngine;
using DG.Tweening;
//Bu kod karakterlerin kol, bacak, kafa gibi uzuvlarını hareket ettirmeye yarar.
public class CharacterAnimation : MonoBehaviour
{
    [Header("Karakter Parcalari")]
    public Transform head;
    public Transform leftArm;
    public Transform rightArm;
    public Transform leftLeg;
    public Transform rightLeg;

    [Header("Animasyon Ayarlari")]
    public float moveAmount = 30f;
    public float rotateAmount = 15f;
    public float animStepDuration = 0.5f;        // Her adim animasyonu
    public float animTotalDuration = 3f;         // Toplam ne kadar sure animasyon yapacak

    private Vector3 headStartRot;
    private Vector3 leftArmStartRot;
    private Vector3 rightArmStartRot;
    private Vector3 leftLegStartPos;
    private Vector3 rightLegStartPos;

    private void Awake()
    {
        if (head != null) headStartRot = head.localEulerAngles;
        if (leftArm != null) leftArmStartRot = leftArm.localEulerAngles;
        if (rightArm != null) rightArmStartRot = rightArm.localEulerAngles;
        if (leftLeg != null) leftLegStartPos = leftLeg.localPosition;
        if (rightLeg != null) rightLegStartPos = rightLeg.localPosition;
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DOTween.Kill(transform);

            // Kafa animasyonu
            if (head != null)
            {
                head.DOLocalRotate(headStartRot + new Vector3(0, 0, rotateAmount), animStepDuration)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetId(head);
            }

            // Kollar
            if (leftArm != null)
            {
                leftArm.DOLocalRotate(leftArmStartRot + new Vector3(0, 0, -rotateAmount), animStepDuration)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetId(leftArm);
            }

            if (rightArm != null)
            {
                rightArm.DOLocalRotate(rightArmStartRot + new Vector3(0, 0, rotateAmount), animStepDuration)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetId(rightArm);
            }

            // Bacaklar
            if (leftLeg != null)
            {
                leftLeg.DOLocalMoveY(leftLegStartPos.y + moveAmount, animStepDuration)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetId(leftLeg);
            }

            if (rightLeg != null)
            {
                rightLeg.DOLocalMoveY(rightLegStartPos.y + moveAmount, animStepDuration)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetId(rightLeg);
            }

            // Belirtilen sure sonra animasyonu durdur
            DOVirtual.DelayedCall(animTotalDuration, StopAnimation);
        }
    }

    private void StopAnimation()
    {
        if (head != null) DOTween.Kill(head);
        if (leftArm != null) DOTween.Kill(leftArm);
        if (rightArm != null) DOTween.Kill(rightArm);
        if (leftLeg != null) DOTween.Kill(leftLeg);
        if (rightLeg != null) DOTween.Kill(rightLeg);

        // Pozisyonlari eski haline getir
        if (head != null) head.localEulerAngles = headStartRot;
        if (leftArm != null) leftArm.localEulerAngles = leftArmStartRot;
        if (rightArm != null) rightArm.localEulerAngles = rightArmStartRot;
        if (leftLeg != null) leftLeg.localPosition = leftLegStartPos;
        if (rightLeg != null) rightLeg.localPosition = rightLegStartPos;
    }

    private void OnDisable()
    {
        DOTween.Kill(transform);
        StopAnimation();
    }
}
