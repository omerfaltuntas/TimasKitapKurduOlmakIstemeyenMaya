using UnityEngine;
using DG.Tweening;
public class WordPopIn : MonoBehaviour
{
    [Header("Harfler")]
    public Transform[] letters;

    [Header("Ayarlamalar")]
    public float delayBetween = 0.15f;         // Harfler arasi gecikme
    public float scaleTime = 0.4f;             // Her harfin buyume suresi
    public float startDelay = 0.5f;            // Sayfa acildiktan sonra gecikme
    public Ease ease = Ease.OutBack;           // Scale easing

    private void OnEnable()
    {
        // Tum harfleri gizle (scale 0)
        foreach (var letter in letters)
            letter.localScale = Vector3.zero;

        // 0.5 saniye sonra animasyonu baslat
        DOVirtual.DelayedCall(startDelay, () =>
        {
            for (int i = 0; i < letters.Length; i++)
            {
                letters[i].DOScale(Vector3.one, scaleTime)
                          .SetEase(ease)
                          .SetDelay(i * delayBetween);
            }
        });
    }
}

