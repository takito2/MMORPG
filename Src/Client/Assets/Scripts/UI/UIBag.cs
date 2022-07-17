using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBag : UIWindow {

    public Text Money;

    public Transform[] pages;

    public GameObject bagItem;

    List<Image> slots;
	// Use this for initialization
	void Start () {
        if (slots == null)
        {
            slots = new List<Image>();
            for (int page = 0; page < this.pages.Length; page++)
            {
                slots.AddRange(this.pages[page].GetComponentsInChildren<Image>(true));
            }
        }
        SetTitle();
        StartCoroutine(InitBags());

	}
	
    IEnumerator InitBags()
    {
        for (int i = 0; i < BagManager.Instance.Items.Length; i++)//根据BagManager内Items内信息将已有item信息反应在背包
        {
            var item = BagManager.Instance.Items[i];//重要！！！，先于此处取到该格子下属脚本，内含图片路径及数量
            if (item.ItemId > 0 )//判断Itemid有效
            {
                GameObject go = Instantiate(bagItem, slots[i].transform);//在对应格子生成slot预制体
                var ui = go.GetComponent<UIIconItem>();//获取预制体上UIIconItem脚本
                var def = ItemManager.Instance.Items[item.ItemId].Define;//获取对应格子内道具item信息
                ui.SetMainIcon(def.Icon, item.Count.ToString());//赋值sprite及Num
            }
        }
        for (int i = BagManager.Instance.Items.Length; i < slots.Count; i++)//将实有道具格子后的格子置灰
        {
            slots[i].color = Color.gray;
        }
        yield return null;
    }

    public void SetTitle()
    {
        this.Money.text = User.Instance.CurrentCharacter.Gold.ToString();
    }

    public void Clear()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].transform.childCount > 0)
            {
                Destroy(slots[i].transform.GetChild(0).gameObject);
            }
        }
    }

    public void OnReset()
    {
        BagManager.Instance.Reset();
        this.Clear();
        StartCoroutine(InitBags());
    }


}
