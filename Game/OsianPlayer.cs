using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsianJudger.Game.Operation;

namespace OsianJudger.Game {
    public class OsianPlayer {
        private int _direction = 1;
        public int Direction {
            get => _direction;
            set {
                _direction = Math.Sign (value);
                if (_direction == 0)
                    throw new InvalidOperationException ();
            }
        }

        public int BasePosY => Direction == 1 ? 0 : OsianLogic.LEN_Y - 1;

        private int _cost = 0;
        public int Cost {
            get {
                return _cost;
            }
            set {
                if (value < 0)
                    throw new InvalidGameOperationException ("Not enough cost.");
                _cost = value;
            }
        }
        public int Base = 10;
    }
}
