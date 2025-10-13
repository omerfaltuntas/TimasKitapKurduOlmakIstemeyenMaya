using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

/// <summary>
/// BOBU -> Localization menüsünden açılan dil yönetim aracı
/// </summary>
public class LocalizationEditor : EditorWindow
{
    // Temel ayarlar için değişkenler
    private string packageName = "Languages";
    private string savePath = "Assets/Localization/Data/ScriptableObject";
    private LocalizationData localizationData;

    // Dil yönetimi için gerekli değişkenler
    private Vector2 scrollPosition;
    private List<string> languages = new List<string>();  // Dil listesi
    private Dictionary<string, Dictionary<string, string>> translations = new Dictionary<string, Dictionary<string, string>>();  // Çeviriler sözlüğü
    private string newLanguageCode = "";  // Yeni dil kodu girişi
    private bool showAddLanguage = false;  // Dil ekleme panelini göster/gizle
    private int selectedLanguageIndex = 0; // Seçilen dilin indeksi

    // Editör penceresini açan menü öğesi
    [MenuItem("BOBU/Localization")]
    public static void ShowWindow()
    {
        GetWindow<LocalizationEditor>("Localization Package Manager");
    }

    private void OnEnable()
    {
        // ScriptableObject'i yükle veya oluştur
        LoadOrCreateLocalizationData();
    }

    // Editör arayüzünü oluşturan ana fonksiyon
    private void OnGUI()
    {
        // Her frame'de dil kontrol senkronizasyon yap
        SyncSelectedLanguage();

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Package Settings", EditorStyles.boldLabel);
        packageName = EditorGUILayout.TextField("Package Name:", packageName);

        EditorGUILayout.BeginHorizontal();
        savePath = EditorGUILayout.TextField("Save Path:", savePath);
        if (GUILayout.Button("Browse", GUILayout.Width(60)))
        {
            string newPath = EditorUtility.OpenFolderPanel("Select Save Location", savePath, "");
            if (!string.IsNullOrEmpty(newPath))
            {
                savePath = "Assets" + newPath.Substring(Application.dataPath.Length);
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Language Management", EditorStyles.boldLabel);

        EditorGUILayout.Space(2);

        // Dil seçimi için dropdown
        if (languages.Count > 0)
        {
            int previousLanguageIndex = selectedLanguageIndex; // Önceki seçilen dili sakla
            selectedLanguageIndex = EditorGUILayout.Popup("Selected Language:", selectedLanguageIndex, languages.ToArray());

            // Eğer dil değiştiyse, LocalizationData'ya kaydet
            if (selectedLanguageIndex != previousLanguageIndex)
            {
                localizationData.selectedLanguage = (LanguageType)selectedLanguageIndex;
                EditorUtility.SetDirty(localizationData); // Değişiklikleri kaydet
                AssetDatabase.SaveAssets();
            }
        }
        else
        {
            EditorGUILayout.LabelField("No languages available.");
        }

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Add New Language"))
            showAddLanguage = !showAddLanguage;

        if (showAddLanguage)
        {
            EditorGUILayout.BeginHorizontal();
            newLanguageCode = EditorGUILayout.TextField("Language Code:", newLanguageCode);
            if (GUILayout.Button("Add", GUILayout.Width(60)))
            {
                if (!string.IsNullOrEmpty(newLanguageCode) && !languages.Contains(newLanguageCode))
                {
                    // Onay dialogu göster
                    if (EditorUtility.DisplayDialog("Add Language",
                        $"Do you want to add {newLanguageCode} language?",
                        "Add", "Cancel"))
                    {
                        // Dili ekle ve kaydet
                        AddLanguage(newLanguageCode);
                        newLanguageCode = "";
                        showAddLanguage = false;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space(10);

        // Mevcut dilleri göster
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        for (int i = 0; i < languages.Count; i++) // for döngüsü kullanarak güvenli iterasyon
        {
            string language = languages[i];
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Language: {language}", EditorStyles.boldLabel);
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                if (EditorUtility.DisplayDialog("Remove Language",
                    $"Do you want to remove {language} language?",
                    "Remove", "Cancel"))
                {
                    RemoveLanguage(language);
                    i--; // Dili kaldırdıktan sonra indeksi azalt
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();
    }

    // "LocalizationData" içerisindeki dilleri getir, dosyalar yoksa oluştur vs.
    private void LoadOrCreateLocalizationData()
    {
        // ScriptableObject'i bul veya oluştur
        string dataPath = $"{savePath}/LocalizationData.asset";
        localizationData = AssetDatabase.LoadAssetAtPath<LocalizationData>(dataPath);

        if (localizationData == null)
        {
            // Klasörü oluştur
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            // Yeni ScriptableObject oluştur
            localizationData = CreateInstance<LocalizationData>();
            AssetDatabase.CreateAsset(localizationData, dataPath);
            AssetDatabase.SaveAssets();
        }

        // Mevcut dilleri yükle
        languages = new List<string>(localizationData.languages);

        // Dil seçme dropdown güncelle
        UpdateLanguageEnum();

        if (languages.Count > 0)
        {
            selectedLanguageIndex = Mathf.Max(0, languages.IndexOf(localizationData.selectedLanguage.ToString()));
        }
    }

    // Editörden yeni dili ekle
    private void AddLanguage(string languageCode)
    {
        // Dili listeye ekle
        languages.Add(languageCode);
        translations[languageCode] = new Dictionary<string, string>();

        // ScriptableObject'i güncelle
        localizationData.languages = new List<string>(languages);
        EditorUtility.SetDirty(localizationData);
        AssetDatabase.SaveAssets();

        // Dil seçme dropdown güncelle
        UpdateLanguageEnum();
    }

    // Editörden ilgili dili sil
    private void RemoveLanguage(string languageCode)
    {
        // Dili listeden kaldır
        languages.Remove(languageCode);
        translations.Remove(languageCode);

        // ScriptableObject'i güncelle
        localizationData.languages = new List<string>(languages);
        EditorUtility.SetDirty(localizationData);
        AssetDatabase.SaveAssets();

        // Dil seçme dropdown güncelle
        UpdateLanguageEnum();
    }

    // Dil seçeneğini güncelleme
    private void UpdateLanguageEnum()
    {
        string enumPath = "Assets/Localization/Data/LanguageType.cs";
        using (StreamWriter writer = new StreamWriter(enumPath))
        {
            writer.WriteLine("public enum LanguageType");
            writer.WriteLine("{");

            foreach (string language in languages)
            {
                string enumName = language.Replace(" ", "").Replace("-", "").Replace(".", "");
                writer.WriteLine($"    {enumName},");
            }

            writer.WriteLine("}");
        }

        AssetDatabase.Refresh();
    }

    // "LocalizationData" içerisinden dil değişirse arayüzdeki "selectedLanguage" dropdown güncelle
    private void SyncSelectedLanguage()
    {
        if (localizationData != null && languages.Count > 0)
        {
            int newIndex = languages.IndexOf(localizationData.selectedLanguage.ToString());

            if (newIndex != selectedLanguageIndex) // Eğer değer değişmişse güncelle
            {
                selectedLanguageIndex = newIndex;
                Repaint(); // Editörü yeniden çizerek güncellenmesini sağla
            }
        }
    }
}