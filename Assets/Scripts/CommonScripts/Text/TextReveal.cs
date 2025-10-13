using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class TextReveal : MonoBehaviour
{
    [Header("TMP Text listesi (siralama onemli)")]
    public List<TMP_Text> textList = new List<TMP_Text>();

    [Header("Zaman Ayarlari")]
    public float startDelay = 1f;
    private float delayPerLetter = 0.02f;
    private float fadeDuration = 0.001f;
    public float intervalBetweenTexts = 0.5f;
    private Ease fadeEase = Ease.OutQuad;

    [Header("Ses Ayari")]
    public bool enableSound = false;

    private List<Tween> activeTweens = new List<Tween>();

    private void OnEnable()
    {
        foreach (var tmp in textList)
        {
            if (tmp != null)
                tmp.gameObject.SetActive(false);
        }

        StartCoroutine(RevealAllTexts());
    }

    private void OnDisable()
    {
        // Sadece bu objeye ait tweenleri durdur
        foreach (var t in activeTweens)
        {
            if (t.IsActive()) t.Kill();
        }
        activeTweens.Clear();

        foreach (var tmp in textList)
        {
            if (tmp != null)
                tmp.gameObject.SetActive(false);
        }
    }

    private IEnumerator RevealAllTexts()
    {
        yield return new WaitForSeconds(startDelay);

        foreach (var tmp in textList)
        {
            if (tmp == null || string.IsNullOrEmpty(tmp.text)) continue;

            tmp.gameObject.SetActive(true);
            yield return StartCoroutine(RevealOneText(tmp));
            yield return new WaitForSeconds(intervalBetweenTexts);
        }
    }

    private IEnumerator RevealOneText(TMP_Text tmp)
    {
        tmp.ForceMeshUpdate();
        var textInfo = tmp.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int matIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertIndex = textInfo.characterInfo[i].vertexIndex;
            Color32[] newColors = textInfo.meshInfo[matIndex].colors32;

            for (int j = 0; j < 4; j++)
            {
                Color32 c = newColors[vertIndex + j];
                newColors[vertIndex + j] = new Color32(c.r, c.g, c.b, 0);
            }
        }

        tmp.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int matIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertIndex = textInfo.characterInfo[i].vertexIndex;
            Color32[] colors = tmp.textInfo.meshInfo[matIndex].colors32;

            if (enableSound && i % 2 == 0 && Random.value > 0.2f)
            {
                AudioManager.Instance.Play("Daktilo");
            }

            Tween t = DOTween.To(() => 0f, a =>
            {
                byte alpha = (byte)(a * 255);
                for (int j = 0; j < 4; j++)
                {
                    Color32 c = colors[vertIndex + j];
                    colors[vertIndex + j] = new Color32(c.r, c.g, c.b, alpha);
                }
                tmp.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            }, 1f, fadeDuration).SetEase(fadeEase);

            activeTweens.Add(t);

            yield return t.WaitForCompletion();
            yield return new WaitForSeconds(delayPerLetter);
        }
    }
}
