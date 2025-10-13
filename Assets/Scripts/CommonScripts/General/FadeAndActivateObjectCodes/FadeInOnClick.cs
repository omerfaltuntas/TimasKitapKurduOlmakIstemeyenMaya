using UnityEngine;
using DG.Tweening;
//Bu kod tikladigimiz objenin fade'inin acilmasini saglayan koddur.

public class FadeInOnClick : MonoBehaviour
{
    public SpriteRenderer targetSprite;
    public float fadeDuration = 0.4f;

    private bool hasFaded = false;

    private void OnEnable()
    {
        if (targetSprite != null)
        {
            Color clr = targetSprite.color;
            clr.a = 0f;
            targetSprite.color = clr;
            hasFaded = false;
        }
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0) && !hasFaded)
        {
            hasFaded = true;
            targetSprite.DOFade(1f, fadeDuration).SetEase(Ease.InOutSine);
        }
    }

    private void OnDisable()
    {
        DOTween.Kill(targetSprite);

        if (targetSprite != null)
        {
            Color clr = targetSprite.color;
            clr.a = 0f;
            targetSprite.color = clr;
        }

        hasFaded = false;
    }
}
