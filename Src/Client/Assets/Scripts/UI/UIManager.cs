using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager> {

    class UIElement
    {
        public string Resources;//UI预制体资源路径
        public bool Cache;//判断关闭UI销毁或隐藏
        public GameObject Instance;//UI实例
    }
    private Dictionary<Type, UIElement> UIResources = new Dictionary<Type, UIElement>();
	// Use this for initialization
	public UIManager()//构造函数，包括初始化管理UI
    {
        UIResources.Add(typeof(UITest), new UIElement() {Resources = "UI/UITest",Cache = true });
        UIResources.Add(typeof(UIBag), new UIElement() { Resources = "UI/UIBag", Cache = false });
        UIResources.Add(typeof(UIShop), new UIElement() { Resources = "UI/UIShop", Cache = false });
        UIResources.Add(typeof(UICharEquip), new UIElement() { Resources = "UI/UICharEquip", Cache = false });
        UIResources.Add(typeof(UIQuestSystem), new UIElement() { Resources = "UI/UIQuestSystem", Cache = false });
        UIResources.Add(typeof(UIQuestDialog), new UIElement() { Resources = "UI/UIQuestDialog", Cache = false });
    }
	
	public T Show<T>()//泛型展示类
    {
        Type type = typeof(T);//获取泛型类型，一般为UI类名，作为字典的Key
        if (UIResources.ContainsKey(type))//查询维护字典是否包含该Key
        {
            UIElement info = UIResources[type];//获取该Key对应Value
            if (info.Instance != null)//当前UIElement有实例，直接激活
            {
                info.Instance.SetActive(true);
            }
            else//无实例流程
            {
                //GameObject Ins = (GameObject)Resources.Load(info.Resources);
                //info.Instance = GameObject.Instantiate(Ins);
                UnityEngine.Object prefab = Resources.Load(info.Resources);//加载预制体
                if (prefab == null)//判空
                {
                    return default(T);
                }
                info.Instance = (GameObject)GameObject.Instantiate(prefab);//实例化并复制于实例
            }
            return info.Instance.GetComponent<T>();//返回实例下组件（原T类型）
        }
        return default(T);
    }

    public void Close(Type type)//关闭方法
    {
        if (this.UIResources.ContainsKey(type))//字典判断是否包含
        {
            UIElement info = UIResources[type];//获取value
            if (info.Cache)//Cache == true 表示保留，仅隐藏，不删除
            {
                info.Instance.SetActive(false);
            }
            else
            {
                GameObject.Destroy(info.Instance);
                info.Instance = null;
            }
        }
    }
}
