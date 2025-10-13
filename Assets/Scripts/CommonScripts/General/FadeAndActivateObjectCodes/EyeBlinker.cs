using UnityEngine;
using DG.Tweening;
//Gozu tek sprite seklinde aç kapa yapan kod.

public class EyeBlinker : MonoBehaviour
{
    [Header("Goz SpriteRenderer")]
    public SpriteRenderer eyeSprite;

    [Header("Kirpma ayarlari")]
    public float blinkDuration = 0.1f;
    public float interval = 1.5f;

    private bool isBlinking = false;
    private Color originalColor;

    private void Awake()
    {
        // Goz baslangicta acik (tam gorunur)
        originalColor = eyeSprite.color;
        Color c = eyeSprite.color;
        c.a = 1f;
        eyeSprite.color = c;
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0) && !isBlinking)
        {
            isBlinking = true;
            InvokeRepeating(nameof(Blink), 0f, interval);
        }
    }

    private void Blink()
    {
        // Alpha 0 ile kapat
        eyeSprite.DOFade(0f, blinkDuration)
            .OnComplete(() =>
            {
                // Alpha 1 ile ac
                eyeSprite.DOFade(1f, blinkDuration);
            });
    }

    private void OnDisable()
    {
        DOTween.Kill(eyeSprite);
        CancelInvoke();
        eyeSprite.color = originalColor;
        isBlinking = false;
    }
}
