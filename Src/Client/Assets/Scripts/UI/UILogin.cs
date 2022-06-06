using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Services;
using SkillBridge.Message;


public class UILogin : MonoBehaviour {
    public InputField username;
    public InputField password;
    public Button buttonLogin;
    //public Button buttonRegister;

	// Use this for initialization
	void Start () {
        UserService.Instance.OnLogin = OnLogin;//监听登录成功所要调用的事件
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnClickLogin()
    {
        if (string.IsNullOrEmpty(this.username.text))
        {
            MessageBox.Show("请输入账号");
            return;
        }
        if (string.IsNullOrEmpty(this.password.text))
        {
            MessageBox.Show("请输入密码");
            return;
        }
        UserService.Instance.SendLogin(this.username.text,this.password.text);
    }

    void OnLogin(Result result,string message)
    {
        if (result == Result.Success)
        {
            //登录成功，进入角色选择
            SceneManager.Instance.LoadScene("CharSelect");
        }
        else
            MessageBox.Show(message,"错误",MessageBoxType.Error);
    }

   
}
