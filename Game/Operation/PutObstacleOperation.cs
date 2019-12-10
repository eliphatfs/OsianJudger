using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsianJudger.Game.Map;

namespace OsianJudger.Game.Operation {
    public class PutObstacleOperation: OperationBase {
        public override int Cost => 3;

        public override void Action (OsianPlayer player, MapData mapData, int px, int py) {
            if (mapData.Map[px, py] is EmptyMapObject && py != 0 && py != OsianLogic.LEN_Y - 1 && mapData.Army.Count ((army) => army.PosX == px && army.PosY == py) == 0) {
                mapData.Map[px, py] = new ObstacleMapObject () {
                    InitialPoints = 5,
                    BreakingPoints = 5,
                    CostBonus = 0,
                    Owner = player
                };
            }
        }
    }
}
