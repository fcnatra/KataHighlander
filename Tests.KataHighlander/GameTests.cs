using FakeItEasy;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

#nullable enable

namespace Tests.KataHighlander
{
	public class GameTests
	{
		World _world;
		Game _game;
		RelocationEngine _relocationEngine;

		public GameTests()
		{
			int xLimit = 10;
			int yLimit = 10;
			_world = new World(xLimit, yLimit);

			_game = new Game(_world);

			_relocationEngine = new RelocationEngine();
		}

		[Fact]
		public void WhenGameStarts_ThereAre_12WarriorsInTheWorld()
		{
			// ACT
			_game.Start();

			// ASSERT
			var expectedNumberOfWarriors = 12;
			Assert.Equal(expectedNumberOfWarriors, _game.Warriors.Count());
		}

		[Fact]
		public void WhenGameStarts_EachWarriorIsIn_DifferentLocation()
		{
			_game.Relocator = new RelocationEngine();

			// ACT
			_game.Start();

			// ASSERT
			var warriors = _game.Warriors;
			var locations = warriors.Select(w => w.Location);
			Assert.Equal(locations.Count(), locations.Distinct().Count());
		}

		[Fact]
		public void WhenASecondGameStarts_WarriorsLocationsAre_DifferentThanInFirst()
		{
			var secondGame = new Game(_world);
			_game.Relocator = new RelocationEngine();
			secondGame.Relocator = new RelocationEngine();

			// ACT
			_game.Start();
			secondGame.Start();

			// ASSERT
			var warriors1 = _game.Warriors;
			var locations1 = warriors1.Select(w => w.Location);

			var warriors2 = secondGame.Warriors;
			var locations2 = warriors2.Select(w => w.Location);

			Assert.NotEqual(locations1, locations2);
		}

		[Fact]
		public void WhenGameStarts_AllWarriorsAre_InsideWorldLimits()
		{

			_game.Relocator = new RelocationEngine();

			// ACT
			_game.Start();

			// ASSERT
			var warriors = _game.Warriors;
			var locations = warriors.Select(w => w.Location);
			var xCoords = locations.Select(x => x.X);
			var yCoords = locations.Select(y => y.Y);

			Assert.True(xCoords.Min() >= 0);
			Assert.True(xCoords.Max() <= _world.XLimit);
			Assert.True(yCoords.Min() >= 0);
			Assert.True(yCoords.Max() <= _world.YLimit);
		}

		[Fact]
		public void GivenNextRound_AllWarriors_Move()
		{
			// ARRANGE
			_game.Relocator = new RelocationEngine();
			_game.Start();
			var warriors = _game.Warriors;
			var locations = warriors.Select(w => w.Location).ToList();

			// ACT
			_game.NextRound();

			// ASSERT
			var locationsNextRound = warriors.Select(w => w.Location);
			Assert.NotEqual(locations, locationsNextRound);
		}

		[Fact]
		public void GivenNextRound_WarriorsAttributesAreIncremented_ByOne()
		{
			// ARRANGE
			_game.Relocator = new RelocationEngine();
			_game.Attributor = new AttributesHandler();

			_game.Start();
			var warriors = _game.Warriors;
			var ages = warriors.Select(w => w.Age + 1).ToList();
			var healths = warriors.Select(w => w.Health + 1).ToList();
			var strengths = warriors.Select(w => w.Strength + 1).ToList();

			// ACT
			_game.NextRound();

			// ASSERT
			var agesNextRound = warriors.Select(w => w.Age);
			var healthsNextRound = warriors.Select(w => w.Health);
			var strengthsNextRound = warriors.Select(w => w.Strength);
			Assert.Equal(ages, agesNextRound);
			Assert.Equal(healths, healthsNextRound);
			Assert.Equal(strengths, strengthsNextRound);
		}

		[Fact]
		public void GivenNextRound_AllWarriors_ContinueInsideTheWorldLimits()
		{
			// ARRANGE
			var world = new World(4, 3);
			_game = new Game(world);
			_game.Relocator = new RelocationEngine();
			_game.Start();
			var warriors = _game.Warriors;

			// ACT
			_game.NextRound();

			// ASSERT
			var locations = warriors.Select(w => w.Location);
			var xCoords = locations.Select(x => x.X);
			var yCoords = locations.Select(y => y.Y);

			Assert.True(xCoords.Min() >= 0);
			Assert.True(xCoords.Max() <= _world.XLimit);
			Assert.True(yCoords.Min() >= 0);
			Assert.True(yCoords.Max() <= _world.YLimit);
		}

		[Fact]
		public void MaxWarriorsInSameLocation_IsTwo()
		{
			// ARRANGE
			var world = new World(4, 3);
			_game = new Game(world);
			_game.Relocator = new RelocationEngine();
			_game.Attributor = new AttributesHandler();
			_game.Start();

			// ACT
			_game.NextRound();

			// ASSERT
			var warriors = _game.Warriors;
			var groupsOfWarriors = warriors.GroupBy(warrior => warrior.Location);
			var largestGroup = groupsOfWarriors.OrderByDescending(group => group.Count()).First();
			Assert.Equal(2, largestGroup.Count());
		}

		[Fact]
		public void AllWarriors_Have_AttributesGreaterThanZero()
		{
			// ARRANGE
			_game.Attributor = new AttributesHandler();

			// ACT
			_game.Start();

			// ASSERT
			Assert.DoesNotContain(_game.Warriors, w => w.Age == 0 || w.Health == 0 || w.Strength == 0);
		}

		[Fact]
		public void GivenTwoWarriors_InSameLocation_WarriorsFight()
		{
			// ARRANGE
			int warrior1Id = 1;
			int warrior2Id = 2;
			IRelocator fakeRelocator = FakeRelocator_ThatPutsTogether_Warriors_ById(warrior1Id, warrior2Id);
			IFightEngine fakeFightEngine = A.Fake<IFightEngine>();

			_game.Relocator = fakeRelocator;
			_game.BattleField = fakeFightEngine;
			_game.Start();

			Warrior warrior1 = _game.Warriors.First(w => w.Id == warrior1Id);
			Warrior warrior2 = _game.Warriors.First(w => w.Id == warrior2Id);

			// ACT
			_game.NextRound();

			// ASSERT
			A.CallTo(() => fakeFightEngine.FightAndGetWinner(warrior1, warrior2)).MustHaveHappenedOnceExactly();
		}

		[Fact]
		public void GivenAFight_BetweenWarriors6_3_and_2_3_Warrior63_Wins()
		{
			// ARRANGE
			var warrior1Desired = new Warrior { Id = 1, Health = 6, Strength = 3 };
			var warrior2Desired = new Warrior { Id = 2, Health = 2, Strength = 3 };

			IRelocator fakeRelocator = FakeRelocator_ThatPutsTogether_Warriors_ById(warrior1Desired.Id, warrior2Desired.Id);
			IAttributesHandler fakeAttributor = FakeAttributor_ThatAssignAttributes_ToWarriorsByName(warrior1Desired, warrior2Desired);

			_game.BattleField = new FightEngine();
			_game.BattleField.Attributor = fakeAttributor;
			_game.Relocator = fakeRelocator;
			_game.Attributor = fakeAttributor;

			// ACT
			_game.Start();
			_game.NextRound();

			// ASSERT
			Assert.DoesNotContain(_game.Warriors, x => x.Id == warrior2Desired.Id);
		}

		[Fact]
		public void GivenAFight_BetweenWarriors6_2_and_6_3_Warrior63_Wins()
		{
			// ARRANGE
			var warrior1Desired = new Warrior { Id = 1, Health = 6, Strength = 2 };
			var warrior2Desired = new Warrior { Id = 2, Health = 6, Strength = 3 };

			IRelocator fakeRelocator = FakeRelocator_ThatPutsTogether_Warriors_ById(warrior1Desired.Id, warrior2Desired.Id);
			IAttributesHandler fakeAttributor = FakeAttributor_ThatAssignAttributes_ToWarriorsByName(warrior1Desired, warrior2Desired);

			_game.BattleField = new FightEngine();
			_game.BattleField.Attributor = fakeAttributor;
			_game.Relocator = fakeRelocator;
			_game.Attributor = fakeAttributor;
			_game.Start();

			// ACT
			_game.NextRound();

			// ASSERT
			Assert.DoesNotContain(_game.Warriors, x => x.Id == warrior1Desired.Id);
		}

		[Fact]
		public void GivenAFight_BetweenWarriors1_and_2_WorldRegistersAFight_InThatLocation()
		{
			// ARRANGE
			var warrior1Desired = new Warrior { Id = 1, Health = 6, Strength = 2 };
			var warrior2Desired = new Warrior { Id = 2, Health = 1, Strength = 1 };

			IRelocator fakeRelocator = FakeRelocator_ThatPutsTogether_Warriors_ById(warrior1Desired.Id, warrior2Desired.Id);
			IAttributesHandler fakeAttributor = FakeAttributor_ThatAssignAttributes_ToWarriorsByName(warrior1Desired, warrior2Desired);

			_game.BattleField = new FightEngine();
			_game.BattleField.Attributor = fakeAttributor;
			_game.Relocator = fakeRelocator;
			_game.Attributor = fakeAttributor;
			_game.Start();

			// ACT
			_game.NextRound();

			// ASSERT
			var location = _game.Warriors.First(w => w.Id == warrior1Desired.Id).Location;
			Assert.Contains(location, _game.Gameboard.FightLocations);
		}

		[Fact]
		public void GivenAFight_WinnerTakesStrengthFromLooser()
		{
			// ARRANGE
			Warrior? winner = null;

			var warrior1Id = 1;
			var warrior2Id = 2;
			IRelocator fakeRelocator = FakeRelocator_ThatPutsTogether_Warriors_ById(warrior1Id, warrior2Id);

			IFightEngine fakeBattleField = A.Fake<IFightEngine>();
			A.CallTo(() => fakeBattleField.FightAndGetWinner(A<Warrior>._, A<Warrior>._))
				.WhenArgumentsMatch((Warrior w1, Warrior w2) => w1.Id == warrior1Id && w2.Id == warrior2Id)
				.Invokes((Warrior w1, Warrior w2) =>
				{
					var batterField = new FightEngine { Attributor = new AttributesHandler() };
					winner = batterField.FightAndGetWinner(w1, w2);
				});

			_game.Relocator = fakeRelocator;
			_game.Attributor = new AttributesHandler();
			_game.BattleField = fakeBattleField;
			_game.Start();

			int w1Strength = _game.Warriors.First(w => w.Id == warrior1Id).Strength;
			int w2Strength = _game.Warriors.First(w => w.Id == warrior2Id).Strength;

			// ACT
			_game.NextRound();

			// ASSERT
			Assert.Equal(w1Strength + w2Strength + 1, winner?.Strength);
		}

		[Fact]
		public void GivenAFight_WinnerIncrementsItsHealthOnTwoPoints()
		{
			// ARRANGE
			Warrior? winner = null;

			var warrior1Id = 1;
			var warrior2Id = 2;
			IRelocator fakeRelocator = FakeRelocator_ThatPutsTogether_Warriors_ById(warrior1Id, warrior2Id);

			IFightEngine fakeBattleField = A.Fake<IFightEngine>();
			A.CallTo(() => fakeBattleField.FightAndGetWinner(A<Warrior>._, A<Warrior>._))
				.WhenArgumentsMatch((Warrior w1, Warrior w2) => w1.Id == warrior1Id && w2.Id == warrior2Id)
				.Invokes((Warrior w1, Warrior w2) =>
				{
					var batterField = new FightEngine { Attributor = new AttributesHandler() };
					winner = batterField.FightAndGetWinner(w1, w2);
				});

			_game.Relocator = fakeRelocator;
			_game.Attributor = new AttributesHandler();
			_game.BattleField = fakeBattleField;
			_game.Start();

			int w1Health = _game.Warriors.First(w => w.Id == warrior1Id).Health;
			int w2Health = _game.Warriors.First(w => w.Id == warrior2Id).Health;

			// ACT
			_game.NextRound();

			// ASSERT
			Assert.Equal((winner?.Id == warrior1Id ? w1Health : w2Health) + 3, winner?.Health);
		}

		private static IAttributesHandler FakeAttributor_ThatAssignAttributes_ToWarriorsByName(Warrior w1ToAssign, Warrior w2ToAssign)
		{
			IAttributesHandler fakeAttributor = A.Fake<IAttributesHandler>();
			A.CallTo(() => fakeAttributor.AssignRandomAttributesToWarrior(A<Warrior>._))
				.WhenArgumentsMatch((Warrior warrior) => warrior.Id == w1ToAssign.Id || warrior.Id == w2ToAssign.Id)
				.Invokes((Warrior warrior) =>
				{
					if (warrior.Id == w1ToAssign.Id)
					{
						warrior.Health = w1ToAssign.Health; warrior.Strength = w1ToAssign.Strength;
					}
					else
					{
						warrior.Health = w2ToAssign.Health; warrior.Strength = w2ToAssign.Strength;
					}
				});
			A.CallTo(() => fakeAttributor.GiveWinnerAttributesFromLosser(A<Warrior>._, A<Warrior>._))
				.Invokes((Warrior winner, Warrior looser) => { new AttributesHandler().GiveWinnerAttributesFromLosser(winner, looser); });

			return fakeAttributor;
		}

		private IRelocator FakeRelocator_ThatPutsTogether_Warriors_ById(int warrior1Id, int warrior2Id)
		{
			IRelocator fakeRelocator = A.Fake<IRelocator>();
			A.CallTo(() => fakeRelocator.RelocateWarrior(A<Game>._, A<Warrior>._, A<Point>._))
				.WhenArgumentsMatch((IGameState game, Warrior warrior, Point offset) => warrior.Id == warrior2Id)
				.Invokes((IGameState game, Warrior warrior, Point offset) =>
				{
					var anotherWarrior = game.Warriors.First(w => w.Id == warrior1Id);
					warrior.Location = anotherWarrior.Location;
				});
			A.CallTo(() => fakeRelocator.GetEmptyRandomLocation(A<IGameState>._))
				.ReturnsLazily((call) => _relocationEngine.GetEmptyRandomLocation(_game));

			return fakeRelocator;
		}

	}
}