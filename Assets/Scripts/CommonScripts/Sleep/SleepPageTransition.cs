using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// * Sayfaya ait seslerin sırasıyla çalınmasını sağlar.
/// * Tüm sesler tamamlandığında sayfa geçişi yapılabilir.
/// </summary> 

public class SleepPageTransition : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Sayfaya ait sesleri sırasıyla listeye ekle. Kendisi sırasıyla sesleri çalacak ve tamamlandığında sayfa geçişi yapmasını sağlayacak.")]
    [SerializeField] private List<AudioSource> audioSources = new List<AudioSource>();

    private int currentIndex = 0;

    /// <summary>
    /// * Sayfa aktif olduğunda sıralı ses oynatma işlemi başlar.
    /// </summary>
    private void OnEnable()
    {
        currentIndex = 0;

        if (audioSources.Count > 0)
        {
            StartCoroutine(PlayAudioSequence());
        }
        else
        {
            Debug.LogWarning("AudioSource listesi boş. Ses çalma işlemi başlatılamadı.");
        }
    }

    /// <summary>
    /// * Listedeki sesleri sırayla çalar ve her biri tamamlandığında bir sonrakine geçer.
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator PlayAudioSequence()
    {
        while (currentIndex < audioSources.Count)
        {
            AudioSource currentSource = audioSources[currentIndex];

            // Ses kaynağı ya da klibi eksikse uyarı ver ve sıradakine geç
            if (currentSource == null || currentSource.clip == null)
            {
                Debug.LogWarning($"[{currentIndex}] AudioSource ya da AudioClip atanmadı, atlanıyor.");
                currentIndex++;
                continue;
            }

            currentSource.Play();
            yield return new WaitForSeconds(currentSource.clip.length);
            currentIndex++;
        }

        Debug.Log("Tüm sesler tamamlandı. Sayfa geçişi yapılabilir.");
        // Burada sayfa geçişi tetiklenebilir (örneğin: PageSwiper.Instance.GoToNextPage();)
        SleepSwiper.Instance.NextPage();
        GeneralPageManager.Instance.onDataUpdate?.Invoke();
    }
}
