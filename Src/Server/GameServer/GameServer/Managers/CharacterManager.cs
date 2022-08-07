using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Message;
using GameServer.Entities;

namespace GameServer.Managers
{
    class CharacterManager : Singleton<CharacterManager>
    {
        public Dictionary<int, Character> Characters = new Dictionary<int, Character>();

        public CharacterManager()
        {
        }

        public void Dispose()
        {
        }

        public void Init()
        {

        }

        public void Clear()
        {
            this.Characters.Clear();
        }

        public Character AddCharacter(TCharacter cha)
        {
            Log.InfoFormat("AddCharacter:cha:{0}",cha.ID);
            Character character = new Character(CharacterType.Player, cha);//将DB对象转变为实体对象，Tcharacter ——》character
            EntityManager.Instance.AddEntity(cha.MapID, character);
            character.Info.EntityId = character.entityId;//将entityid同步到网络层
            this.Characters[cha.ID] = character;
            return character;
        }


        public void RemoveCharacter(int characterId)
        {
            Character cha;
            this.Characters.TryGetValue(characterId,out cha);
            if (cha == null)
                return;
            EntityManager.Instance.RemoveEntity(cha.Data.MapID, cha);
            this.Characters.Remove(characterId);
        }

        public Character GetCharacter(int characterId)
        {
            Character character = null;
            this.Characters.TryGetValue(characterId, out character);
            return character;
        }
    }
}
