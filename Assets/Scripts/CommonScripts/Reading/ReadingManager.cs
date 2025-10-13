using System.Collections;
using System.Collections.Generic;
using AgeOfKids.Reading;
using UnityEngine;

/// <summary>
/// * Kullanıcı Uygulamanın product kısmından seçtiği moda göre kitap modu değişkliğini sağlar
/// </summary>

public class ReadingManager : MonoSingleton<ReadingManager>
{
    [Header("Data")]
    [SerializeField] private ReadingData readingData;

    [Space(3f)]
    [Header("Settings")]
    [SerializeField] private GameObject interactivePages;
    [SerializeField] private GameObject sleepPages;

    [Header("UI Settings")]
    [SerializeField] private List<GameObject> uiObjects = new List<GameObject>();

    void Start()
    {
        // Kullanıcı kitap okuma mod tercihine göre değişiklikleri uygula
        InitalizeMode();
    }

    private void InitalizeMode()
    {
        ReadingType readingType = readingData.readingType;
        if (readingType == ReadingType.Interactive)
        {
            InteractiveMode();
        }
        else if (readingType == ReadingType.Sleep)
        {
            SleepMode();
        }
    }

    /// <summary>
    /// * İnteraktif mod özelliklerini sağla
    /// </summary>
    private void InteractiveMode()
    {
        // Uyku mod özelliklerini kapat
        var page = sleepPages;
        page.GetComponent<SleepSwiper>().enabled = false;
        PageStatus(page, false);

        // UI objelerini aç
        SetObjectsActive(true);
    }

    /// <summary>
    /// * Uyku modu özelliklerini sağla
    /// </summary>
    private void SleepMode()
    {
        // İnteraktif mod özelliklerini kapat
        var page = interactivePages;
        page.GetComponent<PageSwiper>().enabled = false;
        page.GetComponent<PinchToZoomCamera>().enabled = false;
        PageStatus(page, false);

        // UI objelerini kapat
        SetObjectsActive(false);
    }

    /// <summary>
    /// * Kullanıcının tercih ettiği mod
    /// </summary>
    public ReadingType GetReadingMode()
    {
        return readingData.readingType;
    }

    /// <summary>
    /// İstediğin sayfayı aç/kapa
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="status"></param>
    private void PageStatus(GameObject obj, bool status)
    {
        obj.SetActive(status);
    }

    /// <summary>
    /// Listedeki tüm objelerin aktiflik durumunu ayarlar.
    /// </summary>
    /// <param name="state">true ise tüm objeleri aktif eder, false ise pasif eder</param>
    public void SetObjectsActive(bool state)
    {
        foreach (GameObject obj in uiObjects)
        {
            if (obj != null)
                obj.SetActive(state);
        }
    }
}
