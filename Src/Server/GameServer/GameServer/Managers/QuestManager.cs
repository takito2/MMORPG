using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Data;
using GameServer.Entities;
using GameServer.Services;
using Network;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class QuestManager
    {
        private Character Owner;

        public QuestManager(Character owner)
        {
            this.Owner = owner;
        }

        public void GetQuestInfos(List<NQuestInfo> list)
        {
            foreach (var quest in this.Owner.Data.Quests)
            {
                list.Add(GetQuestInfo(quest));//数据库内信息转化网络信息
            }
        }

        public NQuestInfo GetQuestInfo(TCharacterQuest quest)//将一份数据库数据构造成一份网络数据
        {
            return new NQuestInfo()
            {
                QuestId = quest.QuestID,
                QuestGuid = quest.Id,
                Status = (QuestStatus)quest.Status,
                Targets = new int[3]
                {
                    quest.Target1,
                    quest.Target2,
                    quest.Target3,
                }
            };
        }

        public Result AcceptQuest(NetConnection<NetSession> sender, int questId)
        {
            Character character = sender.Session.Character;

            QuestDefine quest;
            if (DataManager.Instance.Quests.TryGetValue(questId,out quest))//配置表校验
            {
                var dbquest = DBService.Instance.Entities.CharacterQuests.Create();
                dbquest.QuestID = questId;
                if (quest.Target1 == QuestTarget.None)//根据配置表取出的数据进行进一步判断，若任务没具体目标，对话后接取即完成
                {
                    dbquest.Status = (int)QuestStatus.Complated;
                }
                else
                {
                    dbquest.Status = (int)QuestStatus.InProgress;//有目标未完成，设置状态为进行中
                }
                sender.Session.Response.questAccept.Quest = this.GetQuestInfo(dbquest);
                character.Data.Quests.Add(dbquest);
                DBService.Instance.Save();
                return Result.Success;
            }
            else
            {
                sender.Session.Response.questAccept.Errormsg = "任务不存在";
                return Result.Failed;
            }
        }

        public Result SubmitQuest(NetConnection<NetSession> sender, int questId)
        {
            Character character = sender.Session.Character;

            QuestDefine quest;
            if (DataManager.Instance.Quests.TryGetValue(questId,out quest))
            {
                var dbquest = character.Data.Quests.Where(q => q.QuestID == quest.ID).FirstOrDefault();//在数据库中找到questId那条数据
                if (dbquest!=null)//有数据
                {
                    if (dbquest.Status != (int)QuestStatus.Complated)//非完成状态
                    {
                        sender.Session.Response.questSubmit.Errormsg = "任务未完成";
                        return Result.Failed;
                    }
                    dbquest.Status = (int)QuestStatus.Finished;
                    sender.Session.Response.questSubmit.Quest = this.GetQuestInfo(dbquest);
                    DBService.Instance.Save();

                    //处理任务奖励

                    //金币经验
                    if (quest.RewardGold > 0)
                    {
                        character.Gold += quest.RewardGold;
                    }
                    if (quest.RewardExp > 0)
                    {

                    }

                    //Item奖励
                    if (quest.RewardItem1 > 0)
                    {
                        character.ItemManager.AddItem(quest.RewardItem1, quest.RewardItem1Count);
                    }
                    if (quest.RewardItem2 > 0)
                    {
                        character.ItemManager.AddItem(quest.RewardItem2, quest.RewardItem2Count);
                    }
                    if (quest.RewardItem3 > 0)
                    {
                        character.ItemManager.AddItem(quest.RewardItem3, quest.RewardItem3Count);
                    }
                    
                    DBService.Instance.Save();
                    return Result.Success;

                }
                sender.Session.Response.questSubmit.Errormsg = "任务不存在[2]";//数据库中不存在
                return Result.Failed;
            }
            else
            {
                sender.Session.Response.questSubmit.Errormsg = "任务不存在[1]";//配置表中不存在
                return Result.Failed;
            }
        }

        
    }
}
