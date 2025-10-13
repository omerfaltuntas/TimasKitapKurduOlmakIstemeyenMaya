using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bu kod, okuma modu paketindeki mod seçimlerini gösterir aynı zamanda günceller.
/// </summar

namespace AgeOfKids.Reading
{
    public class ReadingModeManager : MonoBehaviour
    {
        public static ReadingModeManager Instance;

        [Header("Mode")]
        [SerializeField] private ReadingData readingData;

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
            string mode = GetSelectedMode().ToString();
            Debug.Log($"Güncel Okuma Modu: {mode}");
        }

        // Şu anda seçili olan dili döndürür
        public ReadingType GetSelectedMode()
        {
            return readingData.readingType;
        }

        // Seçili modu değiştirmek
        public void SetSelectedMode(ReadingType readingType)
        {
            readingData.readingType = readingType;
            // Dil değişikliği sonrası yapılacak işlemler (örneğin UI güncellemesi)
            Debug.Log($"Language changed to {readingType.ToString()}");
        }

    }
}

