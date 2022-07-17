using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Login : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Network.NetClient.Instance.Init("127.0.0.1",8000);//初始化服务端ip及端口
        Network.NetClient.Instance.Connect();//链接

        SkillBridge.Message.NetMessage msg = new SkillBridge.Message.NetMessage();//创建发送对象

        //SkillBridge.Message.FirstTestRequest firstTestRequest = new SkillBridge.Message.FirstTestRequest();
        //firstTestRequest.Helloworld = "Hello World";
        //msg.Request.firstRequest = firstTestRequest;//消息的封装

        //另一种做法，直接new对象
        msg.Request = new SkillBridge.Message.NetMessageRequest();//实例化，不然会报空引用

        Network.NetClient.Instance.SendMessage(msg);//只能发送NetMessage类型消息


    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
