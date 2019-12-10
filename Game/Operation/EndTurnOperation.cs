using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsianJudger.Game.Map;

namespace OsianJudger.Game.Operation {
    public class EndTurnOperation : OperationBase {
        public override int Cost => 0;
        public override void Action (OsianPlayer player, MapData mapData, int px, int py) {
        }
    }
}
