using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsianJudger.Game.Map;

namespace OsianJudger.Game.Army {
    public abstract class ArmyBase : IComparable<ArmyBase> {
        public int HP, Speed, BreakingPower, PosX, PosY;
        public int Timestamp;
        public static volatile int NextTimestamp = 1;
        public OsianPlayer Owner;
        public ArmyBase (OsianPlayer owner, int px, int py) {
            Owner = owner;
            PosX = px;
            PosY = py;

            Timestamp = NextTimestamp++;
        }

        public virtual void BreakMove (MapData mapData) {
            if (mapData.Map[PosX, PosY] is BreakableMapObject breakable) {
                if (breakable.Owner == Owner)
                    return;
                breakable.BreakingPoints -= BreakingPower;
                if (breakable.BreakingPoints <= 0)
                    mapData.Map[PosX, PosY] = breakable.OnBroken (Owner);
            }
        }

        int IComparable<ArmyBase>.CompareTo (ArmyBase other) {
            return Timestamp.CompareTo (other.Timestamp);
        }
    }
}
