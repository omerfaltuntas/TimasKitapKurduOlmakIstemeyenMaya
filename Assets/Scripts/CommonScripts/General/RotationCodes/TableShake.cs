using UnityEngine;
using DG.Tweening;
//tıkladıgımızda objenin sallanmasını saglayan kod.

public class TableShake : MonoBehaviour
{
    private Quaternion orijinalRotasyon;
    private bool animasyonBitmedi = false;

    private void Start()
    {
        // Baslangicta tablo rotasyonunu kaydet
        orijinalRotasyon = transform.localRotation;
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0) && !animasyonBitmedi)
        {
            animasyonBitmedi = true;
            DOTween.Kill(transform);

            // Sallanma efektini tek seferde yapar
            transform.DOLocalRotate(new Vector3(0f, 0f, 15f), 0.15f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    transform.DOLocalRotate(new Vector3(0f, 0f, -12f), 0.2f)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                    {
                        transform.DOLocalRotate(new Vector3(0f, 0f, 6f), 0.15f)
                        .SetEase(Ease.InOutSine)
                        .OnComplete(() =>
                        {
                            transform.DOLocalRotate(Vector3.zero, 0.1f)
                            .SetEase(Ease.InOutSine)
                            .OnComplete(() =>
                            {
                                animasyonBitmedi = false; // Animasyon tamamlandi
                            });
                        });
                    });
                });
        }
    }

    private void OnDisable()
    {
        DOTween.Kill(transform);
        transform.localRotation = orijinalRotasyon;
        animasyonBitmedi = false;
    }
}
