using Loki.Common;
using Loki.Game.GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DangerDodger.Classes
{
    class CachedMonster : CachedNetworkObject
    {
        public Rarity Rarity { get; set; }
        public string MonsterTypeMetadata { get; set; }
        public string currentSkillName { get; set; }
        public Vector2i currentSkillDestination { get; set; }
        public bool hasAuraMonsterCannotDie { get; set; }
        public bool hasSkillSuicideExplosion { get; set; }
    }
}
