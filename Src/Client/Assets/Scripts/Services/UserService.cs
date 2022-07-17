using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Network;
using System;
using UnityEngine;
using Models;
using Managers;

namespace Services
{
    public class UserService : Singleton<UserService>, IDisposable//泛型单例
    {
        public UnityEngine.Events.UnityAction<Result, string> OnRegister;
        public UnityEngine.Events.UnityAction<Result, string> OnLogin;
        public UnityEngine.Events.UnityAction<Result, string> OnCharacterCreate;

        NetMessage pendingMessage = null;//相当于数组存储未发送数据
        bool connected = false;

        public UserService()
        {
            NetClient.Instance.OnConnect += OnGameServerConnect;
            NetClient.Instance.OnDisconnect += OnGameServerDisconnect;
            MessageDistributer.Instance.Subscribe<UserRegisterResponse>(this.OnUserRegister);//订阅回发消息
            MessageDistributer.Instance.Subscribe<UserLoginResponse>(this.OnUserLogin);
            MessageDistributer.Instance.Subscribe<UserCreateCharacterResponse>(this.OnUserCreateCharacter);
            MessageDistributer.Instance.Subscribe<UserGameEnterResponse>(this.OnGameEnter);
            MessageDistributer.Instance.Subscribe<UserGameLeaveResponse>(this.OnGameLeave);
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnCharacterEnter);
        }

        

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<UserCreateCharacterResponse>(this.OnUserCreateCharacter);
            MessageDistributer.Instance.Unsubscribe<UserRegisterResponse>(this.OnUserRegister);
            MessageDistributer.Instance.Unsubscribe<UserLoginResponse>(this.OnUserLogin);
            MessageDistributer.Instance.Unsubscribe<UserGameEnterResponse>(this.OnGameEnter);
            MessageDistributer.Instance.Unsubscribe<UserGameLeaveResponse>(this.OnGameLeave);
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnCharacterEnter);
            NetClient.Instance.OnConnect -= OnGameServerConnect;
            NetClient.Instance.OnDisconnect -= OnGameServerDisconnect;
        }


        public void Init()
        {
            Log.InfoFormat("Client UserService Init");
        }

        public void ConnectToServer()
        {
            Debug.Log("ConnectToServer() Start");
            NetClient.Instance.Init("127.0.0.1", 8000);
            NetClient.Instance.Connect();
        }

        void OnGameServerConnect(int result, string reason)
        {
            //Log.InfoFormat("LoadingMessager::OnGameServerConnect :{0} reason{1}", result, reason);
            if (NetClient.Instance.Connected)
            {
                this.connected = true;
                if (this.pendingMessage != null)
                {
                    NetClient.Instance.SendMessage(this.pendingMessage);
                    this.pendingMessage = null;
                }
            }
            else
            {
                if (!this.DisconnectNotify(result, reason))
                {
                    MessageBox.Show(string.Format("网络错误，无法连接到服务器！\n Result:{0} Error:{1}", result, reason), "错误", MessageBoxType.Error);
                }
            }

        }

        public void OnGameServerDisconnect(int result, string reason)//断开服务器连接
        {
            this.DisconnectNotify(result, reason);
            return;
        }

        bool DisconnectNotify(int result, string reason)
        {
            if (this.pendingMessage != null)
            {
                if (this.pendingMessage.Request.userLogin != null)
                {
                    if (this.OnLogin != null)
                    {
                        this.OnLogin(Result.Failed, string.Format("服务器断开！\n Result:{0} Error:{1}", result, reason));
                    }
                }
                else if (this.pendingMessage.Request.userRegister != null)
                {
                    if (this.OnRegister != null)
                    {
                        this.OnRegister(Result.Failed, string.Format("服务器断开！\n Result:{0} Error:{1}", result, reason));
                    }
                }
                else
                {
                    if (this.OnCharacterCreate != null)
                    {
                        this.OnCharacterCreate(Result.Failed, string.Format("服务器断开！\n RESULT:{0} ERROR:{1}", result, reason));
                    }
                }
                return true;
            }
            return false;
        }

        public void SendRegister(string user, string psw)
        {
            Debug.LogFormat("UserRegisterRequest::user:{0} psw:{1}", user, psw);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.userRegister = new UserRegisterRequest();
            message.Request.userRegister.User = user;
            message.Request.userRegister.Passward = psw;

            if (this.connected && NetClient.Instance.Connected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }

        }

        public void SendLogin(string user, string psw)
        {
            Debug.LogFormat("UserLoginRequest::user:{0} psw:{1}", user, psw);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.userLogin = new UserLoginRequest();
            message.Request.userLogin.User = user;
            message.Request.userLogin.Passward = psw;

            if (this.connected && NetClient.Instance.Connected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }
        }

        void OnUserRegister(object sender, UserRegisterResponse response)//接受返回的信息,逻辑层回发至此,回馈Ui层
        {
            Debug.LogFormat("OnUserRegister:{0}[{1}]", response.Result, response.Errormsg);

            if (this.OnRegister != null)
            {
                this.OnRegister(response.Result, response.Errormsg);
            }
            MessageBox.Show(response.Errormsg);
        }

        void OnUserLogin(object sender, UserLoginResponse response)
        {
            Debug.LogFormat("OnUserLogin:{0} [{1}]", response.Result, response.Errormsg);

            if (response.Result == Result.Success)
            {//登录成功逻辑
                Models.User.Instance.SetupUserInfo(response.Userinfo);
            }
            if (this.OnLogin != null)
            {
                this.OnLogin(response.Result, response.Errormsg);
            }
        }
        /// <summary>
        /// 发送角色信息至客户端
        /// </summary>
        /// <param name="charName">角色名称</param>
        /// <param name="cls">角色职业</param>
        public void SendCharacterCreate(string charName, CharacterClass cls)
        {
            Debug.LogFormat("charName:{0} CharacterClass [{1}]", charName, cls);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.createChar = new UserCreateCharacterRequest();
            message.Request.createChar.Name = charName;
            message.Request.createChar.Class = cls;

            if (this.connected && NetClient.Instance.Connected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }
        }
        /// <summary>
        /// 角色创建
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        void OnUserCreateCharacter(object sender, UserCreateCharacterResponse response)
        {
            Debug.LogFormat("OnUserCreateCharacter:{0} [{1}]", response.Result, response.Errormsg);

            if (response.Result == Result.Success)
            {
                Models.User.Instance.Info.Player.Characters.Clear();
                Models.User.Instance.Info.Player.Characters.AddRange(response.Characters);
                Debug.Log("当前数目"+Models.User.Instance.Info.Player.Characters.Count + "response:" + response.Characters.Count);
            }
            if (this.OnCharacterCreate != null)
            {
                this.OnCharacterCreate(response.Result, response.Errormsg);

            }
        }

        public void SendGameEnter(int characterIdx )
        {
            Debug.LogFormat("UserGameEnterRequest::characterId :{0}",characterIdx);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.gameEnter = new UserGameEnterRequest();
            message.Request.gameEnter.characterIdx = characterIdx;
            NetClient.Instance.SendMessage(message);
        }

        void OnGameEnter(object sender, UserGameEnterResponse response)
        {
            Debug.LogFormat("OnGameEnter:{0},[{1}]", response.Result, response.Errormsg);

            if (response.Result == Result.Success)//进入游戏成功，接受回发的数据，初始化道具信息
            {
                if (response.Character != null)
                {
                    ItemManager.Instance.Init(response.Character.Items);
                    BagManager.Instance.Init(response.Character.Bag);
                    EquipManager.Instance.Init(response.Character.Equips);
                }
            }
        }

        public void SendGameLeave()
        {
            Debug.Log("UserGameLeaveRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.gameLeave = new UserGameLeaveRequest();
            NetClient.Instance.SendMessage(message);
        }

        void OnGameLeave(object sender, UserGameLeaveResponse response)
        {
            MapService.Instance.CurrentMapId = 0;
            User.Instance.CurrentCharacter = null;
            Debug.LogFormat("OnGameLeave:{0} [{1}]",response.Result, response.Errormsg);

        }

        private void OnCharacterEnter(object sender, MapCharacterEnterResponse response)
        {
            //Debug.LogFormat("OnMapCharacterEnter:{0} ", response.mapId);
            //NCharacterInfo info = response.Characters[0];//拿到当前角色信息
            //User.Instance.CurrentCharacter = info;
            //SceneManager.Instance.LoadScene(DataManager.Instance.Maps[response.mapId].Resource);//加载新场景
            
        }
    }
}


