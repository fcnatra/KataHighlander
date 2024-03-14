namespace GameLogic
{
	public interface IFightEngine
	{
		IAttributesHandler? Attributor { get; set; }
		Warrior? FightAndGetWinner(Warrior warrior1, Warrior warrior2);
	}
}