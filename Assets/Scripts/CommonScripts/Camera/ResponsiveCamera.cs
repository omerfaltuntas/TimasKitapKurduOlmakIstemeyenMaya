using UnityEngine;

public class ResponsiveCamera : MonoBehaviour
    {
        public float tabletSize;
        public float phoneSize;
        public float tabletPhoneSize;
        public float undefinedDevicesSize;

        float height;

        void Start()
        {
            float screenRatio = (float)Screen.width / (float)Screen.height;

            if (screenRatio <= 1.5 && screenRatio >= 1)
            {
                print("tablet ekran�");
                height = tabletSize;
            }
            else if (screenRatio <= 2.5 && screenRatio >= 2)
            {
                print("telefon ekran�");
                height = phoneSize;
            }
            else if (screenRatio <= 1.9 && screenRatio >= 1.6)
            {
                print("telefon tablet ekran�");
                height = tabletPhoneSize;
            }
            else
            {
                print("Bilinmeyen cihaz boyutu");
                height = undefinedDevicesSize;
            }

            Camera.main.orthographicSize = height / 2.0f;

            if (screenRatio >= 1.0)
            {
                Camera.main.orthographicSize = height / 2.0f;
            }
            else
            {
                float targetWidth = height * screenRatio;
                Camera.main.orthographicSize = targetWidth / 2.0f;
            }

            print(screenRatio);


        }
    }