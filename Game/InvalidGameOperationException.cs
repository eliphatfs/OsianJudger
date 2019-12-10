using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsianJudger.Game {
    public class InvalidGameOperationException: InvalidOperationException {
        public InvalidGameOperationException (string message) : base (message) {
        }
    }
}
