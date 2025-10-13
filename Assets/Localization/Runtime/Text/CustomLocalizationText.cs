using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AgeOfKids.Localization
{
    /// <summary>
    /// Text bileşeni seçeneğini belirleme. Sahne için: "Text3D", Canvas içi kullanım için "UI" seçeniğini editör otomatik seçer.
    /// </summary>
    public enum TextType
    {
        UI,
        Text3D
    }


    /// <summary>
    /// Her dil için başlık + metin içeren veri yapısı.
    /// </summary>
    [Serializable]
    public class TextEntry
    {
        [Tooltip("Dil adı (Otomatik belirlenir)")]
        public string language;

        [Tooltip("Bu dile karşılık gelen metin.")]
        [TextArea(3, 5)] // Daha büyük bir metin alanı oluşturur
        public string text;
    }

    /// <summary>
    /// Bu sınıf, dil seçeneklerine göre UI metinlerini değiştirir.
    /// `LocalizationData` nesnesindeki mevcut diller kadar düzenli metin alanları oluşturur.
    /// Seçili dile göre GameObject içindeki `TextMeshProUGUI` veya `TextMeshPro` bileşeninin text değerini günceller.
    /// </summary>
    /// 

    [ExecuteInEditMode]  // Bu satır, editör modunda da çalışmasına izin verir
    [AddComponentMenu("BOBU/Localization/Custom Localization Text")]
    public class CustomLocalizationText : MonoBehaviour
    {
        [Header("Localization Package")]
        [SerializeField] private LocalizationData localization;

        [Header("Text Type")]
        [HideInInspector] public TextType textType; // Bileşen oluşturulurken otomatik seçilir.

        [Header("Localized Texts")]
        [SerializeField] private List<TextEntry> texts = new List<TextEntry>();

        private TMP_Text textMeshProUI;
        private TextMeshPro textMeshPro3D;
        private RectTransform rectTransform;

        private void Awake()
        {
            // Text ve Rect atamasını sağla.
            AssignTextComponent();

            // Başlangıçta dile göre texti ayarlamayı başlat.
            ApplyLocalization();
        }

        // Seçilen Kullanım alanına göre atamaları yap
        private void AssignTextComponent()
        {
            // UI
            if (textType == TextType.UI)
            {
                textMeshProUI = GetComponent<TextMeshProUGUI>();
                rectTransform = GetComponent<RectTransform>();
                return;
            }

            // 3D Text
            if (textType == TextType.Text3D)
            {
                textMeshPro3D = GetComponent<TextMeshPro>();
                rectTransform = GetComponent<RectTransform>();
                return;
            }
        }

        /// <summary>
        /// Seçili dili alıp, ilgili metni `TextMeshProUGUI` veya `TextMeshPro` bileşenine atar.
        /// </summary>
        public void ApplyLocalization()
        {
            if (localization == null)
            {
                Debug.LogError("Dil seçeneği için LocalizationData ataması eksik!");
                return;
            }

            // Seçili dili getir
            string selectedLang = localization.selectedLanguage.ToString();

            // Dili texts listesinde bul
            var currentEntry = texts.Find(e => e.language == selectedLang);
            if (currentEntry == null)
            {
                Debug.LogError("Dil Seçeneği bulunamadı!");
                return;
            }

            if (textType == TextType.UI && textMeshProUI != null)
            {
                // Dile göre metni yaz
                textMeshProUI.text = currentEntry.text;

                // Metine göre boyutu ayarla
                AdjustTextSize();

                Debug.Log($"Seçili dil ({selectedLang}) için ({textMeshProUI.name}) metnine uyarlaması sağlandı.");
                return;
            }

            if (textType == TextType.Text3D && textMeshPro3D != null)
            {
                // Dile göre metni yaz
                textMeshPro3D.text = currentEntry.text;

                // Metine göre boyutu ayarla
                AdjustTextSize();
                Debug.Log($"Seçili dil ({selectedLang}) için ({textMeshPro3D.name}) metnine uyarlaması sağlandı.");
                return;
            }

        }

        /// <summary>
        /// Metni sığdırmak için `TextMeshProUGUI` veya `TextMeshPro` nesnesinin boyutunu dinamik olarak ayarlar.
        /// </summary>
        private void AdjustTextSize()
        {
            // UI
            // if (textType == TextType.UI && textMeshProUI != null && rectTransform != null)
            // {
            //     textMeshProUI.ForceMeshUpdate();
            //     Vector2 textSize = textMeshProUI.textBounds.size;
            //     rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, textSize.y);
            //     return;
            // }

            // 3D Text
            if (textType == TextType.Text3D && textMeshPro3D != null)
            {
                textMeshPro3D.ForceMeshUpdate();
                Vector2 textSize = textMeshPro3D.textBounds.size;
                // TMP'nin otomatik genişlik ve yükseklik değerlerini kullan
                float preferredWidth = textMeshPro3D.preferredWidth;
                // Minimum genişlik belirleyerek aşırı sıkışmayı engelle
                float adjustedWidth = Mathf.Max(preferredWidth, textSize.x);
                rectTransform.sizeDelta = new Vector2(adjustedWidth, textSize.y);
                return;
            }
        }



        /// <summary>
        /// Editörde değişiklik yapıldığında dil alanlarını günceller.
        /// </summary>
        private void OnValidate()
        {
            if (localization == null) return;

            // Text ve Rect atamasını sağla.
            AssignTextComponent();

            // LocalizationData içindeki mevcut dillerin listesini alır.
            List<string> languageList = localization.languages;
            if (languageList == null) return;

            // Mevcut diller listesine göre metin alanlarını senkronize et
            for (int i = texts.Count - 1; i >= 0; i--)
            {
                if (!languageList.Contains(texts[i].language))
                {
                    texts.RemoveAt(i); // Listede olup dillerde olmayanları kaldır
                }
            }

            // Eğer yeni bir dil eklendiyse, o dil için yeni bir giriş oluştur
            foreach (string lang in languageList)
            {
                if (!texts.Exists(e => e.language == lang))
                {
                    texts.Add(new TextEntry { language = lang, text = "" });
                }
            }

            // Eğer Inspector'da metin değiştiyse ve seçili dile aitse anında güncelle
            ApplyLocalization();

            // Dil değiştiyse text içeriklerini güncelle

        }
    }
}