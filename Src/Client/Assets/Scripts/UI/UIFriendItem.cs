using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Models;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Managers;
using SkillBridge.Message;

public class UIFriendItem : ListView.ListViewItem {

    public Text nickName;
    public Text @class;
    public Text level;
    public Text Status;

    public Image background;
    public Sprite normalBg;
    public Sprite selectedBg;

    public override void onSelected(bool selected)
    {
        this.background.overrideSprite = selected ? selectedBg : normalBg;
    }
    public NFriendInfo Info;
    // Use this for initialization
    void Start () {
	}
	
	public void  SetFriendInfo(NFriendInfo item)
    {
        this.Info = item;
        if (this.nickName != null) this.nickName.text = this.Info.friendInfo.Name;
        if (this.@class != null) this.@class.text = this.Info.friendInfo.Class.ToString();
        if (this.level != null) this.level.text = this.Info.friendInfo.Level.ToString();
        if (this.Status != null) this.Status.text = this.Info.Status == 1 ? "在线" : "离线";
    }
}
