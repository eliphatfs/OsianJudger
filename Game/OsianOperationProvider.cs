using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsianJudger.Game.Map;

namespace OsianJudger.Game {
    public abstract class OsianOperationProvider : IDisposable {
        public abstract Task UpdateGameData (OsianPlayer you, OsianPlayer opponent, MapData mapData);
        public abstract Task<Operation.OperationBase> NextOperation ();
        public abstract int BasicTimeout { get; }
        protected virtual void DisposeMe () {

        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose (bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    DisposeMe ();
                }

                disposedValue = true;
            }
        }

        void IDisposable.Dispose () {
            Dispose (true);
        }
        #endregion
    }
}
