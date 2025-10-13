using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
//Tıkladıgımızda çiçeklerin veya dalların random sallanmasını saglayan kod.

public class FlowerSwing : MonoBehaviour
{
    [Header("Çiçek Transfor Listesi")]
    public List<Transform> cicekler = new List<Transform>();

    [Header("Salınım Ayar Aralığı")]
    public float minAngle = 10f;
    public float maxAngle = 20f;

    public float minDuration = 0.5f;
    public float maxDuration = 1.2f;

    private List<Quaternion> initialRotations = new List<Quaternion>();
    private bool hasStarted = false;

    private void Start()
    {
        foreach (var t in cicekler)
            initialRotations.Add(t.localRotation);
    }

    private void OnMouseDown()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (hasStarted) return;

        hasStarted = true;

        foreach (var flower in cicekler)
        {
            float randomAngle = Random.Range(minAngle, maxAngle);
            float randomDuration = Random.Range(minDuration, maxDuration);
            int randomDirection = Random.value > 0.5f ? 1 : -1;

            flower.DOLocalRotate(
                new Vector3(0, 0, randomAngle * randomDirection),
                randomDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < cicekler.Count; i++)
        {
            DOTween.Kill(cicekler[i]);
            cicekler[i].localRotation = initialRotations[i];
        }

        hasStarted = false;
    }
}
