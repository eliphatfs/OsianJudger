using OsianJudger.Game.Army;
using OsianJudger.Game.Map;
using OsianJudger.Game.Operation;
using OsianJudger.Game.Replay;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OsianJudger.Game {
	public class OsianLogic : IDisposable {
		public const int LEN_X = 5, LEN_Y = 12;
		public OsianPlayer[] Players = new OsianPlayer[2];
		public OsianOperationProvider[] OperationProviders = new OsianOperationProvider[2];
		public MapData Map;
		public ReplayOrganizer ReplayOrganizer;

		public OsianLogic(MapObjectBase[,] gameMap, ReplayOrganizer organizer, params OsianOperationProvider[] operationProviders) {
			if (operationProviders.Length != 2)
				throw new NotSupportedException();
			Map = new MapData();
			Map.Map = gameMap;
			organizer.Bind(this);
			ReplayOrganizer = organizer;
			Array.Copy(operationProviders, OperationProviders, operationProviders.Length);
			Players[0] = new OsianPlayer() { Direction = 1 };
			Players[1] = new OsianPlayer() { Direction = -1 };
		}

		public int Run() {
			StartGameHelper(0, 1);
			StartGameHelper(1, 0);
			for (int i = 0; ; i++) {
				Console.WriteLine("Turn " + i);
				var tr = Turn(i);
				if (tr is int x) {
					ReplayOrganizer.AddMessage(x == 0 ? "Left Side Wins!" : x == 1 ? "Right Side Wins!" : "Tie.");
					return x;
				}
			}
		}

		public int? Turn(int turnnum) {
			foreach (var player in Players) {
				player.Cost += 3;
			}

			ReplayRecorder recorder = new ReplayRecorder();
			recorder.RecordStartFrame(this);
			var x = PlayerMoveHelper(0, 1);
			if (x is int propagResult)
				return propagResult;
			x = PlayerMoveHelper(1, 0);
			if (x is int propagResult2)
				return propagResult2;
			recorder.RecordEndFrame(this);
			ReplayOrganizer.AddFrame(recorder, true);

			MeleeAct();
			RangedAct();
			MapAct();

			if (Players[0].Base <= 0 && Players[1].Base > 0)
				return 1;
			if (Players[1].Base <= 0 && Players[0].Base > 0)
				return 0;
			if (Players[1].Base <= 0 && Players[0].Base <= 0)
				return -1;

			if (turnnum >= 200) {
				if (Players[0].Base > Players[1].Base)
					return 0;
				else if (Players[0].Base < Players[1].Base)
					return 1;
				else
					return -1;
			}
			return null;
		}

		public void MeleeAct() {
			ReplayRecorder recorder = new ReplayRecorder();
			recorder.RecordStartFrame(this);
			foreach (var item in Map.Army.ToArray()) {
				if (item.HP <= 0)
					continue;
				if (item is MeleeArmyBase melee)
					melee.MeleeMove(Map);
			}
			recorder.RecordEndFrame(this);
			ReplayOrganizer.AddFrame(recorder);
		}

		public void RangedAct() {
			ReplayRecorder recorder = new ReplayRecorder();
			recorder.RecordStartFrame(this);
			foreach (var item in Map.Army.ToArray()) {
				if (item.HP <= 0)
					continue;
				if (item is RangedArmyBase ranged)
					ranged.RangedMove(Map);
			}
			recorder.RecordEndFrame(this);
			ReplayOrganizer.AddFrame(recorder);
			ReplayRecorder recorder2 = new ReplayRecorder();
			recorder2.RecordStartFrame(this, true);
			foreach (var item in Map.Army.ToArray()) {
				if (item is RangedArmyBase ranged)
					ranged.RangedAttack(Map);
			}
			recorder2.RecordEndFrame(this);
			ReplayOrganizer.AddFrame(recorder2);
		}

		public void MapAct() {
			ReplayRecorder recorder = new ReplayRecorder();
			recorder.RecordStartFrame(this);
			foreach (var item in Map.Army) {
				item.BreakMove(Map);
				if (item.PosY == Players[0].BasePosY && item.Owner == Players[1])
					Players[0].Base -= item.BreakingPower;
				if (item.PosY == Players[1].BasePosY && item.Owner == Players[0])
					Players[1].Base -= item.BreakingPower;
			}
			recorder.RecordEndFrame(this);
			ReplayOrganizer.AddFrame(recorder);
		}

		protected int? PlayerMoveHelper(int thiz, int oppo) {
			try {
				if (!Task.WaitAll(new[] { PlayerMove(Players[thiz], Players[oppo], OperationProviders[thiz]) }, OperationProviders[thiz].BasicTimeout)) {
					ReplayOrganizer.AddMessage("Player " + thiz + " Timeout.");
					Console.WriteLine(thiz + "TLE");
					return oppo;
				}
			} catch (AggregateException e) {
				ReplayOrganizer.AddMessage("Player " + thiz + " Fails the operation: " + e.InnerException.Message);
				Console.WriteLine(e.ToString());
				return oppo;
			} catch (Exception) {
				return -1;
			}
			return null;
		}

		protected int? StartGameHelper(int thiz, int oppo) {
			try {
				if (!Task.WaitAll(new[] { StartGame(OperationProviders[thiz]) }, OperationProviders[thiz].BasicTimeout * 5))
					return oppo;
			} catch (AggregateException e) {
				Console.WriteLine(e.ToString());
				return oppo;
			} catch (Exception) {
				return -1;
			}
			return null;
		}

		public async Task StartGame(OsianOperationProvider operationProvider) {
			var op = await operationProvider.NextOperation();
			if (!(op is EndTurnOperation))
				throw new InvalidGameOperationException("Launch! expected at game start.");
		}

		public async Task PlayerMove(OsianPlayer player, OsianPlayer opponent, OsianOperationProvider operationProvider) {
			await operationProvider.UpdateGameData(player, opponent, Map);
			while (true) {
				var op = await operationProvider.NextOperation();
				if (op is EndTurnOperation)
					break;
				op.Invoke(player, Map);
			}
		}

		#region IDisposable Support
		private bool disposedValue = false;

		protected virtual void Dispose(bool disposing) {
			if (!disposedValue) {
				if (disposing) {
					foreach (IDisposable provider in OperationProviders) {
						provider.Dispose();
					}
				}
				disposedValue = true;
			}
		}

		void IDisposable.Dispose() {
			Dispose(true);
		}
		#endregion
	}
}
