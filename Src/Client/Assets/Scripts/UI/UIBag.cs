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
        StartCoroutine(InitBags());

	}
	
    IEnumerator InitBags()
    {
        for (int i = 0; i < BagManager.Instance.Items.Length; i++)//根据BagManager内Items内信息将已有item信息反应在背包
        {
            var item = BagManager.Instance.Items[i];
            if (item.ItemId > 0 )//判断Itemid有效
            {
                GameObject go = Instantiate(bagItem, slots[i].transform);
                var ui = go.GetComponent<UIIconItem>();
                var def = ItemManager.Instance.Items[item.ItemId].Define;
                ui.SetMainIcon(def.Icon, item.Count.ToString());
            }
        }
        for (int i = BagManager.Instance.Items.Length; i < slots.Count; i++)//将实有道具格子后的格子置灰
        {
            slots[i].color = Color.gray;
        }
        yield return null;
    }

    public void SetTitle(string title)
    {
        this.Money.text = User.Instance.CurrentCharacter.Id.ToString();
    }

    public void OnReset()
    {
        BagManager.Instance.Reset();
    }

	// Update is called once per frame
	void Update () {
		
	}
}
