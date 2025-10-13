using UnityEngine;
using DG.Tweening;

public class LogoBouncer : MonoBehaviour
{
    [Header("Ziplama Ayarlari")]
    public float jumpHeight = 0.2f;
    public float jumpDuration = 0.6f;

    [Header("Scale Ayarlari")]
    public Vector3 squashScale = new Vector3(1.2f, 0.8f, 1f);
    public Vector3 stretchScale = new Vector3(0.9f, 1.1f, 1f);
    public float scaleDuration = 0.2f;

    [Header("Bekleme Ayari")]
    public float waitAfterBounce = 1.5f; // ziplama bitince bekleme suresi

    private Vector3 originalPos;
    private Vector3 originalScale;

    private void Start()
    {
        originalPos = transform.localPosition;
        originalScale = transform.localScale;

        StartBounceLoop();
    }

    private void StartBounceLoop()
    {
        Sequence bounce = DOTween.Sequence().SetLoops(-1).SetUpdate(true);

        // Yukari ziplama + esneme
        bounce.Append(transform.DOLocalMoveY(originalPos.y + jumpHeight, jumpDuration / 2f)
            .SetEase(Ease.OutQuad));
        bounce.Join(transform.DOScale(stretchScale, scaleDuration).SetEase(Ease.OutSine));

        // Asagi inis + bastirilma
        bounce.Append(transform.DOLocalMoveY(originalPos.y, jumpDuration / 2f)
            .SetEase(Ease.InQuad));
        bounce.Join(transform.DOScale(squashScale, scaleDuration).SetEase(Ease.InSine));

        // Orijinal hale donus
        bounce.Append(transform.DOScale(originalScale, scaleDuration).SetEase(Ease.OutExpo));

        // Ziplama arasi bekleme
        bounce.AppendInterval(waitAfterBounce);
    }
}
