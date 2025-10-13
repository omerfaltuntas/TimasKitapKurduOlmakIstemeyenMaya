using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Bu sınıf, Hierarchy veya mevcut bir Canvas içinde UI veya sahne içinde 3D olarak TextMeshPro nesnesi oluşturmak için kullanılır.
/// Eğer sahnede bir Canvas bulunmuyorsa, UI nesnesi oluşturulurken otomatik olarak yeni bir Canvas ve EventSystem eklenir.
/// </summary>

namespace AgeOfKids.Localization
{
    public class CreateTextMeshPro : MonoBehaviour
    {
        // UI Text bileşeni
        [MenuItem("GameObject/BOBU/Localization/Text/UI-TextMeshPro", false, 10)]
        private static void CreateUITextMeshPro()
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                // Eğer sahnede bir Canvas yoksa, yeni bir tane oluştur
                GameObject canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();

                // EventSystem kontrolü yap ve yoksa oluştur
                if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
                {
                    GameObject eventSystem = new GameObject("EventSystem");
                    eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                    eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                }
            }

            // Yeni UI-TextMeshPro nesnesi oluştur
            GameObject newObj = new GameObject("UI-TextMeshPro");
            TextMeshProUGUI textMesh = newObj.AddComponent<TextMeshProUGUI>();
            textMesh.text = "BOBU TEAM: Yeni UI TextMeshPro";
            textMesh.fontSize = 24;
            textMesh.alignment = TextAlignmentOptions.Center;

            // Seçili nesneyi kontrol et ve altına yerleştir, yoksa Canvas'a yerleştir
            Transform parentTransform = Selection.activeGameObject != null ? Selection.activeGameObject.transform : canvas.transform;
            newObj.transform.SetParent(parentTransform, false);

            // Dil paketini obje içerisine ekle
            newObj.AddComponent<CustomLocalizationText>();

            // Ses objesini ekle
            AddAudioSystem(newObj);

            // Text tipini belirleme
            var textType = newObj.GetComponent<CustomLocalizationText>();
            textType.textType = TextType.UI;

            // Nesneyi otomatik seç
            Selection.activeGameObject = newObj;
        }

        // Sahne Text bileşeni
        [MenuItem("GameObject/BOBU/Localization/Text/3D-TextMeshPro", false, 10)]
        private static void Create3DTextMeshPro()
        {
            // Yeni 3D TextMeshPro nesnesi oluştur
            GameObject newObj = new GameObject("3D-TextMeshPro");
            TextMeshPro textMesh = newObj.AddComponent<TextMeshPro>();
            textMesh.text = "BOBU TEAM: Yeni 3D TextMeshPro";
            textMesh.fontSize = 5;
            textMesh.alignment = TextAlignmentOptions.Center;

            // Varsayılan olarak sahnede görülebilmesi için biraz yukarı taşı
            newObj.transform.position = new Vector3(0, 0, 0);

            // Eğer sahnede bir obje seçiliyse onun altına ekle
            if (Selection.activeGameObject != null)
                newObj.transform.SetParent(Selection.activeGameObject.transform, false);

            // Dil paketini obje içerisine ekle
            newObj.AddComponent<CustomLocalizationText>();

            // Ses objesini ekle
            AddAudioSystem(newObj);

            // Text tipini belirleme
            var textType = newObj.GetComponent<CustomLocalizationText>();
            textType.textType = TextType.Text3D;

            // Nesneyi otomatik seç
            Selection.activeGameObject = newObj;
        }

        // UI Text'leri için "Audio" bileşeni olmadan text oluşturma
        [MenuItem("GameObject/BOBU/Localization/Text/UI-TextMeshPro(None Audio)", false, 10)]
        private static void CreateUITextMeshProNoneAudio()
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                // Eğer sahnede bir Canvas yoksa, yeni bir tane oluştur
                GameObject canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();

                // EventSystem kontrolü yap ve yoksa oluştur
                if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
                {
                    GameObject eventSystem = new GameObject("EventSystem");
                    eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                    eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                }
            }

            // Yeni UI-TextMeshPro nesnesi oluştur
            GameObject newObj = new GameObject("UI-TextMeshPro");
            TextMeshProUGUI textMesh = newObj.AddComponent<TextMeshProUGUI>();
            textMesh.text = "BOBU TEAM: Yeni UI TextMeshPro";
            textMesh.fontSize = 24;
            textMesh.alignment = TextAlignmentOptions.Center;

            // Seçili nesneyi kontrol et ve altına yerleştir, yoksa Canvas'a yerleştir
            Transform parentTransform = Selection.activeGameObject != null ? Selection.activeGameObject.transform : canvas.transform;
            newObj.transform.SetParent(parentTransform, false);

            // Dil paketini obje içerisine ekle
            newObj.AddComponent<CustomLocalizationText>();

            // Text tipini belirleme
            var textType = newObj.GetComponent<CustomLocalizationText>();
            textType.textType = TextType.UI;

            // Nesneyi otomatik seç
            Selection.activeGameObject = newObj;
        }

        // UI Text'leri için "Audio" bileşeni olmadan text oluşturma
        [MenuItem("GameObject/BOBU/Localization/Text/3D-TextMeshPro(None Audio)", false, 10)]
        private static void Create3DTextMeshProNoneAudio()
        {
            // Yeni 3D TextMeshPro nesnesi oluştur
            GameObject newObj = new GameObject("3D-TextMeshPro");
            TextMeshPro textMesh = newObj.AddComponent<TextMeshPro>();
            textMesh.text = "BOBU TEAM: Yeni 3D TextMeshPro";
            textMesh.fontSize = 5;
            textMesh.alignment = TextAlignmentOptions.Center;

            // Varsayılan olarak sahnede görülebilmesi için biraz yukarı taşı
            newObj.transform.position = new Vector3(0, 0, 0);

            // Eğer sahnede bir obje seçiliyse onun altına ekle
            if (Selection.activeGameObject != null)
                newObj.transform.SetParent(Selection.activeGameObject.transform, false);

            // Dil paketini obje içerisine ekle
            newObj.AddComponent<CustomLocalizationText>();

            // Text tipini belirleme
            var textType = newObj.GetComponent<CustomLocalizationText>();
            textType.textType = TextType.Text3D;

            // Nesneyi otomatik seç
            Selection.activeGameObject = newObj;
        }

        // Ses öğelerini ekleme işlemi
        private static void AddAudioSystem(GameObject gameObject)
        {
            gameObject.AddComponent<AudioSource>().playOnAwake = false;
            gameObject.AddComponent<CustomLocalizationAudio>();
        }

        // Genel yönetici kodunu sahnede oluşturmaya yarar.
        [MenuItem("GameObject/BOBU/Localization/More/Localization Manager", false, 10)]
        private static void CreateLocalizationManager()
        {
            // Yeni 3D TextMeshPro nesnesi oluştur
            GameObject newObj = new GameObject("LocalizationManager");
            newObj.AddComponent<LocalizationManager>();
        }

    }
}