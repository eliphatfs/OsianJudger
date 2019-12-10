using OsianJudger.Game;
using OsianJudger.Game.Map;
using OsianJudger.MatchInterop;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsianJudger {
    class Program {
        static List<MapObjectBase[,]> _maps = new List<MapObjectBase[,]>();


        // Moved to RandomFiller

        //static MapObjectBase[,] _generateMap(int seed) {
        //  Random random = new Random(seed);
        //  var map = new MapObjectBase[OsianLogic.LEN_X, OsianLogic.LEN_Y];
        //  for (int i = 0; i < OsianLogic.LEN_X; i++)
        //      for (int j = 0; j < OsianLogic.LEN_Y; j++)
        //          map[i, j] = new EmptyMapObject();
        //  for (int i = 0; i < 5; i++) {
        //      int strength = random.Next(1, 5);
        //      int x = random.Next(OsianLogic.LEN_X);
        //      int y = random.Next(OsianLogic.LEN_Y - 2) + 1;
        //      map[x, y] = new BarrackMapObject() {
        //          InitialPoints = strength,
        //          BreakingPoints = strength,
        //          Owner = null
        //      };
        //      map[OsianLogic.LEN_X - 1 - x, OsianLogic.LEN_Y - 1 - y] = new BarrackMapObject() {
        //          InitialPoints = strength,
        //          BreakingPoints = strength,
        //          Owner = null
        //      };
        //  }
        //  for (int i = 0; i < 10; i++) {
        //      int strength = random.Next(2, 10);
        //      int bonus = random.Next(1, 15);
        //      int x = random.Next(OsianLogic.LEN_X);
        //      int y = random.Next(OsianLogic.LEN_Y - 2) + 1;
        //      map[x, y] = new ObstacleMapObject() {
        //          InitialPoints = strength,
        //          BreakingPoints = strength,
        //          CostBonus = bonus,
        //          Owner = null
        //      };
        //      map[OsianLogic.LEN_X - 1 - x, OsianLogic.LEN_Y - 1 - y] = new ObstacleMapObject() {
        //          InitialPoints = strength,
        //          BreakingPoints = strength,
        //          CostBonus = bonus,
        //          Owner = null
        //      };
        //  }
        //  return map;
        //}

        static SortedDictionary<string, string> _compiledFiles = new SortedDictionary<string, string>();
        static SortedDictionary<string, string> _results = new SortedDictionary<string, string>();

        static void _compile(string dir, BinaryWriter rwriter) {
            var files = string.Join(" ", Directory.EnumerateFiles(dir, "*.cpp", SearchOption.AllDirectories));
            var result = MatchCompiler.Compile(files);
            if (!result.IsCompileSuccessful) {
                rwriter.Write(2);
                rwriter.Write(Path.GetFileNameWithoutExtension(dir));
                rwriter.Write("Compile Error.\n" + result.CompilerHints);
                _results.Add(dir, "Compile Error.\n" + result.CompilerHints);
            } else
                _compiledFiles.Add(dir, result.CompiledExecutablePath);
        }

        static void ResetMaps() {
            MapGenerator mapGenerator = new MapGenerator();
            _maps = mapGenerator.Generate();
        }

        static void Main(string[] args) {
            using (var binresult = File.Create("Results.bin"))
            using (var rwriter = new BinaryWriter(binresult))
                if (args.Length <= 1) {
                    Console.WriteLine("Osian Judger Program.");
                    Console.WriteLine("Usage:");
                    Console.WriteLine("<this program> <compiler> <folder of sources>");
                } else {
                    MatchCompiler.COMPILER_COMMAND = args[0];
                    var listDir = Directory.GetDirectories(args[1]);
                    for (int i = 0; i < listDir.Length; i++) {
                        _compile(listDir[i], rwriter);
                    }
                    using (var stream = File.CreateText("Results.txt")) {
                        for (int i = 0; i < listDir.Length; i++) {
                            int wins = 0;
                            int matches = 0;
                            if (!_compiledFiles.ContainsKey(listDir[i]))
                                continue;
                            for (int j = 0; j < listDir.Length; j++) {
                                if (i == j)
                                    continue;
                                if (!_compiledFiles.ContainsKey(listDir[j]))
                                    continue;
                                var exe1 = _compiledFiles[listDir[i]];
                                var exe2 = _compiledFiles[listDir[j]];
                                ResetMaps();
                                for (int m = 0; m < _maps.Count; m++) {
                                    matches++;
                                    Game.Replay.ReplayOrganizer org = new Game.Replay.ReplayOrganizer();
                                    using (var judger = new OsianLogic(
                                        _maps[m],
                                        org,
                                        new MatchOperationProvider(exe1),
                                        new MatchOperationProvider(exe2)
                                    )) {
                                        switch (judger.Run()) {
                                            case 0:
                                                wins++;
                                                org.WriteReplay(string.Format("{0}-{1}-map{2}-win-{3}.html", Path.GetFileNameWithoutExtension(listDir[i]), Path.GetFileNameWithoutExtension(listDir[j]), m, DateTime.Now.Ticks));
                                                stream.WriteLine(string.Format("{0} wins {1} on map {2}.", Path.GetFileNameWithoutExtension(listDir[i]), Path.GetFileNameWithoutExtension(listDir[j]), m));
                                                break;
                                            case 1:
                                                org.WriteReplay(string.Format("{0}-{1}-map{2}-lose-{3}.html", Path.GetFileNameWithoutExtension(listDir[i]), Path.GetFileNameWithoutExtension(listDir[j]), m, DateTime.Now.Ticks));
                                                stream.WriteLine(string.Format("{0} loses in the game with {1} on map {2}.", Path.GetFileNameWithoutExtension(listDir[i]), Path.GetFileNameWithoutExtension(listDir[j]), m));
                                                break;
                                            case -1:
                                                org.WriteReplay(string.Format("{0}-{1}-map{2}-tie-{3}.html", Path.GetFileNameWithoutExtension(listDir[i]), Path.GetFileNameWithoutExtension(listDir[j]), m, DateTime.Now.Ticks));
                                                stream.WriteLine(string.Format("{0} ties in the game with {1} on map {2}.", Path.GetFileNameWithoutExtension(listDir[i]), Path.GetFileNameWithoutExtension(listDir[j]), m));
                                                break;
                                        }
                                    }
                                }
                            }
                            rwriter.Write(1);
                            rwriter.Write(Path.GetFileNameWithoutExtension(listDir[i]));
                            rwriter.Write(wins);
                            rwriter.Write(matches);
                            rwriter.Write(matches > 0 ? wins * 100.0 / matches : 0.0);
                            _results.Add(Path.GetFileNameWithoutExtension(listDir[i]), string.Format("{0} wins in {1} matches, rate {2}%.", wins, matches, matches > 0 ? wins * 100 / matches : 0));
                        }
                        foreach (var kv in _results) {
                            stream.WriteLine(kv.Key + " - " + kv.Value);
                        }
                        rwriter.Write(0);
                    }
                    foreach (var item in _compiledFiles.Values) {
                        File.Delete(item);
                    }
                }
        }
    }
}
