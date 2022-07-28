using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Managers;
using Models;
using System;
using SkillBridge.Message;
using Assets.Scripts.Models;

public class UICharEquip : UIWindow {
    public Text title;
    public Text money;

    public GameObject itemPrefab;
    public GameObject itemEquipedPrefab;

    public Transform itemListRoot;
    public List<Transform> slots;

	// Use this for initialization
	void Start () {
        RefreshUI();
        EquipManager.Instance.OnEquipChanged += RefreshUI;
	}

    private void OnDestroy()
    {
        EquipManager.Instance.OnEquipChanged -= RefreshUI;
    }

    void RefreshUI()
    {
        ClearAllEquipList();//左边装备列表清空
        InitAllEquipItems();//初始化左边装备列表
        ClearEquipedList();//清空已装备槽位
        InitEquipedItems();//初始化已装备槽位
        this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
    }

    private void ClearAllEquipList()//销毁
    {
        foreach (var item in itemListRoot.GetComponentsInChildren<UIEquipItem>())
        {
            Destroy(item.gameObject);
        }
    }

    /// <summary>
    /// 初始化装备列表
    /// </summary>
    private void InitAllEquipItems()
    {
        foreach (var kv in ItemManager.Instance.Items)
        {
            if (kv.Value.Define.Type == ItemType.Equip && kv.Value.Define.LimitClass == User.Instance.CurrentCharacter.Class)
            {
                if (EquipManager.Instance.Contains(kv.Key))//跳过已经装备到slot的
                    continue;
                GameObject go = Instantiate(itemPrefab, itemListRoot);//实例化
                UIEquipItem ui = go.GetComponent<UIEquipItem>();
                ui.SetEquipItem(kv.Key, kv.Value, this, false);
            }
            
        }
    }

    private void ClearEquipedList()//清空槽位装备
    {
        foreach (var item in slots)
        {
            if (item.childCount > 0)
                Destroy(item.GetChild(0).gameObject);
        }
    }

    /// <summary>
    /// 初始化已装备列表
    /// </summary>
    private void InitEquipedItems()
    {
        for (int i = 0; i < (int)EquipSlot.SlotMax; i++)
        {
            var item = EquipManager.Instance.Equips[i];
            {
                if (item != null)
                {
                    GameObject go = Instantiate(itemEquipedPrefab, slots[i]);
                    UIEquipItem ui = go.GetComponent<UIEquipItem>();
                    ui.SetEquipItem(i, item, this, true);
                }
            }
        }
    }

    public void DoEquip(Item item)//装备道具
    {
        EquipManager.Instance.EquipItem(item);
    }

    public void UnEquip(Item item)
    {
        EquipManager.Instance.UnEquipItem(item);
    }
}
