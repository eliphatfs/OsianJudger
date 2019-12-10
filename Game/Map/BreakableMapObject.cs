using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsianJudger.Game.Map {
    public class BreakableMapObject : MapObjectBase {
        public int InitialPoints;
        public int BreakingPoints;
        public OsianPlayer Owner;

        public virtual MapObjectBase OnBroken (OsianPlayer source) {
            return this;
        }
    }
}
