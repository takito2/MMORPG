using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Common;

namespace GameServer.Services
{
    class DBService : Singleton<DBService>
    {
        ExtremeWorldEntities entities;

        public ExtremeWorldEntities Entities
        {
            get { return this.entities; }
        }

        public void Init()
        {
            entities = new ExtremeWorldEntities();
        }

        /// <summary>
        /// 异步保存，调用立即返回无须等待处理结果，服务器慢慢处理
        /// </summary>
        public void Save( bool async = false)
        {
            if (async)
                entities.SaveChangesAsync();//异步
            else
            entities.SaveChanges();//同步保存
        }
    }
}
