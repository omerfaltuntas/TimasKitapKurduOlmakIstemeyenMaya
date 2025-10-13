using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
// Bu script, iki objenin (drop1 ve drop2) tıklamayla düşme (drop) animasyonunu yönetir.
public class DropSequence : MonoBehaviour
{
    public SpriteRenderer drop1;
    public SpriteRenderer drop2;

    public Transform drop1Pos;
    public Transform drop2Pos;

    public float drop1TargetY;
    public float drop2TargetY;

    public float delay;   // drop1 animasyon süresi
    public float delay2;  // drop2 animasyon süresi

    private bool tiklanabilir = true;

    void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0) && tiklanabilir)
        {
            tiklanabilir = false;

            if (drop1 != null)
            {
                drop1.transform.DOLocalMoveY(drop1TargetY, delay)
                    .SetEase(Ease.OutBounce);
            }

            if (drop2 != null)
            {
                // 0.3 saniye sonra düşmeye başlasın ama animasyon anında hazır
                DOVirtual.DelayedCall(0.3f, () =>
                {
                    drop2.transform.DOLocalMoveY(drop2TargetY, delay2)
                        .SetEase(Ease.OutBounce)
                        .OnComplete(() =>
                        {
                            tiklanabilir = true;
                        });
                });
            }
            else
            {
                DOVirtual.DelayedCall(delay + 0.5f, () => tiklanabilir = true);
            }
        }
    }

    void OnDisable()
    {
        if (drop1 != null)
        {
            DOTween.Kill(drop1.transform);
            drop1.transform.localPosition = drop1Pos.localPosition;
        }

        if (drop2 != null)
        {
            DOTween.Kill(drop2.transform);
            drop2.transform.localPosition = drop2Pos.localPosition;
        }

        tiklanabilir = true;
    }
}
