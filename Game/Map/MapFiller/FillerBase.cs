namespace OsianJudger.Game.Map.MapFiller {
    public abstract class FillerBase {
        public FillerBase() { }
        public abstract void Fill(MapObjectBase[,] map);
    }
}
