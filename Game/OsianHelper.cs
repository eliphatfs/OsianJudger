using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OsianJudger.Game.OsianLogic;

namespace OsianJudger.Game {
    public static class OsianHelper {
        public static void XRangeCheck (this int px) {
            if (px < 0 || px >= LEN_X)
                throw new InvalidGameOperationException ("Row out of range: " + px);
        }
        public static void YRangeCheck (this int py) {
            if (py < 0 || py >= LEN_Y)
                throw new InvalidGameOperationException ("Column out of range: " + py);
        }
        public static bool IsInRange (this int v, int a, int b) {
            return (v >= a && v <= b) || (v <= a && v >= b);
        }
    }
}
