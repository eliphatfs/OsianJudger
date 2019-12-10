namespace OsianJudger.Game.Map.MapFiller {
	// As its name suggests, fill the chess board with map
	public class ChessFiller : FillerBase {
		public ChessFiller() : base() {
		}
		public override void Fill(MapObjectBase[,] map) {
			for (int i = 0; i < OsianLogic.LEN_X; i++) {
				for (int j = 1; j < OsianLogic.LEN_Y - 1; j++) {
					if ((i + j) % 2 == 0) {
						map[i, j] = new ObstacleMapObject() {
							InitialPoints = 6,
							BreakingPoints = 6,
							CostBonus = 6,
							Owner = null
						};
					} else {
						map[i, j] = new BarrackMapObject() {
							InitialPoints = 3,
							BreakingPoints = 3,
							Owner = null
						};
					}
				}
			}
		}
	}
}
