using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsianJudger.Game.Army {
    public class Archer : RangedArmyBase {
        public Archer (OsianPlayer owner, int px, int py) : base (owner, px, py) {
            Attack = 2;
            HP = 1;
            Speed = 1;
            BreakingPower = 1;
            AttackRange = 1;
        }
    }
}
