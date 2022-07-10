using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public bool global = true;
    static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType<T>();
            }
            return instance;
        }

    }

    void Awake()
    {
        if (global)
        {
            if(instance != null && instance != this.gameObject.GetComponent<T>())
            {
                Debug.LogFormat("MonoSingleton:{0}",this.gameObject.GetComponent<T>().ToString());
                Destroy(this.gameObject);
                return;
            }
            DontDestroyOnLoad(this.gameObject);
            instance = this.gameObject.GetComponent<T>();
        }
        this.OnStart();
    }

    protected virtual void OnStart()
    {

    }

}

//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
//{
//    private static T instance;

//    public static T Instance
//    {
//        get
//        {
//            return instance;
//        }
//    }

//    public static bool IsInit
//    {
//        get
//        {
//            return instance != null;
//        }
//    }

//    protected virtual void Awake()
//    {
//        if (instance != null)
//        {
//            Destroy(gameObject);
//        }
//        else
//        {
//            instance = (T)this;
//        }
//    }

//    protected virtual void OnDestroy()
//    {
//        if (instance == this)
//        {
//            instance = null;
//        }
//    }


//}