using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsianJudger.Game.Army {
    public class Castle : MeleeArmyBase {
        public Castle (OsianPlayer owner, int px, int py) : base (owner, px, py) {
            HP = 6;
            Speed = 2;
            BreakingPower = 1;
        }
    }
}
