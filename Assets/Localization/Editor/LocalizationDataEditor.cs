using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;


/// <summary>
/// Bu kodum Localization Editör üzerinden eklenen dil seçeneklerini Enum formatında LanguageType.cs koduna dili ekleme ya da çıkarma yapar.
/// </summary>

[CustomEditor(typeof(LocalizationData))]
public class LocalizationDataEditor : Editor
{
    // Enum dosyasını kaydetme yolu
    string enumFilePath = "Assets/Localization/Data/LanguageType.cs"; // Enum dosyasını burada tutuyoruz

    private void OnEnable()
    {
        // İlk başta enum'ı güncelleyebiliriz.
        UpdateLanguageEnum();
    }

    // Dil listesindeki string değerlerden LanguageType enum'ını oluşturacak metod
    private void UpdateLanguageEnum()
    {
        LocalizationData localizationData = (LocalizationData)target;
        string enumCode = GenerateEnumCode(localizationData.languages);

        // Eğer dosya yoksa, yeni bir dosya oluştur
        if (!File.Exists(enumFilePath))
        {
            File.WriteAllText(enumFilePath, enumCode);
            AssetDatabase.Refresh();
        }
        else
        {
            // Enum kodunu güncelle
            File.WriteAllText(enumFilePath, enumCode);
            AssetDatabase.Refresh();
        }
    }

    // Enum kodunu oluşturacak metod
    private string GenerateEnumCode(List<string> languages)
    {
        string enumCode = "public enum LanguageType\n{\n";

        // Dillerin listesinde döngü ile enum elemanlarını ekleyelim
        foreach (var language in languages)
        {
            string enumValue = language.Replace(" ", "").Replace("-", "").ToUpper(); // Enum ismi geçerli bir formatta olmalı
            enumCode += $"    {enumValue},\n";
        }

        enumCode += "}\n";

        return enumCode;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LocalizationData localizationData = (LocalizationData)target;

        // Dil listesinde değişiklik yapıldığında enum'u tekrar güncelle
        if (GUILayout.Button("Update Language Enum"))
        {
            UpdateLanguageEnum();
        }
    }
}
