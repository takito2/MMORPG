using Common.Data;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestSystem : UIWindow {
    public Text title;

    public GameObject itemPrefab;

    public TabView Tabs;
    public ListView listMain;
    public ListView listBranch;

    public UIQuestInfo qusetInfo;

    private bool showAvailableList = false;//是否显示可接任务，默认显示进行中，故false

	// Use this for initialization
	void Start () {
        this.listMain.onItemSelected += this.OnQuestSelected;
        this.listBranch.onItemSelected += this.OnQuestSelected;
        this.Tabs.OnTabSelect += OnSelectTab;
        RefreshUI();
       		
	}

    private void OnSelectTab(int idx)
    {
        showAvailableList = idx == 1;//为1即显示可接
        RefreshUI();

    }

    private void OnDestroy()
    {
        
    }

    void RefreshUI()
    {
        ClearAllQuestList();
        InitAllQuestItems();
    }

    void ClearAllQuestList()
    {
        this.listMain.RemoveAll();
        this.listBranch.RemoveAll();
    }

    void InitAllQuestItems()
    {
        foreach (var kv in QuestManager.Instance.allQuests)
        {
            if (showAvailableList)
            {
                if (kv.Value.Info != null)//不为空说明已经获取到了网络信息，已经接取，需跳过
                {
                    continue;
                }
            }
            else//进行中且已获取网络信息
            {
                if (kv.Value.Info == null)
                {
                    continue;
                }
            }
            //可接且未获取信息||进行中且已获取网络信息 将初始化
            GameObject go = Instantiate(itemPrefab, kv.Value.Define.Type == QuestType.Main ? this.listMain.transform : this.listBranch.transform);
            UIQuestItem ui = go.GetComponent<UIQuestItem>();
            ui.SetQuestInfo(kv.Value);
            if (kv.Value.Define.Type == QuestType.Main)
                this.listMain.AddItem(ui as ListView.ListViewItem);
            else
                this.listBranch.AddItem(ui as ListView.ListViewItem);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void OnQuestSelected(ListView.ListViewItem item)//当任务在列表被选择时被调用，显示任务信息至右边详情列表
    {
        UIQuestItem questItem = item as UIQuestItem;
        this.qusetInfo.SetQuestInfo(questItem.quest);   
    }
}
