using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static volatile T instance = null;

    public static T Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType(typeof(T)) as T;

            return instance;
        }
    }

    // DontDestroyOnLoad özelliğini kontrol etmek için bir bool parametresi ekleyin.
    protected bool useDontDestroyOnLoad = false;

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;

            // useDontDestroyOnLoad true ise DontDestroyOnLoad uygula.
            if (useDontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
