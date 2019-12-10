using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsianJudger.Game.Army {
    public class Dragon : RangedArmyBase {
        public override bool AOE => true;
        public Dragon (OsianPlayer owner, int px, int py) : base (owner, px, py) {
            HP = 4;
            Speed = 1;
            BreakingPower = 2;
            Attack = 5;
            AttackRange = 2;
        }
    }
}
