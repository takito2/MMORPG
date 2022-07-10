using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    class Item//封装作为内存的Item副本，单纯客户端与服务端通信时可用,减少对数据库的访问
    {
        TCharacterItem dbItem;//数据库对象

        public int ItemID;

        public int Count;

        public Item(TCharacterItem item)//构造函数初始化
        {
            this.dbItem = item;
            this.ItemID = (short)item.ItemID;
            this.Count = (short)item.ItemCount;
        }

        public void Add(int count)
        {
            this.Count += count;
            dbItem.ItemCount = this.Count;
        }

        public void Remove(int count)
        {
            this.Count -= count;
            dbItem.ItemCount = this.Count;
        }

        public bool Use(int count = 1)
        {
            return false;
        }

        public override string ToString()
        {
            return string.Format("ID:{0},Count:{1}",this.ItemID,this.Count);
        }
    }
}
