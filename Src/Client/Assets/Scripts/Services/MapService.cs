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
            MessageDistributer.Instance.Subscribe<MapEntitySyncResponse>(this.OnMapEntitySync);
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
            Debug.LogFormat("OnMapCharacterEnter:Map:{0},Count:{1},CurrentMapId:{2}", response.mapId, response.Characters.Count, CurrentMapId);
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
            Debug.LogFormat("OnMapCharacterLeave: CharID:{0},CurrentCharacter:{1}", response.characterId, User.Instance.CurrentCharacter.Id);
            if (response.characterId != User.Instance.CurrentCharacter.Id)
            {
                Debug.LogFormat("别人离开：response.characterId：{0}，User.Instance.CurrentCharacter.Id：{1}",response.characterId, User.Instance.CurrentCharacter.Id);
                CharacterManager.Instance.RemoveCharacter(response.characterId);
            }               
            else
            {
                Debug.LogFormat("自己离开：response.characterId：{0}，User.Instance.CurrentCharacter.Id：{1}", response.characterId, User.Instance.CurrentCharacter.Id);
                CharacterManager.Instance.Clear();
            }
                
            
        }

        private void EnterMap(int mapId)
        {
            if (DataManager.Instance.Maps.ContainsKey(mapId))
            {
                MapDefine map = DataManager.Instance.Maps[mapId];
                User.Instance.CurrentMapData = map;
                SceneManager.Instance.LoadScene(map.Resource);               

            }
            else
                Debug.LogErrorFormat("EnterMap:Map:{0} not existed",mapId);
        }

        public void SendMapEntitySync(EntityEvent entityEvent, NEntity entity)
        {
            Debug.LogFormat("MapEntityUpdateRequest :ID:{0} POS:{1} DIR:{2} SPD:{3}",entity.Id,entity.Position.String(),entity.Direction.String(),entity.Speed);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapEntitySync = new MapEntitySyncRequest();
            message.Request.mapEntitySync.entitySync = new NEntitySync()
            {
                Id = entity.Id,
                Event = entityEvent,
                Entity = entity             
            };
            NetClient.Instance.SendMessage(message);

        }

        private void OnMapEntitySync(object sender, MapEntitySyncResponse message)
        {
            throw new NotImplementedException();
        }
    }
}
