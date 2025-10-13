using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// BOBU -> Reading Mode menüsünden açılan okuma modu yöneticisi şimdilik iki okuma modu var => Interactive and Sleep
/// </summary>

public class ReadingEditor : EditorWindow
{
    // Temel ayarlar için değişkenler
    #region  Private Fields
    private string packageName = "Reading Mode";
    private string savePath = "Assets/ReadingMode/Data/ScriptableObject";

    private ReadingData readingData;
    #endregion

    // Editör penceresini açan menü öğesi
    [MenuItem("BOBU/Reading Mode")]
    public static void ShowWindow()
    {
        GetWindow<ReadingEditor>("Reading Mode Package Manager");
    }

    private void OnEnable()
    {
        LoadOrCreateReadingData();
    }

    // Editör arayüzünü oluşturan ana fonksiyon
    private void OnGUI()
    {
        if (readingData == null)
        {
            EditorGUILayout.HelpBox("ReadingData asseti yüklenemedi veya oluşturulamadı.", MessageType.Error);
            return;
        }

        EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();

        readingData.readingType = (ReadingType)EditorGUILayout.EnumPopup("Mode:", readingData.readingType);

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(readingData);
            AssetDatabase.SaveAssets();
            Debug.Log("Reading mode updated to: " + readingData.readingType);
        }
    }

    /// <summary>
    /// * ReadingData objesi mevcut değilse oluşturma, mevcut ise yükleme işlemi
    /// </summary>
    private void LoadOrCreateReadingData()
    {
        string assetPath = $"{savePath}/ReadingData.asset";
        readingData = AssetDatabase.LoadAssetAtPath<ReadingData>(assetPath);

        if (readingData == null)
        {
            Debug.LogWarning("ReadingData bulunamadı. Yeni bir tane oluşturuluyor.");
            readingData = ScriptableObject.CreateInstance<ReadingData>();
            if (!AssetDatabase.IsValidFolder(savePath))
            {
                System.IO.Directory.CreateDirectory(savePath);
                AssetDatabase.Refresh();
            }

            AssetDatabase.CreateAsset(readingData, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

}
