using System.Drawing;

namespace GameLogic
{
	public class Game : IGameState
	{
		private const int NUMBER_OF_WARRIORS = 12;
		private readonly Point MOVE_OFFSET = new Point(1, 1);
		private List<Warrior> _warriors;
		private Random _randomizer;
		
		private static readonly List<string> WarriorNames = new List<string>
		{
			"Connor MacLeod",
			"Ramírez",
			"Duncan MacLeod",
			"The Kurgan",
			"Kronos",
			"Timothy of Gilliam",
			"Danny O'Donal",
			"Methos",
			"Mako",
			"Richie Ryan",
			"Slan Quince",
			"Kiem Sun",
			"Felicia Martins"
		};

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
			if (Attributor == null)
				throw new InvalidOperationException("Attributor is not set.");

			for (int warriorId = 0; warriorId < NUMBER_OF_WARRIORS; warriorId++)
			{
				var warrior = new Warrior { Id = warriorId, Name = WarriorNames[warriorId] };

				warrior.Location = Relocator?.GetEmptyRandomLocation(this) ?? Point.Empty;

				warrior.Attributes = Attributor.CreateRandomAttributes();

				_warriors.Add(warrior);
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