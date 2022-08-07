using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using GameServer.Entities;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class EntityManager : Singleton<EntityManager>
    {
        private int idx = 0;
        public List<Entity> AllEntitites = new List<Entity>();
        public Dictionary<int, List<Entity>> MapEntities = new Dictionary<int, List<Entity>>();

        public void AddEntity(int mapID, Entity entity)
        {
            AllEntitites.Add(entity);
            //加入管理器生成唯一id
            entity.EntityData.Id = ++this.idx;
            Log.InfoFormat("管理器生成唯一id:{0},entity.entityId:{1}", entity.EntityData.Id,entity.entityId);
            List<Entity> entities = null;
            if (!MapEntities.TryGetValue(mapID,out entities))
            {
                entities = new List<Entity>();
                MapEntities[mapID] = entities;
            }
            entities.Add(entity);
        }

        public void RemoveEntity(int mapID, Entity entity)
        {
            this.AllEntitites.Remove(entity);
            this.MapEntities[mapID].Remove(entity);
        }
    }
}
