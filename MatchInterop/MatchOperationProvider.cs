using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using OsianJudger.Game;
using OsianJudger.Game.Operation;
using OsianJudger.Game.Army;
using OsianJudger.Game.Map;
using System.Text;

namespace OsianJudger.MatchInterop {
    public class MatchOperationProvider: OsianOperationProvider {
        Process _subProcess;

        public MatchOperationProvider (string executable) {
            ProcessStartInfo si = new ProcessStartInfo ();
            si.FileName = executable;
            si.UseShellExecute = false;
            si.RedirectStandardInput = true;
            si.RedirectStandardOutput = true;
            si.RedirectStandardError = false;
            si.CreateNoWindow = true;
            _subProcess = Process.Start (si);
        }

        public override int BasicTimeout => 700;

        private OperationBase _baseOp (int type) {
            switch (type) {
                case 0:
                    return new PutArmyOperation<Archer> (1);
                case 1:
                    return new PutArmyOperation<Buster> (2);
                case 2:
                    return new PutArmyOperation<Castle> (3);
                case 3:
                    return new PutArmyOperation<Dragon> (9);
                case 5:
                    return new PutObstacleOperation ();
                default:
                    throw new InvalidGameOperationException ("Can't put object of internal type " + type);
            }
        }

        public override async Task<OperationBase> NextOperation () {
            if (_subProcess.HasExited)
                return new EndTurnOperation ();
            var line = await _subProcess.StandardOutput.ReadLineAsync ();
            // Console.WriteLine (line);
            // Console.WriteLine ("");
            if (line.Contains ("Launch!"))
                return new EndTurnOperation ();
            var splited = line.Split (new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (splited.Length != 3)
                throw new InvalidGameOperationException ("Invalid Operation Command: " + line);
            int[] data = new int[3];
            for (int i = 0; i < 3; i++)
                if (!int.TryParse (splited[i], out data[i]))
                    throw new InvalidGameOperationException ("Operand can't be decoded as integers: " + splited[i]);
            var bop = _baseOp (data[2]);
            bop.Px = data[0];
            bop.Py = data[1];
            return bop;
        }

        private string _composeString(int objType, int owner, int hp, int bp, int cb) {
            return string.Join (" ", objType, owner, hp, bp, cb);
        }

        private int _composeOwner (OsianPlayer target, OsianPlayer you) {
            return target == null ? 0 : target == you ? 1 : 2;
        }

        public override async Task UpdateGameData (OsianPlayer you, OsianPlayer opponent, MapData mapData) {
            if (_subProcess.HasExited)
                return;
            StringBuilder builder = new StringBuilder ();
            builder.AppendLine ("1");
            builder.AppendFormat ("{0} {1} {2} {3}\n", you.Cost, opponent.Cost, you.Base, opponent.Base);
            List<string>[,] temps = new List<string>[OsianLogic.LEN_X, OsianLogic.LEN_Y];
            for (int i = 0; i < OsianLogic.LEN_X; i++) {
                for (int j = 0; j < OsianLogic.LEN_Y; j++) {
                    temps[i, j] = new List<string> ();
                    List<string> temp = temps[i, j];
                    if (mapData.Map[i, j] is BarrackMapObject barrack)
                        temp.Add (_composeString (4, _composeOwner (barrack.Owner, you), 0, barrack.BreakingPoints, 0));
                    else if (mapData.Map[i, j] is ObstacleMapObject obstacle)
                        temp.Add (_composeString (5, _composeOwner (obstacle.Owner, you), 0, obstacle.BreakingPoints, obstacle.CostBonus));
                }
            }
            foreach (var item in mapData.Army) {
                List<string> temp = temps[item.PosX, item.PosY];
                if (item is Archer archer)
                    temp.Add (_composeString (0, _composeOwner (archer.Owner, you), archer.HP, 0, 0));
                else if (item is Buster buster)
                    temp.Add (_composeString (1, _composeOwner (buster.Owner, you), buster.HP, 0, 0));
                else if (item is Castle castle)
                    temp.Add (_composeString (2, _composeOwner (castle.Owner, you), castle.HP, 0, 0));
                else if (item is Dragon dragon)
                    temp.Add (_composeString (3, _composeOwner (dragon.Owner, you), dragon.HP, 0, 0));
            }
            for (int i = 0; i < OsianLogic.LEN_X; i++) {
                for (int j = 0; j < OsianLogic.LEN_Y; j++) {
                    List<string> temp = temps[i, Math.Abs (you.BasePosY - j)];
                    builder.Append (temp.Count);
                    builder.Append (" ");
                    builder.Append (string.Join (" ", temp));
                    builder.Append (" ");
                }
            }
            await _subProcess.StandardInput.WriteLineAsync (builder.ToString ());
        }

        protected override void DisposeMe () {
            base.DisposeMe ();
            if (_subProcess != null) {
                try {
                    if (!_subProcess.HasExited) {
                        _subProcess.StandardInput.WriteLine ("0");
                        _subProcess.WaitForExit (1000);
                    }
                } catch {

                }
                if (!_subProcess.HasExited)
                    _subProcess.Kill ();
                _subProcess = null;
            }
        }
    }
}
