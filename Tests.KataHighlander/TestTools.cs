using FakeItEasy;
using System.Drawing;

#nullable enable

namespace Tests.KataHighlander
{
	public partial class GameTests
	{
        // Add this new class (in a separate file or at the bottom of this file if needed)
        internal static class TestTools
	{
		internal static IRelocator FakeRelocator_ThatPutsWarriorsTogether()
		{
			IRelocator fakeRelocator = A.Fake<IRelocator>();
			A.CallTo(() => fakeRelocator.RelocateWarrior(A<IGameState>._, A<Warrior>._, A<Point>._))
				.ReturnsLazily((IGameState game, Warrior warrior, Point offset) =>
				{
					warrior.Location = new Point(4, 4);
					return warrior.Location;
				});
			return fakeRelocator;
		}

		internal static (Warrior, Warrior) GimmeTwoWarriors()
		{
			var warrior1 = new Warrior
			{
				Id = 1,
				Location = new Point(3, 3),
				Attributes = new WarriorAttributes { Health = 6, Strength = 3 }
			};

			var warrior2 = new Warrior
			{
				Id = 2,
				Location = new Point(3, 3),
				Attributes = new WarriorAttributes { Health = 1, Strength = 1 }
			};
			return (warrior1, warrior2);
		}
	}
	}
}