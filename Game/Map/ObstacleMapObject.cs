using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsianJudger.Game.Map {
    public class ObstacleMapObject : BreakableMapObject {
        public int CostBonus;

        public override MapObjectBase OnBroken (OsianPlayer source) {
            source.Cost += CostBonus;
            return new EmptyMapObject ();
        }
    }
}
