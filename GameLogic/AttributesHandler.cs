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
			warrior.Age = _randomizer.Next(minAge, randomMaxAge);
			warrior.Health = _randomizer.Next(minAttribute, randomMaxAttribute);
			warrior.Strength = _randomizer.Next(minAttribute, randomMaxAttribute);
		}

		public void UpdateWarriorAttributesForNextRound(Warrior warrior)
		{
			warrior.Age++;
			warrior.Health++;
			warrior.Strength++;
		}

		public void GiveWinnerAttributesFromLosser(Warrior winner, Warrior looser)
		{
			winner.Health += 2;
			winner.Strength += looser.Strength;
			looser.Strength = 0;
		}
	}
}
