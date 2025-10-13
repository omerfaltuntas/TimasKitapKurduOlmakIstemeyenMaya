using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Bu kod sayfası içerik içerisinde geçirilen sayfa sayısını bildirmektedir.
/// </summary>

public class GeneralPageManager : MonoBehaviour
{
    public static GeneralPageManager Instance;

    [Header("Data")]
    [Tooltip("Kitabın özel alan adı yazılmalı!")]
    [SerializeField] private string dataName;

    [Header("Settings")]
    [Tooltip("Kitabın toplam sayfa sayısı yazılmalı")]
    public int maxPageCount;
    public int currentCount;

    // Kitap okuma verisini tetikleme
    public Action onDataUpdate;

    [Header("UI Settings")]
    [SerializeField] private Button nextPage;
    [SerializeField] private Button previousPage;

    // TODO: Bu bool değişkeni proje çıktısında editör üzerinden false konumuna getirilmeli.
    [Header("Çalışırken bool'u TRUE edin")]
    [SerializeField] private bool isActivePage;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        // İçerikte en son kalınan sayfayı getir
        if (!IsActivePage())
            GetProggress();

        // En son kaldığı sayfadan başlatma(İnteraktif modda ise)
        ReadingType readingType = ReadingManager.Instance.GetReadingMode();
        if (readingType == ReadingType.Interactive)
            ReadWhereYouLeftOff();
    }

    void Start()
    {
        // Veri kaydetmeyi abone et
        onDataUpdate += SaveProgress;

        // Buton dinleyicileri
        nextPage.onClick.AddListener(() => PageSwiper.Instance.NextPage());
        previousPage.onClick.AddListener(() => PageSwiper.Instance.PreviousPage());
    }

    // Sayfa sayısını getir
    private void GetProggress()
    {
        if (!PlayerPrefs.HasKey($"{dataName}_PageData"))
        {
            Debug.LogWarning("Kitaba ait okunan sayfa verisi yok!");
            return;
        }

        currentCount = PlayerPrefs.GetInt($"{dataName}_PageData");
        Debug.Log($"En son okuduğu sayfa: {currentCount}");
    }

    // Sayfa sayısını kaydet
    private void SaveProgress()
    {
        if (string.IsNullOrEmpty(dataName))
            Debug.LogError("dataName değeri atanmadı! PlayerPrefs kaydı yapılamaz.");

        PlayerPrefs.SetInt($"{dataName}_PageData", currentCount);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Kullanıcının kitabı en son kaldığı sayfadan okumasını sağlar.
    /// </summary>
    private void ReadWhereYouLeftOff()
    {
        int value = (currentCount > 0) ? currentCount - 1 : 0; // Eğer currentCount 0 değilse -1 çıkar, yoksa 0 kalır.
        PageSwiper.Instance.currentPageIndex = value; // Değeri eşitle
    }


    /// <summary>
    /// TODO Bu değişken sadece geliştirme aşamasında en son kalan sayfanın getirilmesi ya da getirilmemesini sağlar.
    /// </summary>
    /// <returns></returns>
    private bool IsActivePage()
    {
        return isActivePage;
    }

}