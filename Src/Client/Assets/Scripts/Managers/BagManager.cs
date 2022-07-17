using Assets.Scripts.Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Managers
{
    class BagManager : Singleton<BagManager>
    {
        public int Unlocked;

        public BagItem[] Items;//BagItem结构体数组，负责存储从二进制数据转化来的信息数据

        NBagInfo Info;//背包网络数据

        unsafe public void Init(NBagInfo info)
        {
            this.Info = info;
            this.Unlocked = info.Unlocked;
            Items = new BagItem[this.Unlocked];
            if (info.Items != null && info.Items.Length >= this.Unlocked)
            {
                Analyza(info.Items);
            }
            else
            {
                Info.Items = new byte[sizeof(BagItem) * this.Unlocked];//初始化字节数组长度：结构体长度 * 已解锁格子数
                Reset();
            }
        }

        

        unsafe void Analyza(byte[] data)//字节数组解析成结构体数组
        {
            fixed(byte* pt = data)
            {
                for (int i = 0; i < this.Unlocked; i++)
                {
                    BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));
                    Items[i] = *item;//结构体值类型。用=号地址不改变值改变
                }
            }
        }

        public void Reset()//整理函数，检验最大叠加数进行分格，将分格结果存入Items数组中
        {
            int i = 0;
            foreach (var kv in ItemManager.Instance.Items)
            {
                if (kv.Value.Count <= kv.Value.Define.StackLimit)
                {
                    this.Items[i].ItemId = (ushort)kv.Key;
                    this.Items[i].Count = (ushort)kv.Value.Count;
                }
                else
                {
                    int count = kv.Value.Count;
                    while (count > kv.Value.Define.StackLimit)
                    {
                        this.Items[i].ItemId = (ushort)kv.Key;
                        this.Items[i].Count = (ushort)kv.Value.Define.StackLimit;
                        i++;
                        count -= kv.Value.Define.StackLimit;
                    }
                    this.Items[i].ItemId = (ushort)kv.Key;
                    this.Items[i].Count = (ushort)count;
                }
                i++;
            }
        }

        internal void RemoveItem(int itemid, int count)
        {
            throw new NotImplementedException();
        }

        public void AddItem(int itemid, int count)
        {
            ushort addCount = (ushort)count;
            for (int i = 0; i < Items.Length; i++)//循环已有背包Item列表，查看同id道具格子是否可放下
            {
                if (this.Items[i].ItemId == itemid)//同id，即同道具
                {
                    ushort canAdd = (ushort)(DataManager.Instance.Items[itemid].StackLimit - this.Items[i].Count);//当前格子距离最大堆叠数的差距，即可追加个数
                    if (canAdd >= addCount)//放得下
                    {
                        this.Items[i].Count += addCount;
                        addCount = 0;
                        break;
                    }
                    else//放不下，将当前格子填满，待放入个数更新
                    {
                        this.Items[i].Count += canAdd;
                        addCount -= canAdd;
                    }
                }
            }
            if(addCount > 0)//同id道具格无法放下，需新建情况
            {
                for (int i = 0; i < Items.Length; i++)
                {
                    if (this.Items[i].ItemId == 0 && addCount >0)//空格子
                    {
                        this.Items[i].ItemId = (ushort)itemid;
                        this.Items[i].Count = addCount;
                        addCount = 0;
                    }
                }
            }
        }

        unsafe public NBagInfo GetBagInfo()//结构体数组转字节数组
        {
            fixed(byte* pt = Info.Items)
            {
                for (int i = 0; i < this.Unlocked; i++)
                {
                    BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));
                    *item = Items[i];
                }
            }
            return this.Info;
        }
    }
}
