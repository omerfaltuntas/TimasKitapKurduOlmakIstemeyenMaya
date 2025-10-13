using UnityEngine;
using DG.Tweening;

//Bu kod objenin sallanmasini tıklamayla veya otomatik olarak yapan koddur.
public class PlantSwing : MonoBehaviour
{
    public float aci = 10f;
    public float sure = 0.6f;

    [Header("True ise otomatik baslar, False ise tiklamayla")]
    public bool autoPlay = false;

    private Quaternion ilkRot;
    private bool oynuyor = false;

    private void Awake()
    {
        ilkRot = transform.localRotation;
    }

    private void OnEnable()
    {
        if (autoPlay && !oynuyor)
        {
            oynuyor = true;

            transform.DOLocalRotate(new Vector3(0, 0, aci), sure)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
    }

    private void OnMouseDown()
    {
        if (!autoPlay && Input.GetMouseButtonDown(0) && !oynuyor)
        {
            oynuyor = true;

            transform.DOLocalRotate(new Vector3(0, 0, aci), sure)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
    }

    private void OnDisable()
    {
        DOTween.Kill(transform);
        transform.localRotation = ilkRot;
        oynuyor = false;
    }
}
