using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsianJudger.Game.Operation {
    public abstract class OperationBase {
        public int Px, Py;
        public abstract int Cost { get; }

        public abstract void Action (OsianPlayer player, Map.MapData mapData, int px, int py);

        public void Invoke (OsianPlayer player, Map.MapData mapData) {
            int x = Px;
            int y = Math.Abs (player.BasePosY - Py);
            x.XRangeCheck (); y.YRangeCheck ();
            player.Cost -= Cost;
            Action (player, mapData, x, y);
        }
    }
}
