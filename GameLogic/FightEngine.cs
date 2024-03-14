namespace GameLogic
{
	public class FightEngine : IFightEngine
	{
		public IAttributesHandler? Attributor { get; set; }

		private Random _randomizer;
		private record WarriorRank(Warrior Warrior, double Rank) { public double Rank = Rank; }

		public FightEngine()
		{
			_randomizer = new Random(this.GetHashCode() + DateTime.Now.Microsecond);
		}

		public Warrior? FightAndGetWinner(Warrior warrior1, Warrior warrior2)
		{
			Warrior? winner = null;
			WarriorRank rank1 = CalcRank(warrior1);
			WarriorRank rank2 = CalcRank(warrior2);

			(rank1, rank2) = AssignLuckyBonus(rank1, rank2);

			if (rank1.Rank > rank2.Rank)
			{
				Attributor?.GiveWinnerAttributesFromLosser(warrior1, warrior2);
				winner = rank1.Warrior;
			}
			else if (rank2.Rank > rank1.Rank)
			{
				Attributor?.GiveWinnerAttributesFromLosser(warrior2, warrior1);
				winner = rank2.Warrior;
			}

			return winner;
		}

		private (WarriorRank, WarriorRank) AssignLuckyBonus(WarriorRank rank1, WarriorRank rank2)
		{
			var luckyBonus = _randomizer.Next(1, 2);
			if (luckyBonus == 1) rank1.Rank += LuckyBonus(rank1);
			else rank2.Rank += LuckyBonus(rank2);

			return (rank1, rank2);
		}

		private double LuckyBonus(WarriorRank rank) => rank.Rank * 0.1;

		private WarriorRank CalcRank(Warrior warrior) => new WarriorRank(warrior, (warrior.Health / (double)3) + warrior.Strength);
	}
}