using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Entities;
using SkillBridge.Message;
using Network;
using GameServer.Services;

namespace GameServer.Managers
{
    class FriendManager//基本都通过Character调用，所以无需单例
    {
        private Character Owner;

        List<NFriendInfo> friends = new List<NFriendInfo>();

        bool friendChanged = false;

        internal void GetFriendInfos(List<NFriendInfo> list)
        {
            foreach (var f in this.friends)
            {
                list.Add(f);
            }
        }

        public void InitFriends()
        {
            this.friends.Clear();
            foreach (var friend in this.Owner.Data.Friends)
            {
                this.friends.Add(GetFriendInfo(friend));//数据库信息转网络信息
            }
        }

        public FriendManager(Character owner)
        {
            this.Owner = owner;
        }

        internal void AddFriend(Character friend)
        {
            TCharacterFriend tf = new TCharacterFriend()
            {
                FriendID = friend.Id,
                FriendName = friend.Data.Name,
                Class = friend.Data.Class,
                Level = friend.Data.Level
            };
            this.Owner.Data.Friends.Add(tf);
            friendChanged = true;

        }

        internal bool RemoveFriendByFriendId(int friendId)
        {
            var removeItem = this.Owner.Data.Friends.FirstOrDefault(v => v.FriendID == friendId);
            if (removeItem != null)
            {
                DBService.Instance.Entities.CharacterFriends.Remove(removeItem);
            }
            friendChanged = true;
            return true;
        }

        internal bool RemoveFriendByID(int id)
        {
            var removeItem = this.Owner.Data.Friends.FirstOrDefault(v => v.Id == id);
            if (removeItem != null)
            {
                DBService.Instance.Entities.CharacterFriends.Remove(removeItem);
            }
            friendChanged = true;
            return true;
        }
      

        internal void UpdateFriendInfo(NCharacterInfo friendInfo, int status)
        {
            foreach (var f in this.friends)
            {
                if (f.friendInfo.Id == friendInfo.Id)//好友的好友信息中Id == 下线玩家Id
                {
                    f.Status = status;
                    break;
                }
            }
            this.friendChanged = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="friend">一条数据库信息</param>
        /// <returns></returns>
        internal NFriendInfo GetFriendInfo(TCharacterFriend friend)
        {
            NFriendInfo friendInfo = new NFriendInfo();//网络信息
            var character = CharacterManager.Instance.GetCharacter(friend.FriendID);
            friendInfo.friendInfo = new NCharacterInfo();
            friendInfo.Id = friend.Id;
            if (character == null)//离线
            {
                friendInfo.friendInfo.Id = friend.FriendID;
                friendInfo.friendInfo.Name = friend.FriendName;
                friendInfo.friendInfo.Class = (CharacterClass)friend.Class;
                friendInfo.friendInfo.Level = friend.Level;
                friendInfo.Status = 0;
            }
            else
            {
                friendInfo.friendInfo = GetBasicInfo(character.Info);
                friendInfo.friendInfo.Name = character.Info.Name;
                friendInfo.friendInfo.Class = character.Info.Class;
                friendInfo.friendInfo.Level = character.Info.Level;
                character.FriendManager.UpdateFriendInfo(this.Owner.Info, 1);
                friendInfo.Status = 1;
            }
            return friendInfo;
        }

        private NCharacterInfo GetBasicInfo(NCharacterInfo info)
        {
            return new NCharacterInfo()
            {
                Id = info.Id,
                Name = info.Name,
                Class = info.Class,
                Level = info.Level
            };

        }

        public NFriendInfo GetFriendInfo(int friendId)
        {
            foreach (var f in this.friends)
            {
                if (f.friendInfo.Id == friendId)
                {
                    return f;
                }
            }
            return null;
        }

        public void PostProcess(NetMessageResponse message)
        {
            if (friendChanged)
            {
                this.InitFriends();
                if (message.friendList == null)
                {
                    message.friendList = new FriendListResponse();
                    message.friendList.Friends.AddRange(this.friends);//修改过的friend列表重新赋值到响应中
                }
                friendChanged = false;
            }
        }

        
    }
}
