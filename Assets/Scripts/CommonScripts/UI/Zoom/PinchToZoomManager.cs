using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PinchToZoomManager : MonoBehaviour, IPointerDownHandler
{
    [Header("Settings")]
    [SerializeField] private Sprite[] zoomSprites;
    private Image image;
    private bool zoom;

    void Start()
    {
        image = GetComponent<Image>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PinchToZoomCamera.Instance.ZoomActiverDeactiver();

        zoom = !zoom;
        image.sprite = zoom ? zoomSprites[1] : zoomSprites[0];
    }
}