using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Models;
using Services;
using SkillBridge.Message;

namespace Managers
{
    class EquipManager : Singleton<EquipManager>
    {
        public delegate void OnEquipChangeHandler();

        public event OnEquipChangeHandler OnEquipChanged;

        public Item[] Equips = new Item[(int)EquipSlot.SlotMax];//长度为7

        byte[] Data;//int list

        unsafe public void Init(byte[] data)
        {
            this.Data = data;
            this.ParseEquipData(data);
        }

        public bool Contains(int equipId)//判断是否装备着某件装备
        {
            for (int i = 0; i < Equips.Length; i++)
            {
                if (Equips[i] != null && Equips[i].Id == equipId)
                    return true;
            }
            return false;
        }

        public Item GetEquip(EquipSlot slot)//返回格子道具信息
        {
            return Equips[(int)slot];
        }

        unsafe void ParseEquipData(byte[] data)//解析data填入Equips，data中为各槽位Itemid
        {
            fixed(byte* pt = this.Data)
            {
                for (int i = 0; i < Equips.Length; i++)
                {
                    int itemId = *(int*)(pt + i * sizeof(int));
                    if (itemId > 0)
                        Equips[i] = ItemManager.Instance.Items[itemId];
                    else
                        Equips[i] = null;

                }
                
            }
        }

        unsafe public byte[] GetEquipData()//将Slot槽位的Id转成byte数组
        {
            fixed(byte* pt = Data)
            {
                for (int i = 0; i < (int)EquipSlot.SlotMax; i++)
                {
                    int* itemid = (int*)(pt + i * sizeof(int));
                    if (Equips[i] == null)
                        *itemid = 0;
                    else
                        *itemid = Equips[i].Id;
                }
            }
            return this.Data;
        }


        public void EquipItem(Item equip)//装备道具
        {
            ItemService.Instance.SendEquipItem(equip, true);
        }

        public void UnEquipItem(Item equip)//脱下道具
        {
            ItemService.Instance.SendEquipItem(equip, false);
        }

        public void OnEquipItem(Item equip)
        {
            if (this.Equips[(int)equip.EquipInfo.Slot] != null && this.Equips[(int)equip.EquipInfo.Slot].Id == equip.Id)
            {
                return;//穿相同装备
            }
            this.Equips[(int)equip.EquipInfo.Slot] = ItemManager.Instance.Items[equip.Id];//为该槽位赋予对应Item信息

            if (OnEquipChanged != null)
                OnEquipChanged();
        }

        public void OnUnEquipItem(EquipSlot slot)
        {
            if (this.Equips[(int)slot] != null)//该槽位不为空
            {
                this.Equips[(int)slot] = null;//置空
                if (OnEquipChanged != null)
                    OnEquipChanged();
            }
        }
    }
}
