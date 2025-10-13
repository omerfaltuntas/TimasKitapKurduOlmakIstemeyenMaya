using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
//Bu kod particle'ı calıstıran koddur. Particle'da Play On Awake açık olmalı.

public class ParticleController : MonoBehaviour
{
    public GameObject particle;        // Efekt objesi

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (particle != null)
                particle.SetActive(true);

        }
    }

    private void OnDisable()
    {
        if (particle != null)
            particle.SetActive(false);

    }
}
