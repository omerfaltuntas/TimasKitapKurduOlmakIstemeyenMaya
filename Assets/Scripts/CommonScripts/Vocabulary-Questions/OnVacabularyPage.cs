using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// * Sözlük ve sorular sayfası açıldığında ve kapandığında UI değişiklikleri sağla
/// </summary>

public class OnVacabularyPage : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private List<GameObject> uiObjects = new List<GameObject>();


    void OnEnable()
    {
        SetObjectsActive(false);
    }

    void OnDisable()
    {
        SetObjectsActive(true);
    }


    /// <summary>
    /// Listedeki tüm objelerin aktiflik durumunu ayarlar.
    /// </summary>
    /// <param name="state">true ise tüm objeleri aktif eder, false ise pasif eder</param>
    public void SetObjectsActive(bool state)
    {
        foreach (GameObject obj in uiObjects)
        {
            if (obj != null)
                obj.SetActive(state);
        }
    }
}
