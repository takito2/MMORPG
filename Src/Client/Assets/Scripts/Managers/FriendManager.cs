using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillBridge.Message;

namespace Managers
{
    public class FriendManager : Singleton<FriendManager>
    {
        //public Dictionary<int, NFriendInfo> allFriends = new Dictionary<int, NFriendInfo>();
        public List<NFriendInfo> allFriends;

        public void Init(List<NFriendInfo> friends)
        {
            this.allFriends = friends;
        }


    }
}

