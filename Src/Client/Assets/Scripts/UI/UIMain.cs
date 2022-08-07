using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMain : MonoSingleton<UIMain> {
    public Text avaterName;
    public Text avatetLevel;

	// Use this for initialization
	protected override void OnStart () {
        this.UpdateAvater();
	}
	
    void UpdateAvater()
    {
        this.avaterName.text = string.Format("{0}[{1}]", User.Instance.CurrentCharacter.Name,User.Instance.CurrentCharacter.Id); 
        this.avatetLevel.text = User.Instance.CurrentCharacter.Level.ToString();
    }

	// Update is called once per frame
	void Update () {
		
	}

    public void BackToCharSelect()
    {
        SceneManager.Instance.LoadScene("CharSelect");
        Services.UserService.Instance.SendGameLeave();
    }

    public void OnClickTest()
    {
        UIManager.Instance.Show<UITest>();
    }

    public void OnClickBag()
    {
        UIManager.Instance.Show<UIBag>();
    }

    public void OnClickEquip()
    {
        UIManager.Instance.Show<UICharEquip>();
    }

    public void OnClickQuest()
    {
        UIManager.Instance.Show<UIQuestSystem>();
    }

    public void OnClickFriend()
    {
        UIManager.Instance.Show<UIFriends>();
    }
}
