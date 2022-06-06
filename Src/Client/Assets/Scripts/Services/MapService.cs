using Common;
using Common.Data;
using Manager;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Services
{
    class MapService : Singleton<MapService>, IDisposable
    {
        public int CurrentMapId = 0;

        public MapService()
        {
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);//订阅回发消息
            MessageDistributer.Instance.Subscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
        }

        //public int CurrentMapId { get; private set; }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Unsubscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
        }


        public void Init()
        {
            Log.InfoFormat("Client UserService Init");
        }


        private void OnMapCharacterEnter(object sender, MapCharacterEnterResponse response)
        {
            Debug.LogFormat("OnMapCharacterEnter:Map:{0},Count:{1}", response.mapId, response.Characters.Count);
            foreach (var cha in response.Characters)
            {
                if (User.Instance.CurrentCharacter.Id == cha.Id)
                {//当前角色切换地图
                    User.Instance.CurrentCharacter = cha;//刷新本地数据
                }
                CharacterManager.Instance.AddCharacter(cha);

            }
            if (CurrentMapId != response.mapId)
            {
                this.EnterMap(response.mapId);
                this.CurrentMapId = response.mapId;
            }
        }
       

        private void OnMapCharacterLeave(object sender, MapCharacterLeaveResponse response)
        {
            Debug.LogFormat("OnMapCharacterLeave: CharID:{0}",response.characterId);
            if (response.characterId != User.Instance.CurrentCharacter.Id)
            {
                Debug.Log("其他角色离开");
                CharacterManager.Instance.RemoveCharacter(response.characterId);
            }               
            else
            {
                Debug.Log("自己离开");
                CharacterManager.Instance.Clear();
            }
                
            
        }

        private void EnterMap(int mapId)
        {
            if (DataManager.Instance.Maps.ContainsKey(mapId))
            {
                MapDefine map = DataManager.Instance.Maps[mapId];
                Debug.Log("赋值当前地图对象");
                User.Instance.CurrentMapData = map;
                SceneManager.Instance.LoadScene(map.Resource);               

            }
            else
                Debug.LogErrorFormat("EnterMap:Map:{0} not existed",mapId);
        }
    }
}
