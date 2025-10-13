using UnityEngine;
using DG.Tweening;
//Bu kod tıkladığımız objeyi atadıgımız hedef noktaya gönderen koddur.

public class ClickToMove : MonoBehaviour
{
    public Transform hedefNokta; // Gidilecek hedef pozisyon
    public float hareketSuresi = 1f;

    private Vector3 baslangicLocalPos;

    private void OnEnable()
    {
        // Baslangic pozisyonunu kaydet
        baslangicLocalPos = transform.localPosition;
    }

    private void OnMouseDown()
    {
        // Tiklama kontrolu
        if (Input.GetMouseButtonDown(0))
        {

            // Hedefe hareket
            transform.DOLocalMove(hedefNokta.localPosition, hareketSuresi).SetEase(Ease.InOutSine);
        }
    }

    private void OnDisable()
    {
        // Tweeni durdur
        DOTween.Kill(transform);

        // Pozisyonu sifirla
        transform.localPosition = baslangicLocalPos;
    }
}
