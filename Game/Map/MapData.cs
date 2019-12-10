using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsianJudger.Game.Map {
    public class MapData {
        public MapObjectBase[,] Map = new MapObjectBase[OsianLogic.LEN_X, OsianLogic.LEN_Y];
        public SortedSet<Army.ArmyBase> Army = new SortedSet<Army.ArmyBase> ();
    }
}
