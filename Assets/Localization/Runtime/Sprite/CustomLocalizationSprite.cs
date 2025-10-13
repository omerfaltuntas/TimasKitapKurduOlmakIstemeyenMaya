using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfKids.Localization
{
    /// <summary>
    /// Her dil için başlık + metin içeren veri yapısı.
    /// </summary>
    [Serializable]
    public class SpriteEntry
    {
        [Tooltip("Dil adı (Otomatik belirlenir)")]
        public string language;

        [Tooltip("Bu dile karşılık gelen görsel.")]
        public Sprite sprite;
    }
    [RequireComponent(typeof(SpriteRenderer))]
    [ExecuteInEditMode]  // Bu satır, editör modunda da çalışmasına izin verir
    [AddComponentMenu("BOBU/Localization/Custom Localization Sprite")]
    public class CustomLocalizationSprite : MonoBehaviour
    {
        [Header("Localization Package")]
        [SerializeField] private LocalizationData localization;

        [Header("Localized Sprites")]
        [SerializeField] private List<SpriteEntry> sprites = new List<SpriteEntry>();

        #region  Private Fields
        private SpriteRenderer spriteRenderer;
        #endregion

        void Awake()
        {
            // Görsel bileşenini ata
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Start()
        {
            // Dile göre görseli ata
            ApplyLocalization();
        }

        /// <summary>
        /// Seçili dili alıp, ilgili görseli `SpriteRenderer` bileşenine atar.
        /// </summary>
        public void ApplyLocalization()
        {
            if (localization == null)
            {
                Debug.LogError("Dil seçeneği için LocalizationData ataması eksik!");
                return;
            }

            // spriteRenderer null ise atamaya çalış
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            // Seçili dili getir
            string selectedLang = localization.selectedLanguage.ToString();

            // Dili sprite listesinde bul
            var currentEntry = sprites.Find(e => e.language == selectedLang);
            if (currentEntry == null)
            {
                Debug.LogError("Dil Seçeneği bulunamadı!");
                return;
            }

            // Görseli ata
            spriteRenderer.sprite = currentEntry.sprite;
        }

        /// <summary>
        /// Editörde değişiklik yapıldığında dil alanlarını günceller.
        /// </summary>
        private void OnValidate()
        {
            if (localization == null) return;

            // LocalizationData içindeki mevcut dillerin listesini alır.
            List<string> languageList = localization.languages;
            if (languageList == null) return;

            // Mevcut diller listesine göre görsel alanlarını senkronize et
            for (int i = sprites.Count - 1; i >= 0; i--)
            {
                if (!languageList.Contains(sprites[i].language))
                {
                    sprites.RemoveAt(i); // Listede olup dillerde olmayanları kaldır
                }
            }

            // Eğer yeni bir dil eklendiyse, o dil için yeni bir giriş oluştur
            foreach (string lang in languageList)
            {
                if (!sprites.Exists(e => e.language == lang))
                {
                    sprites.Add(new SpriteEntry { language = lang, sprite = null });
                }
            }

            // Görsel atamasını sağla
            ApplyLocalization();
        }
    }
}
