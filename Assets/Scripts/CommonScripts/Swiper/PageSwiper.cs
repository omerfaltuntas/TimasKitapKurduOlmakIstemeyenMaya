using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Bu kod sayfası interkatif kitap içeriklerinde sayfalar arası geçişini sağlamaktadır.
/// </summary>

public class PageSwiper : MonoBehaviour
{
    public static PageSwiper Instance;

    [Header("Pages")]
    [Tooltip("Pages objesi altındaki bütün sayfaları sırasına uygun şekilde listeler")]
    public List<GameObject> pages = new List<GameObject>();
    [HideInInspector] public int currentPageIndex = 0;
    private Vector3 mousePos;
    private Vector3 pagePos;
    private bool isDragging;
    private bool isDraggable;

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

    void Update()
    {
        // Sol tıklandı mı kontrol ediliyor
        if (Input.GetMouseButtonDown(0))
            TouchInput();

        // Kaydırma işlevi kontrolcüsü
        DraggingInput();

        // Tıklama/kaydırma işlevininin bitişi
        if (Input.GetMouseButtonUp(0))
            EndDragging();

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

    /// <summary>
    /// Bu alan ilk tıklama olayında sadece sayfa etiketi bulunan objeyi bulmada görev alır.
    /// </summary>
    private void TouchInput()
    {
        // Tıklama noktasından bir ışın oluşturuluyor
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Işınla çarpışan tüm nesneleri bulmak için RaycastAll kullanılıyor
        RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction);

        if (hits.Length == 0)
        {
            Debug.LogError("Sahnede kaydırılacak öğeler bulunamadı!");
            return;
        }

        // İlk çarpışan nesneyi al
        PageTypes firstPageType = hits[0].collider.GetComponent<PageTypes>();

        foreach (RaycastHit2D hit in hits)
        {
            PageTypes pageType = hit.collider.GetComponent<PageTypes>();

            if (pageType == null || firstPageType == null) continue;

            // Sayfa kaydırma tipi aktif mi?
            if (pageType.page == Pages.pages)
                isDraggable = true;

            // İlk çarpışan nesne de aynı tipe sahip mi?
            if (isDraggable && pageType.page == Pages.pages && firstPageType.page == Pages.pages)
            {
                float worldX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
                if (worldX > 3 || worldX < -3)
                {
                    currentPageIndex = pages.IndexOf(hit.collider.gameObject);
                    pagePos = hit.collider.gameObject.transform.position;
                    mousePos = pagePos - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    isDragging = true;
                }
            }

            Debug.Log("Page: " + hit.collider.gameObject.name);
        }
    }

    /// <summary>
    /// Bu alan, kullanıcı kaydırma işlevine başladığında aktif olur.
    /// </summary>
    private void DraggingInput()
    {
        // Kaydrıma aktif değilse çalıştırma
        if (!isDragging) return;

        // Tıklama poziyonunu al
        Vector3 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + mousePos;

        // Sürükleme sınırlarını kontrol et
        touchPos.x = Mathf.Clamp(touchPos.x, currentPageIndex == pages.Count - 1 ? -2f : float.MinValue, currentPageIndex == 0 ? 2f : float.MaxValue);

        // Mevcut sayfanın konumunu güncelle
        pages[currentPageIndex].transform.position = new Vector3(touchPos.x, pagePos.y, pagePos.z);

        // Önceki ve sonraki sayfaları yönet
        UpdateAdjacentPages();
    }

    /// <summary>
    /// Sayfalar arası geçişi sağla
    /// </summary>
    private void UpdateAdjacentPages()
    {
        float pageWidth = pages[currentPageIndex].transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.size.x;

        // Sonraki sayfa kontrolü
        if (currentPageIndex < pages.Count - 1 && pages[currentPageIndex].transform.position.x < 0)
        {
            ActivatePage(currentPageIndex + 1, pages[currentPageIndex].transform.position.x + pageWidth);
        }

        // Önceki sayfa kontrolü
        if (currentPageIndex > 0 && pages[currentPageIndex].transform.position.x > 0)
        {
            ActivatePage(currentPageIndex - 1, pages[currentPageIndex].transform.position.x - pageWidth);
        }
    }

    /// <summary>
    /// İlgili sayfaları aktif et ve konumu ayarla
    /// </summary>
    /// <param name="index"></param>
    /// <param name="xPos"></param>
    private void ActivatePage(int index, float xPos)
    {
        pages[index].SetActive(true);
        pages[index].transform.position = new Vector3(xPos, pages[currentPageIndex].transform.position.y);
    }

    /// <summary>
    /// Bu alan, kullanıcı kaydırma işlemini tamamladığında aktif olmaktadır.
    /// </summary>
    private void EndDragging()
    {
        if (currentPageIndex == -1) return;

        // Sayfa sayısını güncelle
        float threshold = 2f;
        if (pages[currentPageIndex].transform.position.x < -threshold && currentPageIndex < pages.Count - 1)
        {
            currentPageIndex++;
        }
        else if (pages[currentPageIndex].transform.position.x > threshold && currentPageIndex > 0)
        {
            currentPageIndex--;
        }

        // Yumuşak sayfa geçişini sağlama
        AnimatePageTransition();

        // Sürükleme kapat
        isDragging = false;
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

}