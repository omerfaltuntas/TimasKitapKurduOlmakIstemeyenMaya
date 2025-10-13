using System.Collections;
using TMPro;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(TMP_Text))]
public class TextCurve : MonoBehaviour
{
    [Header("Egrilik Ayarlari")]
    [Range(-50f, 50f)] public float bendAmount = 10f;
    [Range(0.1f, 5f)] public float horizontalStretch = 1f;

    [Header("Fade Ayarlari")]
    public float fadeDuration = 1f;
    public float fadeDelay = 2f;
    [Range(0f, 1f)] public float currentAlpha = 0f; // Animator alpha
    private Coroutine fadeCoroutine;

    private TMP_Text tmp;

    private void Awake()
    {
        tmp = GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        // Fade'i otomatik baslat
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine(1f, 0f, fadeDuration, fadeDelay));
    }

    void OnDisable()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        currentAlpha = 0f;
        ApplyMesh();
    }

    IEnumerator FadeRoutine(float targetAlpha, float startAlpha, float duration, float delay)
    {
        currentAlpha = startAlpha;
        ApplyMesh();
        yield return null; // Meshin curve ile yamulmasi icin bir frame bekle (gerekirse bir frame daha ekle)
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            ApplyMesh();
            elapsed += Time.deltaTime;
            yield return null;
        }
        currentAlpha = targetAlpha;
        ApplyMesh();
    }

    private void Update()
    {
        ApplyMesh();
    }

    // Mesh'e egrilik ve alpha uygula
    void ApplyMesh()
    {
        if (!tmp) return;

        tmp.ForceMeshUpdate();
        var textInfo = tmp.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int matIndex = charInfo.materialReferenceIndex;
            int vertIndex = charInfo.vertexIndex;

            Vector3[] verts = textInfo.meshInfo[matIndex].vertices;
            Color32[] colors = textInfo.meshInfo[matIndex].colors32;

            Vector3 center = (verts[vertIndex + 0] + verts[vertIndex + 2]) / 2;

            for (int j = 0; j < 4; j++)
                verts[vertIndex + j] -= center;

            float xPosNorm = (center.x - tmp.bounds.min.x) / tmp.bounds.size.x * horizontalStretch;
            float curveY = Mathf.Sin(xPosNorm * Mathf.PI) * bendAmount;
            Vector3 offset = new Vector3(0, curveY, 0);

            for (int j = 0; j < 4; j++)
                verts[vertIndex + j] += center + offset;

            // Her vertexin alpha degerini fade ile ayarla
            for (int j = 0; j < 4; j++)
            {
                var color = tmp.color;
                color.a = currentAlpha;
                colors[vertIndex + j] = color;
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            meshInfo.mesh.colors32 = meshInfo.colors32;
            tmp.UpdateGeometry(meshInfo.mesh, i);
        }
    }

    // Fade'i disaridan elle cagirmak icin
    public void StartFade(float to, float from, float duration, float delay = 0f)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine(to, from, duration, delay));
    }
}
