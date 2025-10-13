using UnityEngine;
using System;

public class TutorialClickReceiver : MonoBehaviour
{
    private Action onClick;

    private void Start()
    {
        // Eger objenin Z pozisyonu 0 ise, -1 yap
        Vector3 pos = transform.localPosition;
        if (Mathf.Approximately(pos.z, 0f))
        {
            pos.z = -1f;
            transform.localPosition = pos;
        }
    }

    // Disaridan tetiklenecek callback baglanir
    public void Assign(Action callback)
    {
        onClick = callback;
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (onClick == null)
            {
                Debug.LogWarning($"[TutorialClickHandler] onClick atanmadı: {gameObject.name}");
                return;
            }

            onClick.Invoke();
        }
    }
}
