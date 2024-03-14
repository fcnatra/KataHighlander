using System.Drawing;

namespace GameLogic
{
	public interface IRelocator
	{
		Point GetEmptyRandomLocation(IGameState game);
		Point RelocateWarrior(IGameState game, Warrior warrior, Point OffsetDiameter);
	}
}
