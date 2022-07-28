using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SkillBridge.Message;
using Common.Data;
using UnityEngine.EventSystems;
using Models;
using System;

public class UIQuestItem : ListView.ListViewItem {

    public Text title;

    public Image background;
    public Sprite normalBg;
    public Sprite selectedBg;

    public override void onSelected(bool selected)
    {
        this.background.overrideSprite = selected ? selectedBg : normalBg;
    }
    // Use this for initialization
    void Start () {
		
	}

    public Quest quest;
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetQuestInfo(Quest item)
    {
        this.quest = item;
        if (this.title != null)
            this.title.text = this.quest.Define.Name;
    }
}
