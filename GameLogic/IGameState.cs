namespace GameLogic
{
	public interface IGameState
	{
		List<Warrior> Warriors { get; }
		World Gameboard { get; set; }
	}
}
