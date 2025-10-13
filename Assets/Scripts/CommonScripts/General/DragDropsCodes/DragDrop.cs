using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class DragDrop : MonoBehaviour
{
    [Header("Distance")]
    public float distanceMeasurement = 2f;

    [Header("Layer")]
    [SerializeField] private int newLayer;
    [SerializeField] private int oldLayer;

    [Header("Puzzle Elements")]
    public List<Slots> puzzleSlots = new List<Slots>();
    [SerializeField] private int pieceId;
    public float placementDuration = 1f;
    public float scaleDuration = 1f;

    [Header("Vectors")]
    public Vector2 originalPos;
    private Vector3 offset;
    private Vector3 originalScale;

    [Header("Bools")]
    public bool isDragging;
    public bool isPlaced;

    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    private Slots filledSlot = null; // Hangi slota yerlestigini tutar



    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        originalPos = transform.localPosition;
        originalScale = transform.localScale;

        DOTween.Kill(transform);
        DOTween.Kill(spriteRenderer);

        spriteRenderer.enabled = true;
        col.enabled = true;

        isDragging = false;
        isPlaced = false;
        filledSlot = null;

        spriteRenderer.sortingOrder = oldLayer;

        transform.localPosition = new Vector3(originalPos.x, originalPos.y, -1f);
        transform.localScale = originalScale;
    }

    private void Update()
    {
        if (isPlaced) return;

        if (isDragging)
        {
            Vector2 touchPos = GetTouchPos();
            transform.position = new Vector3(touchPos.x - offset.x, touchPos.y - offset.y, -1f);
        }
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            spriteRenderer.sortingOrder = newLayer;
            isDragging = true;
            offset = GetTouchPos() - (Vector2)transform.position;
            offset.z = 0f;
        }
    }

    public virtual void OnMouseUp()
    {
        if (Input.GetMouseButtonUp(0))
        {
            bool isCorrectPlacement = false;

            foreach (Slots slot in puzzleSlots)
            {
                if (Vector2.Distance(transform.position, slot.transform.position) < distanceMeasurement)
                {
                    if (slot.isFilled) continue;

                    AudioManager.Instance.Play("ScoreSound");
                    isCorrectPlacement = true;
                    OnCorrectPlacement(slot);
                    break;
                }
            }

            if (!isCorrectPlacement)
            {
                OnIncorrectPlacement();
            }
        }
    }

    private Vector2 GetTouchPos()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public void OnCorrectPlacement(Slots puzzleSlot)
    {
        puzzleSlot.isFilled = true;
        filledSlot = puzzleSlot;

        transform.DOLocalMove(new Vector3(puzzleSlot.transform.position.x, puzzleSlot.transform.position.y, -1f), placementDuration);
        transform.DOScale(puzzleSlot.transform.localScale, scaleDuration);

        isPlaced = true;
        col.enabled = false;

        // Layer'i eski haline getir
        spriteRenderer.sortingOrder = oldLayer;

    }


    public void OnIncorrectPlacement()
    {
        spriteRenderer.sortingOrder = oldLayer;
        isDragging = false;

        Vector3 targetPos = new Vector3(originalPos.x, originalPos.y, -1f);

        transform.DOLocalMove(targetPos, placementDuration);
        transform.DOScale(originalScale, scaleDuration);
    }

    private void OnDisable()
    {
        DOTween.Kill(transform);
        DOTween.Kill(spriteRenderer);

        // Pozisyon ve scale sifirla
        transform.localPosition = new Vector3(originalPos.x, originalPos.y, -1f);
        transform.localScale = originalScale;

        // Collider ve layer sifirla
        if (col != null) col.enabled = true;
        if (spriteRenderer != null) spriteRenderer.sortingOrder = oldLayer;

        // Durumlar reset
        isDragging = false;
        isPlaced = false;

        // Slot bosalt
        if (filledSlot != null)
        {
            filledSlot.isFilled = false;
            filledSlot = null;
        }
    }
}
