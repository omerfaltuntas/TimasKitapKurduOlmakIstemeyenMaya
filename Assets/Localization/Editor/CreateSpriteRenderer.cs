using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AgeOfKids.Localization
{
    public class CreateSpriteRenderer : MonoBehaviour
    {
        // Sprite bileşeni
        [MenuItem("GameObject/BOBU/Localization/Sprite/Sprite Renderer", false, 10)]
        public static void CreateSprite()
        {
            // Yeni obje ekle
            GameObject newoObj = new GameObject("Sprite Renderer");

            // Sprite ekle
            SpriteRenderer spriteRenderer = newoObj.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = 1;

            // Sprite kodunu ekle
            newoObj.AddComponent<CustomLocalizationSprite>();

            // Nesneyi otomatik seç
            Selection.activeGameObject = newoObj;

            // Sesi üret
            AddAudioSystem(newoObj);
        }


        // Ses öğelerini ekleme işlemi
        private static void AddAudioSystem(GameObject gameObject)
        {
            gameObject.AddComponent<AudioSource>().playOnAwake = false;
            gameObject.AddComponent<CustomLocalizationAudio>();
        }
    }
}
