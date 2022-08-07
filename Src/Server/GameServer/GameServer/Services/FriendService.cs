using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using SkillBridge.Message;
using Network;
using GameServer.Models;
using GameServer.Entities;
using GameServer.Managers;

namespace GameServer.Services
{
    class FriendService : Singleton<FriendService>
    {
        //List<FriendAddRequest> friendRequest = new List<FriendAddRequest>();

        public FriendService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendAddRequest>(this.OnFriendAddRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendAddResponse>(this.OnFriendAddResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendRemoveRequest>(this.OnFriendRemove);
        }

        public void Init()
        {

        }

        /// <summary>
        /// 收到加好友请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnFriendAddRequest(NetConnection<NetSession> sender, FriendAddRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnFriendAddRequest::FromId：{0}，FromName:{1},ToID:{2},ToName:{3}", request.FromId, request.FromName, request.ToId, request.ToName);

            if (request.ToId == 0)
            {//没有Id则用名字查找
                foreach (var cha in CharacterManager.Instance.Characters)
                {
                    if (cha.Value.Data.Name == request.ToName)
                    {
                        request.ToId = cha.Key;//重新为ToId赋值
                        break;
                    }
                }
            }
                NetConnection<NetSession> friend = null;
                if (request.ToId > 0)
                {
                    if(character.FriendManager.GetFriendInfo(request.ToId) != null)
                    {
                        sender.Session.Response.friendAddRes = new FriendAddResponse();
                        sender.Session.Response.friendAddRes.Result = Result.Success;
                        sender.Session.Response.friendAddRes.Errormsg = "已经是好友了";
                        sender.SendResponse();
                        return;
                    }
                    friend = SessionManager.Instance.GetSession(request.ToId);//查看是否在线
                }
                if (friend == null)
                {
                    sender.Session.Response.friendAddRes = new FriendAddResponse();
                    sender.Session.Response.friendAddRes.Result = Result.Failed;
                    sender.Session.Response.friendAddRes.Errormsg = "好友不存在或不在线";
                    sender.SendResponse();
                    return;
                }
                Log.InfoFormat("ForwardRequest:FromId:{0} FromName:{1},ToId:{2},ToName:{3}",request.FromId,request.FromName,request.ToId,request.ToName);
                friend.Session.Response.friendAddReq = request;//在线情况，将添加好友请求发送给目标玩家客户端
                friend.SendResponse();

            
        }

        /// <summary>
        /// 收到加好友响应//被加好友者客户端处理同意||拒绝后将结果发送至此
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnFriendAddResponse(NetConnection<NetSession> sender, FriendAddResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnFriendAddResponse::character:{0},Result:{1},FromId:{2},ToId:{3}",character.Id,response.Result,response.Request.FromId,response.Request.ToId);
            sender.Session.Response.friendAddRes = response;//
            if (response.Result == Result.Success)
            {
                var requester = SessionManager.Instance.GetSession(response.Request.FromId);//添加者
                if (requester == null)//添加者已下线
                {
                    sender.Session.Response.friendAddRes.Result = Result.Failed;
                    sender.Session.Response.friendAddRes.Errormsg = "请求者已下线";
                }
                else
                {
                    character.FriendManager.AddFriend(requester.Session.Character);//被加者添加好友
                    requester.Session.Character.FriendManager.AddFriend(character);//添加者添加好友
                    DBService.Instance.Save();
                    requester.Session.Response.friendAddRes = response;
                    requester.Session.Response.friendAddRes.Result = Result.Success;
                    requester.Session.Response.friendAddRes.Errormsg = "添加好友成功";
                    requester.SendResponse();//回发给添加者客户端
                }
            }
            sender.SendResponse();//拒绝添加好友，直接回发给被添加者。继承原先的Result及err

        }

        private void OnFriendRemove(NetConnection<NetSession> sender, FriendRemoveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnFriendRemove::character:{0} FriendReletion:{1}",character.Id,request.Id);
            sender.Session.Response.friendRemove = new FriendRemoveResponse();
            sender.Session.Response.friendRemove.Id = request.Id;

            //删除自己的好友
            if (character.FriendManager.RemoveFriendByID(request.Id))//删除成功
            {
                sender.Session.Response.friendRemove.Result = Result.Success;
                //删除别人好友中的自己
                var friend = SessionManager.Instance.GetSession(request.friendId);//获取被删除者session
                if (friend != null)//在线
                {
                    friend.Session.Character.FriendManager.RemoveFriendByFriendId(character.Id);//将好友列表中的自己删除
                }
                else//离线
                {
                    this.RemoveFriend(request.friendId, character.Id);
                }
            }
            else
                sender.Session.Response.friendRemove.Result = Result.Failed;//删除失败

            DBService.Instance.Save();
            sender.SendResponse();
        }

        void RemoveFriend(int charId,int friendId)
        {
            var removeItem = DBService.Instance.Entities.CharacterFriends.FirstOrDefault(v => v.TCharacterID == charId && v.FriendID == friendId);
            if (removeItem != null)
            {
                DBService.Instance.Entities.CharacterFriends.Remove(removeItem);//数据库记录移除
            }
        }
     
    }
}
