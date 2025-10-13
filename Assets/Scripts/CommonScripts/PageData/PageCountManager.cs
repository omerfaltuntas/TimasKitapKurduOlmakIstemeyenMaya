using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bu kod, sayfa içlerinde bulunur. (Ana Parent) 
/// pageCount Kaçıncı sayfa ise değeri elle girilir.
/// </summary>

public class PageCountManager : MonoBehaviour
{
    [Header("Count Settings")]
    public int pageCount;

    private void OnEnable()
    {
        var manager = GeneralPageManager.Instance;
        if (manager != null && manager.currentCount < pageCount)
        {
            manager.currentCount = pageCount; // Sayfa verisini gönder
            manager.onDataUpdate?.Invoke(); // Veri kaydetmeyi tetikle
        }
    }

}
