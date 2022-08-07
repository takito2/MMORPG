using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SkillBridge.Message;
using System.Linq;
using System.Text;
using Managers;
using Models;
using Network;


namespace Services
{
    public class FriendService : Singleton<FriendService>,IDisposable
    {
        public UnityAction OnFriendUpdate;

        public void Init()
        {

        }

        public FriendService()
        {
            MessageDistributer.Instance.Subscribe<FriendAddRequest>(this.OnFriendAddRepuest);
            MessageDistributer.Instance.Subscribe<FriendAddResponse>(this.OnFriendAddResponse);
            MessageDistributer.Instance.Subscribe<FriendListResponse>(this.OnFriendList);
            MessageDistributer.Instance.Subscribe<FriendRemoveResponse>(this.OnFriendRemove);

        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<FriendAddRequest>(this.OnFriendAddRepuest);
            MessageDistributer.Instance.Unsubscribe<FriendAddResponse>(this.OnFriendAddResponse);
            MessageDistributer.Instance.Unsubscribe<FriendListResponse>(this.OnFriendList);
            MessageDistributer.Instance.Unsubscribe<FriendRemoveResponse>(this.OnFriendRemove);
        }

        internal void SendFriendAddRequest(int friendId, string frinendName)
        {
            Debug.Log("SendFriendAdd");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.friendAddReq = new FriendAddRequest();
            message.Request.friendAddReq.FromId = User.Instance.CurrentCharacter.Id;
            message.Request.friendAddReq.FromName = User.Instance.CurrentCharacter.Name;
            message.Request.friendAddReq.ToId = friendId;
            message.Request.friendAddReq.ToName = frinendName;
            NetClient.Instance.SendMessage(message);
        }

        public void  SendFriendAddResponse(bool accept,FriendAddRequest request)
        {
            Debug.Log("SendFriendAdd");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.friendAddRes = new FriendAddResponse();
            message.Request.friendAddRes.Result = accept ? Result.Success : Result.Failed;
            message.Request.friendAddRes.Errormsg = accept ? "对方同意" : "对方拒绝了你的请求";
            message.Request.friendAddRes.Request = request;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 收到好友请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnFriendAddRepuest(object sender, FriendAddRequest request)
        {
            var confirm = MessageBox.Show(string.Format("{0} 请求添加你为好友", request.FromName), "好友请求", MessageBoxType.Confirm, "接受", "拒绝");
            confirm.OnYes = () =>
            {
                this.SendFriendAddResponse(true, request);//根据操作回发响应
            };
            confirm.OnNo = () => 
            {
                this.SendFriendAddResponse(false, request);
            };
        }

        /// <summary>
        /// 收到添加好友响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnFriendAddResponse(object sender, FriendAddResponse message)
        {
            if (message.Result == Result.Success)
            {
                MessageBox.Show(message.Request.ToName + "接受了你的请求", "添加成功");
            }
            else
            {
                MessageBox.Show(message.Errormsg, "添加好友失败");
            }
        }

        private void OnFriendList(object sender, FriendListResponse message)
        {
            Debug.Log("OnFriendList");
            FriendManager.Instance.allFriends = message.Friends;
            if (this.OnFriendUpdate != null)//在UiFriend添加了UIReFresh方法
                this.OnFriendUpdate();
        }

        internal void SendFriendRemoveRequest(int id, int friendId)
        {
            Debug.Log("SendfriendRemove");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.friendRemove = new FriendRemoveRequest();
            message.Request.friendRemove.Id = id;//自己Id
            message.Request.friendRemove.friendId = friendId;
            NetClient.Instance.SendMessage(message);
        }

        private void OnFriendRemove(object sender, FriendRemoveResponse message)
        {
            if (message.Result == Result.Success)
                MessageBox.Show("删除成功", "删除好友");
            else
                MessageBox.Show("删除失败","删除好友",MessageBoxType.Error);
        }

        

        

        




        
    }

}
