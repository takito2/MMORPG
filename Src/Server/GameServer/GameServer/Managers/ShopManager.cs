using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Network;
using SkillBridge.Message;
using Managers;
using GameServer.Managers;
using Common.Data;
using GameServer.Services;

namespace Managers
{
    class ShopManager : Singleton<ShopManager>
    {
        public Result BuyItem(NetConnection<NetSession> sender, int shopId, int shopItemId)//处理购买Item实际逻辑
        {
            if (!DataManager.Instance.Shops.ContainsKey(shopId))
                return Result.Failed;

            ShopItemDefine shopItem;
            if (DataManager.Instance.ShopItems[shopId].TryGetValue(shopItemId,out shopItem))
            {
                Log.InfoFormat("BuyItem::Chatacter:");
                if (sender.Session.Character.Gold >= shopItem.Price)
                {
                    sender.Session.Character.ItemManager.AddItem(shopItem.ItemID, shopItem.Count);//当前人物增添Item
                    sender.Session.Character.Gold -= shopItem.Price;//当前人物扣除对应花费
                    DBService.Instance.Save();
                    return Result.Success;
                }
                
            }
            return Result.Failed;
        }
    }
}
