using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// "Scriptable Object" asseti içerisinde dilleri tutar.
/// </summary>
/// 

[CreateAssetMenu(fileName = "LocalizationData", menuName = "BOBU/Localization/LocalizationData")]
public class LocalizationData : ScriptableObject
{
    [HideInInspector] public List<string> languages = new List<string>();
    public LanguageType selectedLanguage;  // Seçilen dil 
}




