using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DangerDodger.Classes
{
    class BossInfo
    {
        public BossInfo()
        {
        }
        public BossInfo(string name, string[][] attacks)
        {
            Name = name;
            Attacks = new List<BossAttack>();
            foreach (string[] attack in attacks)
            {
                Attacks.Add(new BossAttack()
                {
                    Name = attack[0],
                    CoveredArea = (AttackStyle)Enum.Parse(typeof(AttackStyle), attack[1], true)
                });
            }
        }
        public string Name { get; set; }
        public List<BossAttack> Attacks { get; set; }

        public class BossAttack
        {
            public string Name { get; set; }
            public AttackStyle CoveredArea { get; set; }//Might use string for this if JsonConvert.DeserializeObject can't parse enums
            public Nullable<int> Value { get; set; }
        }

        public enum AttackStyle
        {
            AnywhereButCaster,
            Line,
            Circle,
            Sector
        }
    }
}
