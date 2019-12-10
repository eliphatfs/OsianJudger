using System;
using System.Collections.Generic;
using System.IO;

namespace OsianJudger.Game.Replay {
    public class ReplayOrganizer {
        public OsianLogic BoundLogic;
        public List<string> frames = new List<string> ();

        public void Bind (OsianLogic logic) {
            BoundLogic = logic;
        }

        public void AddFrame (ReplayRecorder recorder, bool force = false) {
            if (force || recorder.HasChanges (BoundLogic))
                frames.Add (recorder.ToString ());
        }

        public void AddMessage (string message) {
            frames.Add ("{w:\"$\"}".Replace ("$", message));
        }

        public void WriteReplay (string path) {
            File.WriteAllText (path, ReplayTemplate.Template.Replace ("$_DATA", string.Join (",", frames)));
        }
    }
}