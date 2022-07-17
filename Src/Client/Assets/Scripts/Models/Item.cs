using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Managers;

namespace Assets.Scripts.Models
{
    public class Item
    {
        public int Id;
        public int Count;
        public ItemDefine Define;
        public EquipDefine EquipInfo;
        public Item(NItemInfo item) : this(item.Id, item.Count)
        { }

        /// <summary>
        /// 主要区别，客户端参数为网络协议的Item，服务端参数为DB内的Item
        /// </summary>
        /// <param name="item"></param>
        public Item(int id,int count)
        {
            this.Id = id;
            this.Count = count;
            //this.Define = DataManager.Instance.Items[this.Id];
            DataManager.Instance.Items.TryGetValue(this.Id, out this.Define);
            DataManager.Instance.Equips.TryGetValue(this.Id, out this.EquipInfo);
        }

        public override string ToString()
        {
            return string.Format("Id:{0},Count:{1}",this.Id,this.Count);
        }
    }
}
