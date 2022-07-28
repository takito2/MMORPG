using Entities;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElementManager : MonoSingleton<UIWorldElementManager> {

    public GameObject nameBarPrefab;
    public GameObject npcStatusPrefab;

    private Dictionary<Transform, GameObject> elementsNames = new Dictionary<Transform, GameObject>();
    private Dictionary<Transform, GameObject> elementsStatus = new Dictionary<Transform, GameObject>();

    public Camera Camera;

	// Use this for initialization

    protected override void OnStart()
    {
        base.OnStart();
    }

    // Update is called once per frame
    void Update () {
		
	}


    public void AddCharacterNameBar(Transform owner, Character character)
    {
        GameObject goNameBar = Instantiate(nameBarPrefab , this.transform);
        goNameBar.name = "NameBar" + character.entityId;
        goNameBar.GetComponent<UIWorldElement>().owner = owner;
        goNameBar.GetComponent<UINameBar>().character = character;
        this.elementsNames[owner] = goNameBar;
        goNameBar.SetActive(true);
    }

    public void RemoveCharacterNameBar(Transform owner)
    {
        if (this.elementsNames.ContainsKey(owner))
        {
            Destroy(this.elementsNames[owner]);//从游戏中删除
            this.elementsNames.Remove(owner);//从字典移除
        }
    }

    public void AddNpcQuestStatus(Transform owner, NpcQuestStatus status)
    {
        if(this.elementsStatus.ContainsKey(owner))
        {
            elementsStatus[owner].GetComponent<UIQuestStatus>().SetQuestStatus(status);
        }
        else
        {
            GameObject go = Instantiate(npcStatusPrefab, this.transform);
            go.name = "NpcQuestStatus" + owner.name;
            go.GetComponent<UIWorldElement>().owner = owner;
            go.GetComponent<UIQuestStatus>().SetQuestStatus(status);
            this.elementsStatus[owner] = go;
            go.SetActive(true);
        }
        
    }

    public void RemoveNpcQuestStatus(Transform owner)
    {
        if (this.elementsStatus.ContainsKey(owner))
        {
            Destroy(this.elementsStatus[owner]);//从游戏中删除
            this.elementsStatus.Remove(owner);//从字典移除
        }
    }
}
