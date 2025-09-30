
namespace GameLogic
{
	public interface IAttributesHandler
	{
		WarriorAttributes CreateRandomAttributes();
		void GiveWinnerAttributesFromLosser(Warrior winner, Warrior looser);
		void UpdateWarriorAttributesForNextRound(Warrior warrior);
	}
}
