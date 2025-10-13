using System.Collections;
using UnityEngine;
using DG.Tweening;

//Bu kod objenin acilmasini saglayan koddur.
public class GameObjectOpen : MonoBehaviour
{
    public GameObject objects;

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (objects != null)
            {
                objects.SetActive(true);

            }
        }
    }


    private void OnDisable()
    {
        if (objects != null)
        {
            objects.SetActive(false);

        }
    }
}
