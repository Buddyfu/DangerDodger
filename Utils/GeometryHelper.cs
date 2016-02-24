using Loki.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DangerDodger.Utils
{
    class GeometryHelper
    {
        public static Vector2 getAveragePoint(List<Vector2> pts)
        {
            return new Vector2()
            {
                X = pts.Average(p => p.X),
                Y = pts.Average(p => p.Y)
            };
        }
        public static double getAngleBetweenPoints(Vector2 pt1, Vector2 pt2)
        {
            return Math.Atan2(pt2.Y - pt1.Y, pt2.X - pt1.X);
        }

        public static Vector2i GetPointOnCircle(Vector2i center, double radian, double radius)
        {
            return new Vector2i()
            {
                X = center.X + (int)(radius * Math.Cos(radian)),
                Y = center.Y + (int)(radius * Math.Sin(radian))
            };
        }
    }
}
