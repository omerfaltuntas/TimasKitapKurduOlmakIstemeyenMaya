using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Bu kod ısıgı ac-kapa yapmamızı saglayan koddur.
public class LightActivator : MonoBehaviour
{
    [Header("Lamba isigi objesi")]
    public GameObject lampLight;
    public string soundName;

    private bool isLightOn = false;

    private void Start()
    {
        // Baslangicta isigi kapatir
        if (lampLight != null)
            lampLight.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            AudioManager.Instance.Play(soundName);
            // Işık durumunu tersine cevirir
            isLightOn = !isLightOn;

            // Işık objesini aktif/pasif yapar
            if (lampLight != null)
                lampLight.SetActive(isLightOn);
        }
    }
    private void OnDisable()
    {
        // Baslangicta isigi kapatir
        if (lampLight != null)
            lampLight.SetActive(false);
    }
}
