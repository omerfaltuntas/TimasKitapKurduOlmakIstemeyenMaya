using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// * Soru modeli 4 adet buton şıkları ve 1 adette doğru cevap var. Aynı zamanda doğru yanlış şıklarının sprite değişikliğini sağlar.
/// </summary>
/// 

public class Question : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private List<Sprite> answerSprites = new List<Sprite>(); // 0 = default, 1 = doğru, 2 = yanlış
    [SerializeField] private List<Button> buttons = new List<Button>();

    [Header("Answer Index")]
    [SerializeField] private int correctAnswerIndex = 0;
    public int CorrectAnswerIndex => correctAnswerIndex;
    private bool isAnswered = false;
    public Action<bool> OnAnswered; // Doğru mu yanlış mı callback

    void Start()
    {
        SetupButtons();
    }

    /// <summary>
    ///  * Buton tıklamaları dinle
    /// </summary>
    private void SetupButtons()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            int index = i;
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => OnAnswerSelected(index));

            // Başlangıçta default sprite ata
            var img = buttons[i].GetComponent<Image>();
            if (img != null)
                img.sprite = answerSprites[0];
        }

        isAnswered = false;
    }

    /// <summary>
    /// * Cevap şıkkı işaretlendi.
    /// </summary>
    /// <param name="selectedIndex"></param>
    private void OnAnswerSelected(int selectedIndex)
    {
        if (isAnswered) return;
        isAnswered = true;

        bool isCorrect = selectedIndex == correctAnswerIndex;

        // Seçilen cevaba sprite ata
        var selectedImg = buttons[selectedIndex].GetComponent<Image>();
        if (selectedImg != null)
            selectedImg.sprite = isCorrect ? answerSprites[1] : answerSprites[2];

        // Doğru cevabın sprite'ını göster (eğer yanlış cevaplandıysa)
        if (!isCorrect)
        {
            var correctImg = buttons[correctAnswerIndex].GetComponent<Image>();
            if (correctImg != null)
                correctImg.sprite = answerSprites[1];
        }

        // Tüm butonları kilitle
        foreach (var btn in buttons)
            btn.enabled = false;

        OnAnswered?.Invoke(isCorrect); // Manager'a bildir
    }

}
