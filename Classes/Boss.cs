using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DangerDodger.Classes
{
    class Boss
    {
        public Boss()
        {
        }
        public Boss(string name, string[][] attacks)
        {
            Name = name;
            Attack = new List<BossAttack>();
            foreach (string[] attack in attacks)
            {
                Attack.Add(new BossAttack()
                {
                    AttackName = attack[0],
                    AttackStyle = (AttackStyle)Enum.Parse(typeof(AttackStyle), attack[1], true)
                });
            }
        }
        public string Name { get; set; }
        public List<BossAttack> Attack { get; set; }

        public class BossAttack
        {
            public string AttackName { get; set; }
            public AttackStyle AttackStyle { get; set; }
        }

        public enum AttackStyle
        {
            AoE,
            Line
        }
    }
}
