using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainCity : MonoBehaviour {
    public Text avaterName;
    public Text avatetLevel;

	// Use this for initialization
	void Start () {
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
}
