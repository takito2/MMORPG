using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Services;
using Common.Data;

namespace Managers
{
    class ShopManager : Singleton<ShopManager>
    {
        public void Init()
        {
            NPCManager.Instance.RegisterNpcEvent(NpcFunction.InvokeShop, OnOpenShop);//注册npc启用方法
        }

        private bool OnOpenShop(NpcDefine npc)
        {
            this.ShowShop(npc.Param);
            return true;
        }

        private void ShowShop(int shopId)
        {
            ShopDefine shop;
            if (DataManager.Instance.Shops.TryGetValue(shopId,out shop))
            {
                UIShop uIShop = UIManager.Instance.Show<UIShop>();//打开商店
                if (uIShop != null)
                {
                    uIShop.SetShop(shop);//根据shopid设置商店信息
                }
            }
        }

        public  bool BuyItem(int shopId,int shopItemId)
        {
            ItemService.Instance.SendBuyItem(shopId, shopItemId);
            return true;
        }
    }
}
