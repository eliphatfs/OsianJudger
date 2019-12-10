using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsianJudger.Game.Army {
    public class Buster : MeleeArmyBase {
        public Buster (OsianPlayer owner, int px, int py) : base (owner, px, py) {
            HP = 2;
            Speed = 3;
            BreakingPower = 1;
        }
    }
}
