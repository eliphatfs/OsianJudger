using System;

namespace OsianJudger.Game.Map.MapFiller {
    /// <summary>
    /// Randomly fill the map, support seed
    /// </summary>
    public class RandomFiller : FillerBase {
        private readonly Random random;
        public RandomFiller(Random random) : base() {
            this.random = random;
        }
        public override void Fill(MapObjectBase[,] map) {
            for (int i = 0; i < 5; i++) {
                int strength = random.Next(1, 5);
                int x = random.Next(OsianLogic.LEN_X);
                int y = random.Next(OsianLogic.LEN_Y - 2) + 1;
                map[x, y] = new BarrackMapObject() {
                    InitialPoints = strength,
                    BreakingPoints = strength,
                    Owner = null
                };
                map[OsianLogic.LEN_X - 1 - x, OsianLogic.LEN_Y - 1 - y] = new BarrackMapObject() {
                    InitialPoints = strength,
                    BreakingPoints = strength,
                    Owner = null
                };
            }
            for (int i = 0; i < 10; i++) {
                int strength = random.Next(2, 10);
                int bonus = random.Next(1, 15);
                int x = random.Next(OsianLogic.LEN_X);
                int y = random.Next(OsianLogic.LEN_Y - 2) + 1;
                map[x, y] = new ObstacleMapObject() {
                    InitialPoints = strength,
                    BreakingPoints = strength,
                    CostBonus = bonus,
                    Owner = null
                };
                map[OsianLogic.LEN_X - 1 - x, OsianLogic.LEN_Y - 1 - y] = new ObstacleMapObject() {
                    InitialPoints = strength,
                    BreakingPoints = strength,
                    CostBonus = bonus,
                    Owner = null
                };
            }
        }
    }
}
