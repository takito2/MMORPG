﻿using System;
using System.Collections;
using System.Collections.Generic;
using Models;
using UnityEngine;

public class UIQuestDialog : UIWindow {
    public UIQuestInfo questInfo;

    public Quest quest;

    public GameObject openButtons;
    public GameObject submitButtons;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetQuest(Quest quest)
    {
        this.quest = quest;
        this.UpdateQuest();
        if (this.quest.Info == null)
        {
            openButtons.SetActive(true);
            submitButtons.SetActive(false);
        }
        else
        {
            if (this.quest.Info.Status == SkillBridge.Message.QuestStatus.Complated)
            {
                openButtons.SetActive(false);
                submitButtons.SetActive(true);
            }
            else
            {
                openButtons.SetActive(false);
                submitButtons.SetActive(false);
            }
        }
    }

    private void UpdateQuest()
    {
        if (this.quest != null)
        {
            this.questInfo.SetQuestInfo(quest);
        }
    }
}
