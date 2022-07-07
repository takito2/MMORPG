using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class Singleton<T> where T : new()
{
    static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
            }
            return instance;
        }

    }
}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Singleton<T> : MonoBehaviour where T : Singleton<T>
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
