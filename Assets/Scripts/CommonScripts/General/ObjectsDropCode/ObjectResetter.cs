using UnityEngine;
using DG.Tweening;

public class ObjectResetter : MonoBehaviour
{
    public Vector3 initialLocalPos;

    private void OnEnable()
    {
        // Disable olduktan sonra tekrar aktif olursa yeniden düşür
        transform.localPosition += new Vector3(0, 3f, 0);
        transform.DOLocalMoveY(initialLocalPos.y, 2f).SetEase(Ease.OutBounce);
    }


    private void OnDisable()
    {
        // Tween'i durdur
        DOTween.Kill(transform);

        // Yerel pozisyona sifirla
        if (transform.parent != null)
            transform.localPosition = initialLocalPos;
        else
            transform.position = initialLocalPos;
    }

}
