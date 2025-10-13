using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// * Kitap içerisindeki arayüz birimlerinin genel yönetimi sağlamaktadır.
/// </summary>

public class BookUIManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button open;
    [SerializeField] private Button close;
    [SerializeField] private Button home;

    [Header("Sound & Voice")]
    [SerializeField] private RectTransform soundArea;
    [SerializeField] private Button soundButton;
    [SerializeField] private GameObject soundOff;
    [SerializeField] private Button voiceButton;
    [SerializeField] private GameObject voiceOff;
    private bool isSound = false;
    private bool isVoice = false;

    [Header("Background Music")]
    [SerializeField] private AudioSource backgroundMusic;

    [Header("Storytelling")]
    [SerializeField] private AudioSource storytelling;

    [Header("Page Navigation")]
    [SerializeField] private RectTransform pageArea;
    [SerializeField] private Image sliderValue;
    [SerializeField] private RectTransform handle;
    [SerializeField] private TMP_Text numberText;
    [SerializeField] private Button nextPage;
    [SerializeField] private Button previousPage;

    void Awake()
    {
        // Settings paneli kapalı gelsin
        PanelStatus(false);
    }


    void Start()
    {
        // Buton dinleyicileri
        open.onClick.AddListener(() => PanelStatus(true));
        close.onClick.AddListener(() => PanelStatus(false));

        // Uygulamaya dön
        home.onClick.AddListener(GoHome);

        // Sayfa geçiş butonları
        nextPage.onClick.AddListener(OnNextPage);
        previousPage.onClick.AddListener(OnPreviousPage);

        // Sesler
        soundButton.onClick.AddListener(ToggleSound);
        voiceButton.onClick.AddListener(ToogleVoice);
    }

    /// <summary>
    /// * Ayarlar panelinin aç/kapa işlevini yürütür.
    /// </summary>
    /// <param name="status"></param>
    private void PanelStatus(bool status)
    {
        settingsPanel.SetActive(status);

        if (status)
        {
            // Sound alanını aç
            PanelAnimation(soundArea, -60f, 0.3f);
            // Page alanını aç
            PanelAnimation(pageArea, 0f, 0.3f);

            // Panel açıldığında güncel sayfa verisini göster
            UpdatePageUI();
        }
        else
        {
            // Sound alanını aç
            PanelAnimation(soundArea, 300f, 0.1f);
            // Page alanını aç
            PanelAnimation(pageArea, -300f, 0.1f);
        }

    }

    /// <summary>
    /// * İlgili rect transform değerlerine animasyon uygula
    /// </summary>
    /// <param name="duration"></param>
    private void PanelAnimation(RectTransform rectTransform, float yPos, float duration)
    {
        rectTransform.DOAnchorPosY(yPos, duration).SetEase(Ease.InOutSine);
    }

    #region  PAGE MANAGER

    /// <summary>
    /// * Bir sonraki sayfaya geç
    /// </summary>
    private void OnNextPage()
    {
        // Bir sonraki sayfaya geç
        PageSwiper.Instance.NextPage();
        // SaveProgress çağrılır
        GeneralPageManager.Instance.onDataUpdate?.Invoke();
        // UI güncelle
        UpdatePageUI();
    }

    /// <summary>
    /// * Bir önceki sayfaya geç
    /// </summary>
    private void OnPreviousPage()
    {
        // Bir önceki sayfaya geç
        PageSwiper.Instance.PreviousPage();
        // SaveProgress çağrılır
        GeneralPageManager.Instance.onDataUpdate?.Invoke();
        // UI güncelle
        UpdatePageUI();
    }

    /// <summary>
    /// * Sayfa alanı -> Slider barı, handle ve text güncelle
    /// </summary>
    private void UpdatePageUI()
    {
        // Değişkenleri al
        int currentCount = PageSwiper.Instance.currentPageIndex + 1; // Slider için 1 based index
        int maxCount = GeneralPageManager.Instance.maxPageCount;

        // Sayfa numarası gösterimi
        numberText.text = $"{currentCount}";

        // Slider güncelle (0 ile 1 arasında bir değer)
        if (maxCount > 0)
        {
            // Bar
            float ratio = (float)(currentCount - 1) / (maxCount - 1);
            sliderValue.fillAmount = Mathf.Clamp01(ratio);

            // Handle'ın anchoredPosition değerini güncelle
            float forwardXPosition = HandleBar(sliderValue.fillAmount, 0f, 1f, -200f, 200f);
            handle.anchoredPosition = new Vector2(forwardXPosition, handle.anchoredPosition.y);
        }
    }

    // Handle barın slider ile birlikte hareket etmesini sağlar.
    public float HandleBar(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }

    #endregion


    #region  SOUND MANAGER

    /// <summary>
    /// * Ses efektleri, arkaplan müziğini kapat/aç işlemleri
    /// </summary>
    private void ToggleSound()
    {
        isSound = !isSound;

        // AudioManager'daki tüm sesleri kapat/aç
        foreach (var sound in AudioManager.Instance.Sounds)
        {
            if (sound.Source != null)
                sound.Source.mute = isSound;
        }

        // BG müziğini kapat aç
        backgroundMusic.mute = isSound;

        // UI'daki simgeyi güncelle
        soundOff.SetActive(isSound);
    }

    /// <summary>
    /// * Kitap içi seslendirme öğelerini kapat/aç işlemleri
    /// </summary>
    private void ToogleVoice()
    {
        isVoice = !isVoice;

        // Seslendirmeyi kapat aç
        storytelling.mute = isVoice;

        // UI'daki simgeyi güncelle
        voiceOff.SetActive(isVoice);
    }

    #endregion

    /// <summary>
    /// * Kitap içeriğinde, uygulama ekranına dönme
    /// </summary>
    private void GoHome()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
