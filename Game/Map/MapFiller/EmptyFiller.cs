namespace OsianJudger.Game.Map.MapFiller {
    // As its name suggests, don't fill the map!
    public class EmptyFiller : FillerBase {
        public EmptyFiller() : base() {
        }
        public override void Fill(MapObjectBase[,] map) {
            for (int i = 0; i < OsianLogic.LEN_X; i++) {
                for (int j = 1; j < OsianLogic.LEN_Y - 1; j++) {
                    // Empty
                }
            }
        }
    }
}
