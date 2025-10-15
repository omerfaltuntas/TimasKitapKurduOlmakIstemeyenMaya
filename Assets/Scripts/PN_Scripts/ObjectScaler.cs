using UnityEngine;
using DG.Tweening;

// Objeye tiklaninca scale degerini 1 yapan script
public class ObjectScaler : MonoBehaviour
{
    [SerializeField] private GameObject targetObj; // Disaridan atanacak obje

    private Vector3 initialScale;   // Baslangictaki scale
    private bool animasyonBitmedi = false; // Animasyon kontrolu

    private void Start()
    {
        if (targetObj == null) return;
        initialScale = targetObj.transform.localScale; // Baslangic scale kaydet
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0) && !animasyonBitmedi && targetObj != null)
        {
            animasyonBitmedi = true;
            DOTween.Kill(targetObj.transform);

            // Objenin scale'ini yumuşak bir sekilde 1 yap
            targetObj.transform.DOScale(Vector3.one, 1f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    animasyonBitmedi = false; // Animasyon tamamlandi
                });
        }
    }

    private void OnDisable()
    {
        if (targetObj == null) return;

        DOTween.Kill(targetObj.transform);
        targetObj.transform.localScale = initialScale; // Scale'i sifirla
        animasyonBitmedi = false;
    }
}