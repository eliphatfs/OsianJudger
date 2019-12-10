using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsianJudger.Game.Map;
using OsianJudger.Game.Army;

namespace OsianJudger.Game.Replay {
    public class ReplayRecorder {
        public struct RecordData {
            public int x, a, o;
            public float y;
            public string t, c;
            public static RecordData Make (int x_, float y_, int a_, int o_, string t_, string c_) {
                return new RecordData () {
                    x = x_,
                    y = y_,
                    a = a_,
                    o = o_,
                    t = t_,
                    c = c_
                };
            }

            public override string ToString () {
                StringBuilder builder = new StringBuilder ();
                return builder.Append ("{")
                    .Append ("x:").Append (x)
                    .Append (",y:").Append (y)
                    .Append (",a:").Append (a)
                    .Append (",o:").Append (o)
                    .Append (",t:'").Append (t)
                    .Append ("',c:'").Append (c)
                    .Append ("'}")
                    .ToString ();
            }
        }
        SortedDictionary<string, RecordData> _trackedStart = new SortedDictionary<string, RecordData> ();
        SortedDictionary<string, RecordData> _trackedEnd = new SortedDictionary<string, RecordData> ();
        int[] _basics = new int[4];

        private string _armyStr (ArmyBase army, string postfix) {
            if (army is Archer) return "archer" + postfix;
            if (army is Buster) return "buster";
            if (army is Castle) return "castle";
            if (army is Dragon) return "dragon";
            return null;
        }

        private void _record (OsianLogic logic, SortedDictionary<string, RecordData> put, bool attackFrame = false) {
            var left = logic.Players[0];
            var data = logic.Map;
            for (int i = 0; i < OsianLogic.LEN_X; i++) {
                for (int j = 0; j < OsianLogic.LEN_Y; j++) {
                    if (data.Map[i, j] is BarrackMapObject b) {
                        put.Add ("a" + i + "," + j, RecordData.Make (i, j, 1, b.Owner == left ? 0 : 1, "team" + (b.Owner == left ? "l" : (b.Owner == null ? "n" : "r")), ""));
                    }
                    if (data.Map[i, j] is ObstacleMapObject ob) {
                        put.Add ("a" + i + "," + j, RecordData.Make (i, j, 1, ob.Owner == left ? 0 : 1, "obstacle", ""));
                    }
                }
            }
            Dictionary<string, string> dup = new Dictionary<string, string> ();
            foreach (var army in data.Army) {
                var keystr = _armyStr (army, "");
                keystr = ((int)('e' - keystr[0])) + keystr;
                var key = "m" + army.Timestamp + "." + keystr;
                var dupkey = keystr + army.PosX + "," + army.PosY;
                if (dup.ContainsKey (dupkey)) {
                    var v = put[dup[dupkey]];
                    v.c = "x" + (int.Parse (v.c.Substring (1)) + 1);
                    put[dup[dupkey]] = v;
                }
                else {
                    var atk = attackFrame;
                    var rd = RecordData.Make (army.PosX, army.PosY - (atk ? 0.02f : 0.0f), 1, army.Owner == left ? 0 : 1, _armyStr (army, atk ? "a" : ""), "x1");
                    put.Add (key, rd);
                    dup.Add (dupkey, key);
                }
            }
        }

        public void RecordStartFrame (OsianLogic logic, bool attackFrame = false) {
            _basics = new[] { logic.Players[0].Cost, logic.Players[1].Cost, logic.Players[0].Base, logic.Players[1].Base };
            _record (logic, _trackedStart, attackFrame);
        }

        public void RecordEndFrame (OsianLogic logic) {
            _record (logic, _trackedEnd);
            MakeConsistent ();
        }

        private void _mconsistent (SortedDictionary<string, RecordData> src, SortedDictionary<string, RecordData> dst) {
            foreach (var item in src) {
                if (!dst.ContainsKey (item.Key)) {
                    var v = item.Value;
                    v.a = 0;
                    dst.Add (item.Key, v);
                }
            }
        }

        public void MakeConsistent () {
            _mconsistent (_trackedStart, _trackedEnd);
            _mconsistent (_trackedEnd, _trackedStart);
        }

        public bool HasChanges (OsianLogic logic) {
            var nb = new[] { logic.Players[0].Cost, logic.Players[1].Cost, logic.Players[0].Base, logic.Players[1].Base };
            if (!nb.SequenceEqual (_basics))
                return true;
            foreach (var item in _trackedStart) {
                if (_trackedEnd[item.Key].ToString () != _trackedStart[item.Key].ToString ()) {
                    return true;
                }
            }
            return false;
        }

        public override string ToString () {
            StringBuilder builder = new StringBuilder ();
            builder.Append ("{");
            builder.AppendFormat ("c:{0},", _trackedStart.Count);
            builder.AppendFormat ("b:[{0}],", string.Join (",", _basics));
            builder.Append ("s:[");
            foreach (var item in _trackedStart) {
                builder.Append (item.Value.ToString ()).Append (',');
            }
            builder.Append ("],e:[");
            foreach (var item in _trackedEnd) {
                builder.Append (item.Value.ToString ()).Append (',');
            }
            builder.Append ("]}");
            return builder.ToString ();
        }
    }
}
