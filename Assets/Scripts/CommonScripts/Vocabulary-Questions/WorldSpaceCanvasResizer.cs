using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class WorldSpaceCanvasResizer : MonoBehaviour
{
    [Tooltip("Referans alınacak diğer Canvas (Screen Space - Camera Canvas)")]
    public RectTransform referenceCanvas;

    void Start()
    {
        StartCoroutine(ApplyWithDelay());
    }

    IEnumerator ApplyWithDelay()
    {
        yield return null; // layout otursun
        ApplyCanvasProperties();
    }

    void ApplyCanvasProperties()
    {
        if (referenceCanvas == null)
        {
            Debug.LogWarning("Reference Canvas atanmamış!");
            return;
        }

        RectTransform myRect = GetComponent<RectTransform>();

        // Boyutu eşitle
        myRect.sizeDelta = referenceCanvas.sizeDelta;

        // Pivot ve Anchor'ları eşitle
        myRect.pivot = referenceCanvas.pivot;
        myRect.anchorMin = referenceCanvas.anchorMin;
        myRect.anchorMax = referenceCanvas.anchorMax;

        // Ölçek faktörünü al ve uygula
        myRect.localScale = referenceCanvas.lossyScale;

    }
}
