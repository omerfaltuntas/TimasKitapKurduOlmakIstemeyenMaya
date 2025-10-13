using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Bu kod tıkladimiz objenin ses calmasini istedgimiz koddur.

public class ClickVoice : MonoBehaviour
{
    public string soundName;
    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.Play(soundName);

            }
        }

    }
    private void OnDisable()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Stop(soundName);
        }
    }

}
