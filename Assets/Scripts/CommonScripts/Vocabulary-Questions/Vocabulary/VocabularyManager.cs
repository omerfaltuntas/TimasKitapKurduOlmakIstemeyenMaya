using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// * Yeni kelimelerin üretilmesini sağlamaktadır. (Prefabs) aracılığyla dizaynlar yapılır. Standart sürükle bırak olması lazım.
/// </summary>

public class VocabularyManager : MonoBehaviour
{
    [Header("Question Prefabs")]
    [SerializeField] private List<GameObject> vocabularyPrefabs = new List<GameObject>(); // Kelimlerin Prefabsleri
    [SerializeField] private ScrollRect scrollRect; // ScrollView içindeki ScrollRect component'i
    [SerializeField] private RectTransform area; // Kelimelerin üretileceği alan
    private bool isScrolling = false;
    private Coroutine coroutine;
    void Start()
    {
        // Kelimleri üret
        InitalizeVocabulary();

        // ScrollRect'e dinleyici ekle
        scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
    }

    /// <summary>
    /// * Kelimelerin tamamını üret
    /// </summary>
    private void InitalizeVocabulary()
    {
        // Öncekileri temizle
        foreach (Transform child in area)
        {
            Destroy(child.gameObject);
        }

        // Tümünü üret
        foreach (GameObject prefab in vocabularyPrefabs)
        {
            GameObject obj = Instantiate(prefab, area);
        }
    }

    /// <summary>
    /// Scroll işlemi yapıldığında tetiklenir
    /// </summary>
    private void OnScrollValueChanged(Vector2 value)
    {
        isScrolling = true;
        Debug.Log("Scroll yapılıyor");

        if (coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(WaitForScrollToStop());
    }

    /// <summary>
    /// Scroll'un durduğunu tespit etmek için küçük bir gecikme ile kontrol
    /// </summary>
    private IEnumerator WaitForScrollToStop()
    {
        yield return new WaitForSeconds(0.2f);
        isScrolling = false;
        Debug.Log("Scroll durdu.");
    }
}
