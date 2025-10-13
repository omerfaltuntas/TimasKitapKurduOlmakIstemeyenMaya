using System.Collections;
using UnityEngine;
using DG.Tweening;
//Bu kod objenin yer dusup sekmesini saglayan koddur.
public class ObjectDrop : MonoBehaviour
{
    public SpriteRenderer box1;
    public SpriteRenderer box2;

    public float box1TargetY;
    public float box2TargetY;

    public float delay;
    public float delay2;

    private bool hasDropped = false;

    private Vector3 box1InitialPos;
    private Vector3 box2InitialPos;

    void Start()
    {
        // Kutularin baslangic pozisyonlarini kaydet
        box1InitialPos = box1.transform.localPosition;
        box2InitialPos = box2.transform.localPosition;
    }

    void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {

            if (hasDropped) return;
            hasDropped = true;

            // Ilk kutu animasyonu
            box1.transform.DOLocalMoveY(box1TargetY, delay).SetEase(Ease.OutBounce).OnComplete(() =>
            {
                // Ilk kutudan sonra ikinci kutu animasyonu baslar
                box2.transform.DOLocalMoveY(box2TargetY, delay2).SetEase(Ease.OutBounce);
            });
        }
    }

    void OnDisable()
    {
        // DOTween animasyonlarini iptal et
        DOTween.Kill(box1.transform);
        DOTween.Kill(box2.transform);

        // Kutulari baslangic pozisyonlarina geri al
        box1.transform.localPosition = box1InitialPos;
        box2.transform.localPosition = box2InitialPos;

        hasDropped = false;
    }
}
