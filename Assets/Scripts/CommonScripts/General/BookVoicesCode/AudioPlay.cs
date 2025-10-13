using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Bu kod sayfa üzerinde ses caldirma kodudur.
public class AudioPlay : MonoBehaviour
{
    public string musicName;
    private void OnEnable()
    {
        AudioManager.Instance.Play(musicName);
    }

    private void OnDisable()
    {
        AudioManager.Instance.Stop(musicName);
    }
}
