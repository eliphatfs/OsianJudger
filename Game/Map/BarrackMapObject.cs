using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsianJudger.Game.Map {
    public class BarrackMapObject : BreakableMapObject {
        public override MapObjectBase OnBroken (OsianPlayer source) {
            Owner = source;
            BreakingPoints = InitialPoints;
            return this;
        }
    }
}
