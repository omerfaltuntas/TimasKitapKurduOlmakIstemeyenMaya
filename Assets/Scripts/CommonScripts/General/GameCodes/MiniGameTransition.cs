using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
//Oyun paneline geçmeyi sağlayan kod.

public class MiniGameTransition : MonoBehaviour
{
    [Header("Genel Canvas (kapanacak olan)")]
    public GameObject genelCanvas;

    [Header("Kapatilacak Collider Listesi")]
    public List<BoxCollider2D> collidersToDisable = new List<BoxCollider2D>();

    [Header("Mini Oyun Paneli (acilacak olan)")]
    public GameObject miniOyunPanel;

    [Header("Cikis Objeni Buraya Ata (GameObject)")]
    public GameObject cikisObjesi;

    [Header("Tutorial Objesi (panel acilinca gosterilecek)")]
    public GameObject tutorialObjesi;

    private Vector3 orijinalScale;
    private CanvasGroup canvasGroup;
    private bool acik = false;

    void Start()
    {
        if (miniOyunPanel == null)
        {
            Debug.LogError("❌ miniOyunPanel atanmadı!");
            return;
        }

        orijinalScale = miniOyunPanel.transform.localScale;
        miniOyunPanel.SetActive(false);

        canvasGroup = miniOyunPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = miniOyunPanel.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        if (tutorialObjesi != null) tutorialObjesi.SetActive(false);
    }

    void OnMouseDown()
    {
        if (!(Input.GetMouseButtonDown(0) && !acik)) return;

        if (miniOyunPanel == null || canvasGroup == null || genelCanvas == null)
        {
            Debug.LogWarning("❌ Gerekli referanslardan biri eksik!");
            return;
        }

        acik = true;
        genelCanvas.SetActive(false);

        // listedeki tum collider'lari kapat
        for (int i = 0; i < collidersToDisable.Count; i++)
            if (collidersToDisable[i] != null) collidersToDisable[i].enabled = false;

        miniOyunPanel.SetActive(true);
        miniOyunPanel.transform.localScale = Vector3.zero;
        canvasGroup.alpha = 0f;

        // hedef set -> kill(transform) ile temizlenebilir
        canvasGroup.DOFade(1f, 0.3f).SetTarget(miniOyunPanel.transform);
        miniOyunPanel.transform
            .DOScale(orijinalScale, 0.6f)
            .SetEase(Ease.OutQuad)
            .SetTarget(miniOyunPanel.transform);

        if (tutorialObjesi != null)
        {
            tutorialObjesi.SetActive(false);
            DOVirtual.DelayedCall(2f, () =>
            {
                tutorialObjesi.SetActive(true);
            }).SetTarget(tutorialObjesi.transform);
        }
    }

    void Update()
    {
        if (!acik || cikisObjesi == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == cikisObjesi)
            {
                PaneliKapat();
            }
        }
    }

    void PaneliKapat()
    {
        if (canvasGroup == null || miniOyunPanel == null || genelCanvas == null) return;

        acik = false;

        canvasGroup.DOFade(0f, 0.2f).SetTarget(miniOyunPanel.transform);
        miniOyunPanel.transform
            .DOScale(Vector3.zero, 0.4f)
            .SetEase(Ease.OutQuad)
            .SetTarget(miniOyunPanel.transform)
            .OnComplete(() =>
            {
                miniOyunPanel.SetActive(false);
                genelCanvas.SetActive(true);

                // listedeki tum collider'lari ac
                for (int i = 0; i < collidersToDisable.Count; i++)
                    if (collidersToDisable[i] != null) collidersToDisable[i].enabled = true;

                if (tutorialObjesi != null) tutorialObjesi.SetActive(false);
            });
    }

    void OnDisable()
    {
        if (miniOyunPanel == null || canvasGroup == null || genelCanvas == null) return;

        // sadece transform hedeflerini kill et
        DOTween.Kill(miniOyunPanel.transform);
        if (tutorialObjesi != null) DOTween.Kill(tutorialObjesi.transform);

        miniOyunPanel.transform.localScale = orijinalScale;
        canvasGroup.alpha = 0f;

        miniOyunPanel.SetActive(false);
        genelCanvas.SetActive(true);

        // listedeki tum collider'lari ac
        for (int i = 0; i < collidersToDisable.Count; i++)
            if (collidersToDisable[i] != null) collidersToDisable[i].enabled = true;

        acik = false;
        if (tutorialObjesi != null) tutorialObjesi.SetActive(false);
    }
}
