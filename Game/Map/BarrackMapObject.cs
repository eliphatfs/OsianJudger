namespace OsianJudger.Game.Map {
    public class BarrackMapObject : BreakableMapObject {
        public override MapObjectBase OnBroken(OsianPlayer source) {
            Owner = source;
            BreakingPoints = InitialPoints;
            return this;
        }
    }
}
