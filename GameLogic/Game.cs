using System.Drawing;

namespace GameLogic
{
	public class Game : IGameState
	{
		private const int NUMBER_OF_WARRIORS = 12;
		private readonly Point MOVE_OFFSET = new Point(1, 1);
		private List<Warrior> _warriors;
		private Random _randomizer;

		public List<Warrior> Warriors { get { return new List<Warrior>(_warriors); } }
		public World Gameboard { get ; set; }
		public IRelocator? Relocator { get; set; }
		public IFightEngine? BattleField { get; set; }
		public IAttributesHandler? Attributor { get; set; }

		public Game(World world)
		{
			_randomizer = new Random(this.GetHashCode() + DateTime.Now.Microsecond);
			Gameboard = world;
			_warriors = [];
		}

		public void Start()
		{
			CreateWarriors();
		}

		private void CreateWarriors()
		{
			for (int warriorId = 0; warriorId < NUMBER_OF_WARRIORS; warriorId++)
			{
				var warrior = new Warrior { Id = warriorId, Name = GetNameForWarrior(warriorId) };

				warrior.Location = Relocator?.GetEmptyRandomLocation(this) ?? Point.Empty;

				Attributor?.AssignRandomAttributesToWarrior(warrior);

				_warriors.Add(warrior);
			}
		}

		private string GetNameForWarrior(int warriorId)
		{
			switch (warriorId)
			{
				case 0: return "Connor MacLeod";
				case 1: return "Ramírez";
				case 2: return "Duncan MacLeod";
				case 3: return "The Kurgan";
				case 4: return "Kronos";
				case 5: return "Timothy of Gilliam";
				case 6: return "Danny O'Donal";
				case 7: return "Methos";
				case 8: return "Mako";
				case 9: return "Richie Ryan";
				case 10: return "Slan Quince";
				case 11: return "Kiem Sun";
				case 12: return "Felicia Martins";
				default: return string.Empty;
			}
		}

		public void NextRound()
		{
			_warriors.ForEach((w) => Relocator?.RelocateWarrior(this, w, MOVE_OFFSET));

			RunFights();

			_warriors.ForEach((w) => Attributor?.UpdateWarriorAttributesForNextRound(w));
		}

		private void RunFights()
		{
			var groupsOfTwo = _warriors.GroupBy(warrior => warrior.Location).Where(group => group.Count() == 2);

			foreach (var group in groupsOfTwo)
			{
				var warrior1 = group.First();
				var warrior2 = group.Last();

				Gameboard.FightLocations.Clear();
				Gameboard.FightLocations.Add(warrior1.Location);

				var winner = BattleField?.FightAndGetWinner(warrior1, warrior2);

				if (winner == warrior1) RemoveLooser(warrior2);
				if (winner == warrior2) RemoveLooser(warrior1);
			};
		}

		private void RemoveLooser(Warrior warrior) => _warriors.Remove(warrior);
	}
}