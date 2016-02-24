using Loki.Bot.Logic.Bots.OldGrindBot;
using Loki.Bot.Pathfinding;
using Loki.Common;
using Loki.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DangerDodger.Utils
{
    class MoveHelper
    {
        public static Vector2i CalcSafePosition(double escapeAngle, float safeDistance)
        {
            Vector2i result = new Vector2i();
            for (int angleModificator = 0; angleModificator < 36; angleModificator++)
            {
                //We prioritize a route as close to our escape angle as possible
                double angle = escapeAngle + (Math.Ceiling((double)angleModificator / 2) * 0.174533 * (angleModificator % 2 == 0 ? 1 : -1));
                Vector2i pos = GeometryHelper.GetPointOnCircle(LokiPoe.Me.Position, angle, safeDistance);
                bool skipPos = false;
                try
                {
                    skipPos = !ExilePather.NavigationGrid.IsWalkable(pos) ||
                        !ExilePather.CanObjectSee(LokiPoe.Me, pos, true) ||
                        Utility.ClosedDoorBetween(LokiPoe.Me, pos, 10, 10, true);
                }
                catch
                {
                    skipPos = true;
                }
                if (skipPos) continue;
                result = pos;
                break;

            }
            return result;
        }
    }
}
