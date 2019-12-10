using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsianJudger.Game.Army {
    public abstract class MeleeArmyBase : ArmyBase {
        public MeleeArmyBase (OsianPlayer owner, int px, int py) : base (owner, px, py) {
        }

        private void _step (Map.MapData mapData) {
            PosY += Owner.Direction;
        }

        private bool _detectAttack (Map.MapData mapData) {
            if (mapData.Map[PosX, PosY] is Map.BreakableMapObject breakable)
                if (breakable.Owner != Owner)
                    return true;
            if (PosY == 0 || PosY == OsianLogic.LEN_Y - 1)
                if (PosY != Owner.BasePosY)
                    return true;

            foreach (var army in mapData.Army) {
                if (army.Owner != Owner && army.PosY == PosY && army.PosX == PosX) {
                    return true;
                }
            }
            return false;
        }

        private void _doAttack (Map.MapData mapData) {
            foreach (var army in mapData.Army.ToArray ()) {
                if (army.Owner != Owner && army.PosY == PosY && army.PosX == PosX) {
                    int delta = Math.Min (army.HP, HP);
                    HP -= delta;
                    army.HP -= delta;
                    if (army.HP <= 0)
                        mapData.Army.Remove (army);
                    if (HP <= 0)
                        break;
                }
            }
            if (HP <= 0)
                mapData.Army.Remove (this);
        }

        public virtual void MeleeMove (Map.MapData mapData) {
            for (int i = 0; i < Speed && !_detectAttack (mapData); i++)
                _step (mapData);
            _doAttack (mapData);
        }
    }
}
