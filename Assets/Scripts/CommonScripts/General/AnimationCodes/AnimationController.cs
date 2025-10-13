using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//Animasyonları yonettigimiz kod.

public class AnimationController : MonoBehaviour
{
    public Animator anim;
    public string animBool;
    private float animationDuration;
    public Transform startPos;

    public bool isRepeatable;

    private void OnDisable()
    {
        anim.SetBool(animBool, false);
        GetComponent<BoxCollider2D>().enabled = true;
        if (startPos != null)
        {
            transform.position = startPos.position;
        }
    }

    private void Start()
    {
        if (startPos != null)
        {
            transform.position = startPos.position;
        }

        // İlk animasyon klibini al
        AnimationClip clip = anim.runtimeAnimatorController.animationClips[0];

        // Animasyon süresini al ve float değerine çevir
        animationDuration = clip.length;

        print("duration" + animationDuration.ToString());
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(AnimationFalseWait());
        }

    }

    IEnumerator AnimationFalseWait()
    {
        if (isRepeatable)
        {
            GetComponent<BoxCollider2D>().enabled = false;
            anim.SetBool(animBool, true);
            yield return new WaitForSeconds(animationDuration);
            GetComponent<BoxCollider2D>().enabled = true;
            anim.SetBool(animBool, false);
        }

        else
        {
            GetComponent<BoxCollider2D>().enabled = false;
            anim.SetBool(animBool, true);
            yield return new WaitForSeconds(animationDuration);
            GetComponent<BoxCollider2D>().enabled = true;
        }
    }

}

