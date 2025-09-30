namespace GameLogic
{
	public class AttributesHandler : IAttributesHandler
	{
		private Random _randomizer;
		private const int minAge = 25;
		private const int randomMaxAge = 2000;
		private const int minAttribute = 1;
		private const int randomMaxAttribute = 200;

		public AttributesHandler()
        {
            _randomizer = new Random(this.GetHashCode() + DateTime.Now.Microsecond);
        }

        public void AssignRandomAttributesToWarrior(Warrior warrior)
		{
			var attr = warrior.Attributes;

			attr.Age = _randomizer.Next(minAge, randomMaxAge);
			attr.Health = _randomizer.Next(minAttribute, randomMaxAttribute);
			attr.Strength = _randomizer.Next(minAttribute, randomMaxAttribute);
		}

		public void UpdateWarriorAttributesForNextRound(Warrior warrior)
		{
			var attr = warrior.Attributes;

			attr.Age++;
			attr.Health++;
			attr.Strength++;
		}

		public void GiveWinnerAttributesFromLosser(Warrior winner, Warrior looser)
		{
			var winnerAttr = winner.Attributes;
			var looserAttr = looser.Attributes;

			winnerAttr.Health += 2;
			winnerAttr.Strength += looserAttr.Strength;
			looserAttr.Strength = 0;
		}
	}
}
