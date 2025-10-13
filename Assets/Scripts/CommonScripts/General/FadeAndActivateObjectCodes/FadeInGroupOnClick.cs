using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
//Bu kod tikladigimiz objenin altinda bulunan parent'taki spriteların grupca fade'inin acilmasini saglar.
public class FadeInGroupOnClick : MonoBehaviour
{
    public Transform targetParent; // SpriteRenderer'lar bu parentin altinda
    public float fadeDuration = 0.4f;

    private List<SpriteRenderer> spriteList = new List<SpriteRenderer>();
    private bool hasFaded = false;

    private void Awake()
    {
        if (targetParent != null)
            spriteList.AddRange(targetParent.GetComponentsInChildren<SpriteRenderer>(true));
    }

    private void OnEnable()
    {
        hasFaded = false;

        foreach (var sr in spriteList)
        {
            if (sr != null)
            {
                Color clr = sr.color;
                clr.a = 0f;
                sr.color = clr;
            }
        }
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0) && !hasFaded)
        {
            hasFaded = true;

            foreach (var sr in spriteList)
            {
                if (sr != null)
                {
                    sr.DOFade(1f, fadeDuration).SetEase(Ease.InOutSine);
                }
            }
        }
    }

    private void OnDisable()
    {
        foreach (var sr in spriteList)
        {
            DOTween.Kill(sr);

            if (sr != null)
            {
                Color clr = sr.color;
                clr.a = 0f;
                sr.color = clr;
            }
        }

        hasFaded = false;
    }
}
