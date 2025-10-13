using UnityEngine;
using DG.Tweening;
//Bu kod objenin ziplamasini saglayan koddur.

public class ClickJump : MonoBehaviour
{
    public float jumpPower = 0.1f;    // Ziplama yuksekligi
    public float duration = 0.3f;     // Animasyon suresi

    private Vector3 startPos;
    private bool hasStarted = false;

    void Awake()
    {
        startPos = transform.localPosition;
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0) && !hasStarted)
        {
            hasStarted = true;

            // Yukari-asagi loop'lu animasyon
            transform.DOLocalMoveY(startPos.y + jumpPower, duration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutQuad);
        }
    }

    private void OnDisable()
    {
        DOTween.Kill(transform);
        transform.DOLocalMove(startPos, 0.25f);
        hasStarted = false;
    }
}
