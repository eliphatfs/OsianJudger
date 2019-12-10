using OsianJudger.Game.Map.MapFiller;
using System;
using System.Collections.Generic;

namespace OsianJudger.Game.Map {
	/// <summary>
	/// The Class that handles map generation process 
	/// </summary>
	public class MapGenerator {
		private readonly List<FillerBase> fillers;

		public MapGenerator() {
			fillers = new List<FillerBase>();
			fillers.Add(new RandomFiller(new Random(1337)));
			fillers.Add(new RandomFiller(new Random(1903802)));
			fillers.Add(new RandomFiller(new Random(1896)));
			fillers.Add(new EmptyFiller());
			fillers.Add(new FullFiller());
			fillers.Add(new ChessFiller());
		}

		public List<MapObjectBase[,]> Generate() {
			List<MapObjectBase[,]> maps = new List<MapObjectBase[,]>();
			foreach (var filler in fillers) {
				var map = new MapObjectBase[OsianLogic.LEN_X, OsianLogic.LEN_Y];
				for (int i = 0; i < OsianLogic.LEN_X; i++)
					for (int j = 0; j < OsianLogic.LEN_Y; j++)
						map[i, j] = new EmptyMapObject();
				filler.Fill(map);
				maps.Add(map);
			}
			return maps;
		}
	}
}
