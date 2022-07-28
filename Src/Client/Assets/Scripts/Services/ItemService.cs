using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkillBridge.Message;
using Network;
using UnityEngine;
using Assets.Scripts.Models;
using Managers;


namespace Services
{
    class ItemService : Singleton<ItemService>,IDisposable
    {
        public ItemService()
        {
            MessageDistributer.Instance.Subscribe<ItemBuyResponse>(this.OnItemBuy);//订阅
            MessageDistributer.Instance.Subscribe<ItemEquipResponse>(this.OnItemEquip);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ItemBuyResponse>(this.OnItemBuy);
            MessageDistributer.Instance.Unsubscribe<ItemEquipResponse>(this.OnItemEquip);//取消订阅
        }

        public void SendBuyItem(int shopId, int shopItemId)
        {
            Debug.Log("SendBuyItem");

            NetMessage mesasge = new NetMessage();
            mesasge.Request = new NetMessageRequest();
            mesasge.Request.itemBuy = new ItemBuyRequest();
            mesasge.Request.itemBuy.shopId = shopId;
            mesasge.Request.itemBuy.shopItemId = shopItemId;
            NetClient.Instance.SendMessage(mesasge);
        }

        private void OnItemBuy(object sender, ItemBuyResponse message)
        {
            MessageBox.Show("购买结果：" + message.Result + "\n" + message.Errormsg,"购买完成");
        }

        Item pendingEquip = null;//当前装备
        bool isEquip;

        public bool SendEquipItem(Item equip,bool isEquip)
        {
            if (pendingEquip != null)
                return false;
            Debug.Log("SendEquipItem");

            pendingEquip = equip;//通知客户端当前装备
            this.isEquip = isEquip;

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.itemEquip = new ItemEquipRequest();
            message.Request.itemEquip.Slot = (int)equip.EquipInfo.Slot;//槽位
            message.Request.itemEquip.itemId = equip.Id;
            message.Request.itemEquip.isEquip = isEquip;
            NetClient.Instance.SendMessage(message);
            return true;

        }

        private void OnItemEquip(object sender, ItemEquipResponse message)
        {
            if (message.Result == Result.Success)//收到装备成功后调用EquipManager内方法更改客户端数据
            {
                if (pendingEquip != null)
                {
                    if (this.isEquip)
                        EquipManager.Instance.OnEquipItem(pendingEquip);//穿
                    else
                        EquipManager.Instance.OnUnEquipItem(pendingEquip.EquipInfo.Slot);//脱
                    pendingEquip = null;    
                }
            }
        }

    }
}
