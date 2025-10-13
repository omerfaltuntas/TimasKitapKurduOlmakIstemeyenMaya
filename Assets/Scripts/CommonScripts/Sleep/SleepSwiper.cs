using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// * Uyku modu sayfalarına ait özellikleri yönetir.
/// </summary>

public class SleepSwiper : MonoBehaviour
{
    public static SleepSwiper Instance;

    [Header("Pages")]
    [Tooltip("Pages objesi altındaki bütün sayfaları sırasına uygun şekilde listeler")]
    public List<GameObject> pages = new List<GameObject>();
    [HideInInspector] public int currentPageIndex = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void OnEnable()
    {
        // Başlangıçta sayfaları bul ve ilk sayfayı aç
        InitalizePages();
    }

    /// <summary>
    /// Başlangıçta ilk sayfanın açık olmasını sağlar.
    /// </summary>
    private void InitalizePages()
    {
        // Sadece ilk sayfanın açık olmasını sağla
        foreach (var page in pages) page.SetActive(false);
        pages[currentPageIndex].SetActive(true);
    }

    #region  PAGE TRANSITION

    /// <summary>
    /// Buton ile bir sonraki sayfaya geçiş yapar.
    /// </summary>
    public void NextPage()
    {
        if (currentPageIndex < pages.Count - 1)
        {
            currentPageIndex++;
            AnimatePageTransition();
        }
    }

    /// <summary>
    /// Buton ile bir önceki sayfaya geçiş yapar.
    /// </summary>
    public void PreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            AnimatePageTransition();
        }
    }

    /// <summary>
    /// Animasyonla sayfa geçişini yönet
    /// </summary>
    private void AnimatePageTransition()
    {
        float pageWidth = pages[currentPageIndex].transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.size.x;

        pages[currentPageIndex].transform.SetParent(transform);
        pages[currentPageIndex].SetActive(true);
        MovePage(pages[currentPageIndex], Vector3.zero);

        if (currentPageIndex > 0)
            MovePage(pages[currentPageIndex - 1], new Vector3(-pageWidth, 0, 0));

        if (currentPageIndex < pages.Count - 1)
            MovePage(pages[currentPageIndex + 1], new Vector3(pageWidth, 0, 0));

        DOTween.Sequence().AppendInterval(0.4f).OnComplete(PagesFixer);
    }

    /// <summary>
    ///  Animasyonla sayfa geçişini sağla
    /// </summary>
    /// <param name="page"></param>
    /// <param name="targetPosition"></param>
    private void MovePage(GameObject page, Vector3 targetPosition)
    {
        page.transform.DOMove(targetPosition, 0.3f).SetEase(Ease.Unset);
    }

    /// <summary>
    /// İlgili sayfaları kapat/aç
    /// </summary>
    private void PagesFixer()
    {
        for (int i = 0; i < pages.Count; i++)
            pages[i].SetActive(i == currentPageIndex);
    }

    #endregion
}
