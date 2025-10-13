using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfKids.Localization
{
    /// <summary>
    /// Her dil için başlık + klip içeren veri yapısı.
    /// </summary>
    [Serializable]
    public class ClipEntry
    {
        [Tooltip("Dil adı (Otomatik belirlenir)")]
        public string language;

        [Tooltip("Bu dile karşılık gelen ses klibi.")]
        public AudioClip audioClip;
    }

    /// <summary>
    /// Bu sınıf, dil seçeneklerine göre ses kliplerini değiştirir.
    /// `LocalizationData` nesnesindeki mevcut diller kadar düzenli ses klip alanları oluşturur.
    /// Seçili dile göre GameObject içindeki `AudioSource` bileşenindeki klip değerini günceller.
    /// </summary>
    /// 

    [RequireComponent(typeof(AudioSource))] // Ses objesi zorunlu
    [ExecuteInEditMode]  // Bu satır, editör modunda da çalışmasına izin verir
    [AddComponentMenu("BOBU/Localization/Custom Localization Audio")]
    public class CustomLocalizationAudio : MonoBehaviour
    {
        [Header("Localization Package")]
        [SerializeField] private LocalizationData localization;

        [Header("Localized Clips")]
        [SerializeField] private List<ClipEntry> clips = new List<ClipEntry>();
        private AudioSource audioSource;

        void Awake()
        {
            // Ses bileşenini ata
            audioSource = GetComponent<AudioSource>();
        }

        void Start()
        {
            // Dile göre sesi ata ve çal
            ApplyLocalization();
        }

        /// <summary>
        /// Seçili dili alıp, ilgili klibi `AudioSource` bileşenine atar.
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

            // Dili clips listesinde bul
            var currentEntry = clips.Find(e => e.language == selectedLang);
            if (currentEntry == null)
            {
                Debug.LogError("Dil Seçeneği bulunamadı!");
                return;
            }

            // Sesi ata
            audioSource.clip = currentEntry.audioClip;
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

            // Mevcut diller listesine göre metin alanlarını senkronize et
            for (int i = clips.Count - 1; i >= 0; i--)
            {
                if (!languageList.Contains(clips[i].language))
                {
                    clips.RemoveAt(i); // Listede olup dillerde olmayanları kaldır
                }
            }

            // Eğer yeni bir dil eklendiyse, o dil için yeni bir giriş oluştur
            foreach (string lang in languageList)
            {
                if (!clips.Exists(e => e.language == lang))
                {
                    clips.Add(new ClipEntry { language = lang, audioClip = null });
                }
            }

        }

    }
}