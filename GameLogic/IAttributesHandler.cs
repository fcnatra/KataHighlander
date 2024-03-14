
namespace GameLogic
{
	public interface IAttributesHandler
	{
		void AssignRandomAttributesToWarrior(Warrior warrior);
		void GiveWinnerAttributesFromLosser(Warrior winner, Warrior looser);
		void UpdateWarriorAttributesForNextRound(Warrior warrior);
	}
}
