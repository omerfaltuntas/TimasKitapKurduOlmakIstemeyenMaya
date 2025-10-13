using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbianceMusic : MonoBehaviour
{
    public string MusicName;
    void OnEnable()
    {
        Invoke(nameof(MusicOpen), 0.2f);
    }

    public void MusicOpen()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Play(MusicName);
        }
    }
    void OnDisable()
    {
        // Eger oyun kapanirken AudioManager bizden once yok olduysa
        // hata almamak icin null olup olmadigini kontrol et.
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Stop(MusicName);
        }
    }
}


