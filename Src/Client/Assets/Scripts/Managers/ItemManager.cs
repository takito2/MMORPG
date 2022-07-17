using Assets.Scripts.Models;
using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
//using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using Services;

namespace Managers
{
    public class ItemManager : Singleton<ItemManager>
    {
        public Dictionary<int, Item> Items = new Dictionary<int, Item>();
        internal void Init(List<NItemInfo> items)
        {
            this.Items.Clear();
            foreach (var info in items)//从网络数据填充到字典内
            {
                Item item = new Item(info);
                this.Items.Add(item.Id, item);

                Debug.LogFormat("ItemManager:Init[{0}]", item);
            }
            StatusService.Instance.RegisterStatusNotify(StatusType.Item,OnItemNotify);//注册Item状态相应事件
        }


        public ItemDefine GetItem(int itemId)
        {


            return null;
        }

        private bool OnItemNotify(NStatus status)
        {
            if (status.Action == StatusAction.Add)//根据Action分配函数处理
            {
                this.AddItem(status.Id, status.Value);
            }
            if (status.Action == StatusAction.Delete)
            {
                this.RemoveItem(status.Id,status.Value);
            }
            return true;
        }

        private void AddItem(int itemid, int count)
        {
            Item item = null;
            if (this.Items.TryGetValue(itemid , out item))//看原Items内是否包含，决定是否重新创建
            {
                item.Count += count;//直接+=
            }
            else
            {
                item = new Item(itemid, count);//重新创建并添加入itens中
                this.Items.Add(itemid,item);
            }
            BagManager.Instance.AddItem(itemid,count);
        }

        private void RemoveItem(int itemid, int count)
        {
            if (!this.Items.ContainsKey(itemid))//判断是否拥有
            {
                return;
            }
            Item item = this.Items[itemid];
            if (item.Count < count)//判断移除数目是否合理
                return;
            item.Count -= count;

            BagManager.Instance.RemoveItem(itemid, count);
        }


        public bool UseItem(int itemId)
        {

            return false;
        }

        public bool UseItem(ItemDefine item)
        {

            return false;
        }
    }
}
