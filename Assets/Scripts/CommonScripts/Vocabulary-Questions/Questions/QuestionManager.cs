using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// * Soru Prefabs'lerini tutar ve sırasıyla sorulara cevap verilmesini bekler. 
/// </summary>

public class QuestionManager : MonoBehaviour
{
    [Header("Question Prefabs")]
    [SerializeField] private List<GameObject> questionPrefabs = new List<GameObject>(); // Soruların Prefabsleri parentında Question.cs kodu var.
    [SerializeField] private RectTransform area; // Soruların üretileceği alan

    [Header("Steps")]
    [SerializeField] private RectTransform stepArea;
    [SerializeField] private GameObject stepPrefabs;
    private List<Image> steps = new List<Image>();
    [SerializeField] private Color[] stepColors; // Index => 0 kapalı soru, 1 açık soru, 2 doğru cevaplanan soru, 3 yanlış cevaplanan soru.

    [Header("Continue")]
    [SerializeField] private Button continueButton; // Açık olan soruya ait şık seçilmişse continue interactable aktfi olacak ve sonraki soruya geçilecek. Yoksa false...

    private int currentQuestionIndex = 0; // Şu anda kaçıncı sorudayız
    private GameObject currentQuestionGO; // Aktif olarak sahnede yer alan soru GameObject’i

    void Start()
    {
        // Step üret
        InitalizeSteps();

        // Devam butonuna tıklama işlemini dinle
        continueButton.onClick.AddListener(OnContinueClicked);

        // İlk başta pasif olsun
        continueButton.interactable = false;

        // İlk soruyu başlat
        LoadQuestion(0);
    }

    /// <summary>
    /// Soru adımılarını üret
    /// </summary>
    private void InitalizeSteps()
    {
        for (int i = 0; i < questionPrefabs.Count; i++)
        {
            GameObject stepObj = Instantiate(stepPrefabs, stepArea);
            Image stepImage = stepObj.GetComponent<Image>();

            if (stepImage != null)
            {
                var numberText = stepObj.GetComponentInChildren<TMP_Text>();
                if (numberText != null)
                    numberText.text = (i + 1).ToString();

                steps.Add(stepImage);
            }
            else
            {
                Debug.LogWarning("StepPrefab içinde Image component bulunamadı.");
            }
        }
    }

    /// <summary>
    /// Belirli index’teki soruyu yükler.
    /// </summary>
    private void LoadQuestion(int index)
    {
        // Önceki soru varsa sahneden kaldır
        if (currentQuestionGO != null)
            Destroy(currentQuestionGO);

        // Yeni soruyu oluştur ve sahneye yerleştir
        currentQuestionGO = Instantiate(questionPrefabs[index], area);

        // Question.cs bileşenini al
        var question = currentQuestionGO.GetComponent<Question>();

        // Adım rengini 'aktif soru' olarak güncelle (1. renk)
        UpdateStep(index, 1);

        // Şık işaretlendiğinde ne yapılacağını tanımla
        question.OnAnswered += (bool isCorrect) =>
        {
            // Devam butonunu aktif et
            continueButton.interactable = true;

            // Cevap doğruysa 2, yanlışsa 3. renge geç
            UpdateStep(index, isCorrect ? 2 : 3);
        };
    }

    /// <summary>
    /// Devam butonuna tıklandığında çalışır. Bir sonraki soruya geçer.
    /// </summary>
    private void OnContinueClicked()
    {
        // Devam butonunu tekrar pasif yap
        continueButton.interactable = false;

        // Sıradaki soruya geç
        currentQuestionIndex++;

        // Hâlâ sorular kaldıysa yükle
        if (currentQuestionIndex < questionPrefabs.Count)
            LoadQuestion(currentQuestionIndex);
        else
            Debug.Log("Tüm sorular tamamlandı!"); // Quiz bitti
    }

    /// <summary>
    /// Adım göstergesinin rengini günceller.
    /// </summary>
    /// <param name="index">Adım index’i</param>
    /// <param name="colorIndex">Renk index’i (stepColors içinde)</param>
    private void UpdateStep(int index, int colorIndex)
    {
        // Geçerli indekslerdeyse güncelle
        if (index >= 0 && index < steps.Count && colorIndex >= 0 && colorIndex < stepColors.Length)
            steps[index].color = stepColors[colorIndex];
    }
}
