namespace OsianJudger.Game.Map.MapFiller {
	// As its name suggests, fill the map with obstacles
	public class FullFiller : FillerBase {
		public FullFiller() : base() {
		}
		public override void Fill(MapObjectBase[,] map) {
			for (int i = 0; i < OsianLogic.LEN_X; i++) {
				for (int j = 1; j < OsianLogic.LEN_Y - 1; j++) {
					map[i, j] = new ObstacleMapObject() {
						InitialPoints = 10,
						BreakingPoints = 10,
						CostBonus = 10,
						Owner = null
					};
				}
			}
		}
	}
}
