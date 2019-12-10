using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsianJudger.Game.Army {
    public abstract class RangedArmyBase: ArmyBase {
        public int AttackRange, Attack;
        public bool AttackedThisFrame;
        public virtual bool AOE => false;
        public RangedArmyBase (OsianPlayer owner, int px, int py) : base (owner, px, py) {
        }

        private bool _detectAttack (Map.MapData mapData) {
            if (mapData.Map[PosX, PosY] is Map.BreakableMapObject breakable)
                if (breakable.Owner != Owner)
                    return true;
            if (PosY == 0 || PosY == OsianLogic.LEN_Y - 1)
                if (PosY != Owner.BasePosY)
                    return true;
            foreach (var army in mapData.Army) {
                if (army.Owner != Owner && army.PosY.IsInRange (PosY, PosY + AttackRange * Owner.Direction) && army.PosX == PosX) {
                    if (army is Buster && army.PosY != PosY)
                        continue;
                    return true;
                }
            }
            return false;
        }

        private void _step (Map.MapData mapData) {
            PosY += Owner.Direction;
        }

        private void _doMelee (Map.MapData mapData) {
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

        private void _doAttack (Map.MapData mapData) {
            AttackedThisFrame = false;
            foreach (var army in mapData.Army.ToArray ()) {
                if (army.Owner != Owner && !(army is Buster) && army.PosY.IsInRange (PosY + Owner.Direction, PosY + Owner.Direction * AttackRange) && army.PosX == PosX) {
                    army.HP -= Attack;
                    AttackedThisFrame = true;
                    if (army.HP <= 0)
                        mapData.Army.Remove (army);
                    if (!AOE)
                        break;
                }
            }
        }

        public virtual void RangedMove (Map.MapData mapData) {
            for (int i = 0; i < Speed && !_detectAttack (mapData); i++)
                _step (mapData);
            _doMelee (mapData);
        }

        public virtual void RangedAttack (Map.MapData mapData) {
            _doAttack (mapData);
        }
    }
}
