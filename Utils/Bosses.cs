using DangerDodger.Classes;
using System.Collections.Generic;

namespace DangerDodger.Utils
{
    class Bosses
    {
        public static List<Boss> AllBosses = new List<Boss>()
        {
            new Boss("boss1", new[] {
                new[]{ "boss1attack1", "AoE" },
                new[]{ "boss1attack2", "AoE" }
            }),
            new Boss("boss2", new[] {
                new[]{ "boss2attack1", "Line" },
                new[]{ "boss2attack2", "AoE" }
            })
        };
    }
}
