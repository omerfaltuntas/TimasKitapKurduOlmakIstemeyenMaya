using UnityEngine;

public class PinchToZoomCamera : MonoSingleton<PinchToZoomCamera>
{

    public float startSize;
    public float zoomOutMin = 1;
    public float zoomOutMax;
    public static bool isZoomActive;

    Vector3 _touchStart;
    private PageSwiper pageSwiper;

    private void Start()
    {
        pageSwiper = GetComponent<PageSwiper>();
        zoomOutMax = Camera.main.orthographicSize;

        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = 16.0f / 9.0f;
        float difference = targetRatio / screenRatio;
        startSize = Camera.main.orthographicSize;
    }

    public void ZoomActiverDeactiver()
    {
        isZoomActive = !isZoomActive;
        pageSwiper.enabled = !pageSwiper.enabled;

        GameObject myObject = pageSwiper.pages[PageSwiper.Instance.currentPageIndex];
        Collider2D[] colliders = myObject.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = !isZoomActive;
        }

        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = 16.0f / 9.0f;
        float difference = targetRatio / screenRatio;
        Camera.main.transform.position = new Vector3(0, 0, -10);

        if (screenRatio <= targetRatio)
        {
            Camera.main.orthographicSize = 6.5f * difference;
        }
        else
        {
            Camera.main.orthographicSize = 6.5f;
        }
    }
    void Update()
    {
        if (isZoomActive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            if (Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

                float difference = currentMagnitude - prevMagnitude;

                zoom(difference * 0.01f);
            }
            else if (Input.GetMouseButton(0))
            {
                Vector3 direction = _touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Camera.main.transform.position += direction;
            }
            zoom(Input.GetAxis("Mouse ScrollWheel"));
        }
    }
    void zoom(float increment)
    {
        float newSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);

        Camera.main.orthographicSize = newSize;

        // Kameranın boyutu değişirken sınırları kontrol et
        Vector3 cameraPosition = Camera.main.transform.position;

        float minXLimit = -((startSize / Camera.main.orthographicSize) + (startSize - Camera.main.orthographicSize));
        float maxXLimit = ((startSize / Camera.main.orthographicSize) + (startSize - Camera.main.orthographicSize));
        float minYLimit = -((startSize / Camera.main.orthographicSize) + (startSize - Camera.main.orthographicSize)) / 2;
        float maxYLimit = ((startSize / Camera.main.orthographicSize) + (startSize - Camera.main.orthographicSize)) / 2;

        cameraPosition.x = Mathf.Clamp(cameraPosition.x, minXLimit, maxXLimit);
        cameraPosition.y = Mathf.Clamp(cameraPosition.y, minYLimit, maxYLimit);

        Camera.main.transform.position = cameraPosition;
    }
}