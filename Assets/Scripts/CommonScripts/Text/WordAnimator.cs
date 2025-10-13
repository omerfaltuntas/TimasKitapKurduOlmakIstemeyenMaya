using UnityEngine;
using DG.Tweening;

public class WordAnimator : MonoBehaviour
{
    [Header("Harf Objeleri")]
    public Transform[] letters;

    [Header("Animasyon Ayarlari")]
    public float startDelay = 0.5f; // Baslangicta toplam bekleme
    public float delayBetweenLetters = 0.2f;
    public float scaleDuration = 0.5f;
    public Ease scaleEase = Ease.OutBounce;

    private void OnEnable()
    {
        // Tum harfleri sahneye gelir gelmez gorunmez yap
        foreach (Transform harf in letters)
        {
            if (harf == null) continue;
            harf.localScale = Vector3.zero;
        }

        // Animasyonu gecikmeli baslat
        Invoke(nameof(AnimateLettersSafely), startDelay);
    }

    private void AnimateLettersSafely()
    {
        if (letters == null || letters.Length == 0)
            return;

        for (int i = 0; i < letters.Length; i++)
        {
            Transform harf = letters[i];
            if (harf == null) continue;

            // Onceki animasyonu temizle
            harf.DOKill();

            // Yeni animasyonu baslat
            harf.DOScale(Vector3.one, scaleDuration)
                .SetEase(scaleEase)
                .SetDelay(i * delayBetweenLetters)
                .SetUpdate(true); // timescale'dan bagimsiz calisir
        }
    }
}
