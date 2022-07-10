using Common;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    /// <summary>
    /// 提供Item相关对外接口
    /// </summary>
    class ItemManager
    {
        Character Owner;

        public Dictionary<int, Item> Items = new Dictionary<int, Item>();

        public ItemManager(Character owner)
        {
            this.Owner = owner;

            foreach (var item in owner.Data.Items)
            {
                this.Items.Add(item.ItemID, new Item(item));
            }
        }

        public bool UseItem(int itemId,int count = 1)//使用函数
        {
            Log.InfoFormat("[{0}]UseItem[{1}:{2}]",this.Owner.Data.ID,itemId,count);
            Item item = null;
            if (this.Items.TryGetValue(itemId,out item))
            {
                if (item.Count < count)//持有数
                    return false;

                //TODO:增加使用逻辑
                item.Remove(count);

                return true;

            }
            return false;
        }

        public bool HasItem(int itemId)//判断是否拥有
        {
            Item item = null;
            if (this.Items.TryGetValue(itemId, out item))
                return item.Count > 0;
            return false;
        }

        public Item GetItem(int itemId)
        {
            Item item = null;
            this.Items.TryGetValue(itemId, out item);
            Log.InfoFormat("[{0}]GetItrm[{1}:{2}]",this.Owner.Data.ID,itemId,item);
            return item;
        }

        public bool AddItem(int itemId,int count)//增加Item
        {
            Item item = null;

            if (this.Items.TryGetValue(itemId,out item))//原本已存在，对数目进行增加
            {
                item.Add(count);
            }
            else//原本不存在，重新创建
            {
                TCharacterItem dbItem = new TCharacterItem();
                dbItem.TCharacterID = Owner.Data.ID;
                dbItem.Owner = Owner.Data;
                dbItem.ItemID = itemId;
                dbItem.ItemCount = count;
                Owner.Data.Items.Add(dbItem);//更新现有数据
                item = new Item(dbItem);
                this.Items.Add(itemId, item);
            }
            Log.InfoFormat("[{0}]AddItem[{1}] addCount:{2}",this.Owner.Data.ID,item,count);
            //DBService.Instance.Save();
            return true;
        }

        public bool RemoveItem(int ItemId,int count)
        {
            if (!this.Items.ContainsKey(ItemId))
            {
                return false;
            }
            Item item = this.Items[ItemId];
            if (item.Count < count)
                return false;
            item.Remove(count);
            Log.InfoFormat("[{0}]RemoveItem[{1}] removeCount{2}",this.Owner.Data.ID,item,count);
            //DBService.Instance.Save();
            return true;
        }

        public void GetItemInfos(List<NItemInfo> list)//将内存数据转变为网络数据，参数为Info.Items
        {
            foreach (var item in this.Items)
            {
                list.Add(new NItemInfo() { Id = item.Value.ItemID, Count = item.Value.Count });
            }
        }
    }
}
