using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Network;
using SkillBridge.Message;
using Common;

namespace GameServer.Services
{
    class HelloWorldService : Singleton<HelloWorldService>//单例
    {
        public void Init()//初始化方法
        {
           
        }

        public void Start()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FirstTestRequest>(this.OnFirstTestRequest);
            //作用是告诉网路底层使用OnFirstTestRequest方法处理FirstTestRequest协议/消息
        }

        void  OnFirstTestRequest(NetConnection<NetSession> sender, FirstTestRequest firstTestRequest)
        {
            Log.InfoFormat("OnFirstTestRequest message : {0}", firstTestRequest.Helloworld );//日志输出
        }

        public void Stop()
        {

        }
    }
}
