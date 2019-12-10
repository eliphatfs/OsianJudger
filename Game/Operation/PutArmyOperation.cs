using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsianJudger.Game.Map;

namespace OsianJudger.Game.Operation {
    public class PutArmyOperation<TArmy>: OperationBase where TArmy : Army.ArmyBase {
        public override int Cost => _cost;

        public override void Action (OsianPlayer player, MapData mapData, int px, int py) {
            if (mapData.Map[px, py] is BarrackMapObject barrack) {
                if (player != barrack.Owner)
                    throw new InvalidGameOperationException ("The barrack is not yours.");
            }
            else if (py != player.BasePosY) {
                throw new InvalidGameOperationException ("Not placing army on barracks or bases.");
            }
            mapData.Army.Add ((TArmy)Activator.CreateInstance (typeof (TArmy), player, px, py));
        }

        private readonly int _cost;

        public PutArmyOperation (int cost) {
            _cost = cost;
        }
    }
}
