using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Bu kodum dil paketindeki dil seçimlerini ve günceller dilleri göstermektedir.
/// </summary>
namespace AgeOfKids.Localization
{
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance;

        [Header("Language")]
        public LocalizationData localization;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            string language = GetSelectedLanguage().ToString();
            Debug.Log($"Güncel Dil: {language}");
        }

        // Şu anda seçili olan dili döndürür
        public LanguageType GetSelectedLanguage()
        {
            return localization.selectedLanguage;
        }

        // Seçili dili değiştirmek
        public void SetSelectedLanguage(LanguageType language)
        {
            localization.selectedLanguage = language;
            // Dil değişikliği sonrası yapılacak işlemler (örneğin UI güncellemesi)
            Debug.Log($"Language changed to {language.ToString()}");
        }
    }
}