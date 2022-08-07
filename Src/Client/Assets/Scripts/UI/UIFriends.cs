using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Models;
using Services;
using System;

public class UIFriends : UIWindow {

    public GameObject itemPrefab;
    public ListView listMain;
    public Transform itemRoot;
    public UIFriendItem selectedItem;
    public bool Opening = false;

	// Use this for initialization
	void Start () {
        Opening = true;
        FriendService.Instance.OnFriendUpdate = RefreshUI;
        this.listMain.onItemSelected += this.OnFriendSelected;//绑定事件，获取选中item
        RefreshUI();

	}

    private void OnFriendSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIFriendItem;
    }

    public void OnClickFriendAdd()
    {
        InputBox.Show("输入要添加的好友名称或ID", "添加好友").OnSubmit += OnFriendAddSubmit;
    }

    private bool OnFriendAddSubmit(string input, out string tips)//添加好友提交执行逻辑
    {
        tips = "";
        int friendId = 0;
        string frinendName = "";
        if (!int.TryParse(input, out friendId))
            frinendName = input;
        if (friendId == User.Instance.CurrentCharacter.Id || frinendName == User.Instance.CurrentCharacter.Name)
        {
            tips = "不能添加自己哦！";
            return false;
        }

        FriendService.Instance.SendFriendAddRequest(friendId, frinendName);
        return true;
    }

    public void ObClickFriendChat()
    {
        MessageBox.Show("暂未开放");
    }

    public void OnClickFriendRemove()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要删除的好友");
            return;
        }
        MessageBox.Show(string.Format("确定要删除好友【{0}】吗？",selectedItem.Info.friendInfo.Name),"删除好友",MessageBoxType.Confirm,"删除","取消").OnYes =()=> {
            //参数为好友记录的Id及好友的Id
            FriendService.Instance.SendFriendRemoveRequest(this.selectedItem.Info.Id, this.selectedItem.Info.friendInfo.Id);
        };
    }

    void RefreshUI()
    {
        if(Opening)
        ClearFriendList();
        InitFriendItems();
    }

    /// <summary>
    /// 初始化好友列表
    /// </summary>
    private void InitFriendItems()
    {
        foreach (var item in FriendManager.Instance.allFriends)
        {
            GameObject go = Instantiate(itemPrefab, this.listMain.transform);
            UIFriendItem ui = go.GetComponent<UIFriendItem>();
            ui.SetFriendInfo(item);
            this.listMain.AddItem(ui);
        }
    }

    /// <summary>
    /// 清楚好友列表
    /// </summary>
    private void ClearFriendList()
    {
        this.listMain.RemoveAll();
    }
}
