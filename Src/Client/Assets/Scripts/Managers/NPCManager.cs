using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Data;

namespace Managers
{
    /// <summary>
    /// NPC管理器，主要负责提供，NPC交互方法（注册），及交互实际逻辑
    /// </summary>
    class NPCManager : Singleton<NPCManager>//
    {
        public delegate bool NpcActionHandler(NpcDefine npc);

        Dictionary<NpcFunction, NpcActionHandler> eventMap = new Dictionary<NpcFunction, NpcActionHandler>();

        public void RegisterNpcEvent(NpcFunction function,NpcActionHandler action)//注册方法
        {
            if (!eventMap.ContainsKey(function))
            {
                eventMap[function] = action;//没有则新建并赋值
            }
            else
            {
                eventMap[function] += action;//已有加入多播行列
            }
        }

        public NpcDefine GetNpcDefine(int npcID)
        {
            NpcDefine npc = null;
            DataManager.Instance.NPCs.TryGetValue(npcID, out npc);
            return npc;
        }

        public bool Interactive(NpcDefine npc)//根据NpcDefine进行交互，分类型交互
        {
            if (DoTaskInteractive(npc))//任务
            {
                return true;
            }
            else if (npc.Type == NpcType.Functional)//其他功能
            {
                return DoFunctionInteractive(npc);
            }
            return false;
        }

        public bool Interactive(int npcId)//根据ID进行交互
        {
            if (DataManager.Instance.NPCs.ContainsKey(npcId))
            {
                var npc = DataManager.Instance.NPCs[npcId];
                return Interactive(npc);
            }
            return false;
        }

        

        private bool DoTaskInteractive(NpcDefine npc)//任务系统交互
        {
            var status = QuestManager.Instance.GetQuestStatusByNpc(npc.ID);
            if (status == NpcQuestStatus.None)
                return false;
            return QuestManager.Instance.OpenNpcQuest(npc.ID);//打开对话框
        }

        private bool DoFunctionInteractive(NpcDefine npc)
        {
            if (npc.Type != NpcType.Functional)
                return false;
            if (!eventMap.ContainsKey(npc.Function))
                return false;
            return eventMap[npc.Function](npc);//调用事件执行，进行分发，npc为参数
        }

        
    }
}
