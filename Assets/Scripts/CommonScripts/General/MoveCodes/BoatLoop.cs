using UnityEngine;
using DG.Tweening;
//Bu script, bir tekne ve damla objesinin belirli noktalarda hareket edip fade animasyonlarıyla sonsuz döngüde gidip gelmesini sağlar. 
//Tıklamayla başlayan animasyonda, damla zıplar ve tekne görünmez olarak bir noktadan diğerine ışınlanır.


public class BoatLoop : MonoBehaviour
{
    [Header("Referanslar")]
    public Transform boat;
    public Transform drop;
    public SpriteRenderer boatSR;
    public SpriteRenderer dropSR;

    [Header("Targetlar (local)")]
    public Transform target1;
    public Transform target2;

    [Header("Sureler")]
    public float fadeSuresi = 0.2f;
    public float geriDonusSuresi = 2f;

    [Header("Damla Ziplama Ayarlari")]
    public float ziplamaYuksekligi = 0.5f;

    private Vector3 boatStartPos;
    private Vector3 dropStartPos;
    private Vector3 dropStartScale;
    private float boatStartAlpha;
    private float dropStartAlpha;

    private Sequence mainSeq;

    void Start()
    {
        if (boat == null || drop == null || boatSR == null || dropSR == null || target1 == null || target2 == null)
        {
            Debug.LogError("Lutfen tum referanslari Inspector uzerinden atayin!", this);
            enabled = false;
            return;
        }

        boatStartPos = boat.localPosition;
        dropStartPos = drop.localPosition;
        dropStartScale = drop.localScale;
        boatStartAlpha = boatSR.color.a;
        dropStartAlpha = dropSR.color.a;
    }

    void OnMouseDown()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        DOTween.Kill(transform, true); // GÜNCELLEME: true parametresi OnComplete'leri de aninda calistirir.
        ResetToInitialState();

        // Ana akisi yonetecek olan sekans
        mainSeq = DOTween.Sequence().SetTarget(transform);

        // --- GÜNCELLEME: 1. SADECE BİR KEZ ÇALIŞACAK KISIM ---
        // 0) start -> target1 (gorunur gidis + zip)
        mainSeq.Append(boat.DOLocalMove(target1.localPosition, geriDonusSuresi).SetEase(Ease.Linear));
        mainSeq.Join(DamlaZiplamaAnimasyonu(geriDonusSuresi));


        // --- GÜNCELLEME: 2. SONSUZ DÖNGÜYE GİRECEK KISIM ---
        // Donguye girecek adimlari ayri bir sekansa aliyoruz.
        Sequence loopSeq = DOTween.Sequence();

        // A) target1'de fade-out
        loopSeq.Append(boatSR.DOFade(0f, fadeSuresi).SetEase(Ease.Linear));
        loopSeq.Join(dropSR.DOFade(0f, fadeSuresi).SetEase(Ease.Linear));

        // B) gorunmezken target2'ye isinla
        loopSeq.AppendCallback(() => { boat.localPosition = target2.localPosition; });

        // C) target2'de fade-in
        loopSeq.Append(boatSR.DOFade(boatStartAlpha, fadeSuresi).SetEase(Ease.Linear));
        loopSeq.Join(dropSR.DOFade(dropStartAlpha, fadeSuresi).SetEase(Ease.Linear));

        // D) target2 -> target1 (gorunur gidis + zip)
        loopSeq.Append(boat.DOLocalMove(target1.localPosition, geriDonusSuresi).SetEase(Ease.Linear));
        loopSeq.Join(DamlaZiplamaAnimasyonu(geriDonusSuresi));

        // GÜNCELLEME: Donguyu sadece 'loopSeq' icin ayarliyoruz.
        loopSeq.SetLoops(-1, LoopType.Restart);

        // GÜNCELLEME: Sonsuz donguyu ana sekansin sonuna ekliyoruz.
        mainSeq.Append(loopSeq);
    }

    void OnDisable()
    {
        DOTween.Kill(transform);
        ResetToInitialState();
    }

    private Tween DamlaZiplamaAnimasyonu(float toplamSure)
    {
        float periyot = 0.30f;
        int hop = Mathf.Max(1, Mathf.RoundToInt(toplamSure / periyot));
        float half = toplamSure / (hop * 2f);

        Vector3 p = drop.localPosition;
        p.y = dropStartPos.y;
        drop.localPosition = p;

        return drop.DOLocalMoveY(dropStartPos.y + ziplamaYuksekligi, half)
                   .SetEase(Ease.OutQuad)
                   .SetLoops(hop * 2, LoopType.Yoyo)
                   .SetTarget(transform);
    }

    private void ResetToInitialState()
    {
        // Reset metodunda bir degisiklik yok, ayni kaliyor.
        if (boat != null) boat.localPosition = boatStartPos;
        if (drop != null)
        {
            drop.localPosition = dropStartPos;
            drop.localScale = dropStartScale;
        }
        if (boatSR != null)
        {
            var c = boatSR.color; c.a = boatStartAlpha; boatSR.color = c;
        }
        if (dropSR != null)
        {
            var c2 = dropSR.color; c2.a = dropStartAlpha; dropSR.color = c2;
        }
    }
}
