using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Update uzerine atilan ogenin ekseni etrafinda donusunu saglayan kod.
public class TurnController : MonoBehaviour
{
    public float turnSpeed = 30f;

    void Update()
    {
        transform.Rotate(Vector3.forward * turnSpeed * Time.deltaTime);
    }
}
