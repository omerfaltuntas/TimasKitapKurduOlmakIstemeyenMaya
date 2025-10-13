using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// "Scriptable Object" asseti içerisinde modları tutar.
/// </summary>
/// 

[CreateAssetMenu(fileName = "ReadingData", menuName = "BOBU/ReadingMode/ReadingData")]
public class ReadingData : ScriptableObject
{
    public ReadingType readingType; // Seçilen mod
}
