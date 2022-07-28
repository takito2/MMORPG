﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Entities;
using Managers;
using Network;
using SkillBridge.Message;
using GameServer.Managers;

namespace GameServer.Services
{
    class ItemService : Singleton<ItemService>
    {
        public ItemService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ItemBuyRequest>(this.OnItemBuy);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ItemEquipRequest>(this.OnItemEquip);
        }


        public void Init()
        {

        }

        private void OnItemBuy(NetConnection<NetSession> sender, ItemBuyRequest request)//购买Item
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnItemBuy : character:{0}:Shop:{1},ShopItem:{2}",character.Id,request.shopId,request.shopItemId);
            var result = ShopManager.Instance.BuyItem(sender,request.shopId,request.shopItemId);
            sender.Session.Response.itemBuy = new ItemBuyResponse();
            sender.Session.Response.itemBuy.Result = result;
            sender.SendResponse();
        }

        private void OnItemEquip(NetConnection<NetSession> sender, ItemEquipRequest request)//穿脱装备
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnItemEquip:charahcter:{0}:Slot:{1},Item:{2},Equip:{4}",character.Id,request.Slot,request.itemId,request.isEquip);
            var result = EquipManager.Instance.EquipItem(sender,request.Slot,request.itemId,request.isEquip);
            sender.Session.Response.itemEquip = new ItemEquipResponse();
            sender.Session.Response.itemEquip.Result = result;
            sender.SendResponse();
        }
    }
}
