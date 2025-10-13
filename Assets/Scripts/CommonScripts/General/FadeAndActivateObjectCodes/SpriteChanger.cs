using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

//liste içine attıgımız spriteları değiştiren kod. 
public class SpriteChanger : MonoBehaviour
{
    [Header("Sira ile fade yapilacak sprite listesi")]
    public List<SpriteRenderer> spriteList = new List<SpriteRenderer>();

    [Header("Fade ayari")]
    public float fadeDuration = 1f;
    public float delayBetween = 0.5f;

    private List<Vector3> initialPositions = new List<Vector3>();
    private List<Vector3> initialScales = new List<Vector3>();

    private Sequence loopSequence;
    private bool loopStarted = false;

    private void Start()
    {
        if (spriteList == null || spriteList.Count == 0) return;

        foreach (var sr in spriteList)
        {
            if (sr == null) continue;
            initialPositions.Add(sr.transform.localPosition);
            initialScales.Add(sr.transform.localScale);
            SetAlpha(sr, 0f); // hepsi kapali baslar
        }
    }

    private void OnMouseDown()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (loopStarted) return;
        if (spriteList == null || spriteList.Count < 2) return;

        loopStarted = true;

        // Ilk sprite fade-in
        spriteList[0].DOFade(1f, fadeDuration).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            // Loop animasyonu baslat
            loopSequence = DOTween.Sequence();

            for (int i = 0; i < spriteList.Count; i++)
            {
                int current = i;
                int next = (i + 1) % spriteList.Count;

                loopSequence.Append(spriteList[current].DOFade(0f, fadeDuration).SetEase(Ease.InOutSine))
                            .Join(spriteList[next].DOFade(1f, fadeDuration).SetEase(Ease.InOutSine))
                            .AppendInterval(delayBetween);
            }

            loopSequence.SetLoops(-1);
        });
    }

    private void SetAlpha(SpriteRenderer sr, float alpha)
    {
        if (sr == null) return;
        Color c = sr.color;
        c.a = alpha;
        sr.color = c;
    }

    private void OnDisable()
    {
        DOTween.Kill(transform);

        if (loopSequence != null)
            loopSequence.Kill();

        loopStarted = false;

        for (int i = 0; i < spriteList.Count; i++)
        {
            if (spriteList[i] == null) continue;
            spriteList[i].transform.localPosition = initialPositions[i];
            spriteList[i].transform.localScale = initialScales[i];
            SetAlpha(spriteList[i], 0f);
        }
    }
}
