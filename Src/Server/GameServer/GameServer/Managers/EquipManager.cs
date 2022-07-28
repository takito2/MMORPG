using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Entities;
using Network;
using SkillBridge.Message;
using GameServer.Managers;
using GameServer.Services;

namespace GameServer.Managers
{
    class EquipManager : Singleton<EquipManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="slot">槽位</param>
        /// <param name="itemId">道具Id</param>
        /// <param name="isEquip">穿or脱</param>
        /// <returns></returns>
        public Result EquipItem(NetConnection<NetSession> sender, int slot, int itemId, bool isEquip)
        {
            Character character = sender.Session.Character;
            if (!character.ItemManager.Items.ContainsKey(itemId))//判断是否拥有此件装备
                return Result.Failed;

            UpdateEquip(character.Data.Equips, slot, itemId, isEquip);

            DBService.Instance.Save();
            return Result.Success;  

        }

        unsafe void UpdateEquip(byte[] equipData,int slot,int itemId,bool isEquip)
        {
            fixed(byte* pt = equipData)//获得数组首地址
            {
                int* slotid = (int*)(pt + slot * sizeof(int));//计算slot槽位地址
                if (isEquip)
                    *slotid = itemId;//穿,更改id
                else
                    *slotid = 0;//脱
            }
        }
    }
}
