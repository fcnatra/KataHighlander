using System.Drawing;

namespace GameLogic
{
	// Interface for relocating warriors on the game board
	public interface IRelocator
	{
		Point GetEmptyRandomLocation(IGameState game);
		Point RelocateWarrior(IGameState game, Warrior warrior, Point OffsetDiameter);
	}
}
