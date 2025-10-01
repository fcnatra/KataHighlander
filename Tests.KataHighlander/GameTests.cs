using FakeItEasy;
using System.Drawing;
using System.Linq;

#nullable enable

namespace Tests.KataHighlander
{
	public class GameTests
	{
		private World _world;
		private Game _game;
        private AttributesHandler _attributor;
        private RelocationEngine _relocationEngine;

		public GameTests()
		{
			int xLimit = 10;
			int yLimit = 10;
			_world = new World(xLimit, yLimit);
			_game = new Game(_world);

			_attributor = new AttributesHandler();
			_relocationEngine = new RelocationEngine();
		}

		[Fact]
		public void WhenGameStarts_ThereAre_2WarriorsInTheWorld()
		{
			// ACT
			_game.Warriors.Add(new Warrior { Id = 1, Name = "Warrior 1", Location = new Point(1, 1) });
			_game.Warriors.Add(new Warrior { Id = 2, Name = "Warrior 2", Location = new Point(2, 2) });

			// ASSERT
			var expectedNumberOfWarriors = 2;
			Assert.Equal(expectedNumberOfWarriors, _game.Warriors.Count());
		}

		[Fact]
		public void WhenDefaultGameStarts_EachWarriorIsIn_DifferentLocation()
		{
			_game.Relocator = _relocationEngine;
			_game.Attributor = _attributor;

			// ACT
			_game.CreateDefaultWarriors();

			// ASSERT
			var warriors = _game.Warriors;
			var locations = warriors.Select(w => w.Location);
			Assert.Equal(locations.Count(), locations.Distinct().Count());
		}

		[Fact]
		public void WhenASecondDefaultGameStarts_WarriorsLocationsAre_DifferentThanInFirst()
		{
			_game.Attributor = _attributor;
			_game.Relocator = _relocationEngine;

			var secondGame = new Game(_world);
			secondGame.Attributor = _attributor;
			secondGame.Relocator = _relocationEngine;

			// ACT
			_game.CreateDefaultWarriors();
			secondGame.CreateDefaultWarriors();

			// ASSERT
			var warriors1 = _game.Warriors;
			var locations1 = warriors1.Select(w => w.Location);

			var warriors2 = secondGame.Warriors;
			var locations2 = warriors2.Select(w => w.Location);

			Assert.NotEqual(locations1, locations2);
		}

		[Fact]
		public void Warriors_OutsideLimits_AreRelocatedInside()
		{
			var world = new World(5, 5);
			var game = new Game(world);

			game.Attributor = A.Fake<IAttributesHandler>();
			game.Relocator = new RelocationEngine();

			game.Warriors.Add(new Warrior { Id = 1, Name = "Warrior 1", Location = new Point(1, 1) });
			game.Warriors.Add(new Warrior { Id = 2, Name = "Warrior 2", Location = new Point(20, 20) });

			// ACT
			game.NextRound();

			// ASSERT
			var warriors = game.Warriors;
			var locations = warriors.Select(w => w.Location);
			var xCoords = locations.Select(x => x.X);
			var yCoords = locations.Select(y => y.Y);

			Assert.True(xCoords.Min() >= 0);
			Assert.True(xCoords.Max() <= world.XLimit);
			Assert.True(yCoords.Min() >= 0);
			Assert.True(yCoords.Max() <= world.YLimit);
		}

		[Fact]
		public void GivenNextRound_AllWarriors_Move()
		{
			// ARRANGE
            World world = new(10, 10);
            var game = new Game(world);
			game.Attributor = A.Fake<IAttributesHandler>();
			game.Relocator = new RelocationEngine();
			game.CreateDefaultWarriors();
			var warriors = game.Warriors;
			var locations = warriors.Select(w => w.Location).ToList();

			// ACT
			game.NextRound();

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

			_game.CreateDefaultWarriors();
			var warriors = _game.Warriors;
			var ages = warriors.Select(w => w.Attributes.Age + 1).ToList();
			var healths = warriors.Select(w => w.Attributes.Health + 1).ToList();
			var strengths = warriors.Select(w => w.Attributes.Strength + 1).ToList();

			// ACT
			_game.NextRound();

			// ASSERT
			var agesNextRound = warriors.Select(w => w.Attributes.Age);
			var healthsNextRound = warriors.Select(w => w.Attributes.Health);
			var strengthsNextRound = warriors.Select(w => w.Attributes.Strength);
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
			_game.Attributor = new AttributesHandler();
			_game.CreateDefaultWarriors();
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
			_game.CreateDefaultWarriors();

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
			_game.CreateDefaultWarriors();

			// ASSERT
			   Assert.DoesNotContain(_game.Warriors, w => w.Attributes.Age == 0 || w.Attributes.Health == 0 || w.Attributes.Strength == 0);
		}

		[Fact]
		public void GivenTwoWarriors_InSameLocation_WarriorsFight()
		{
			// ARRANGE
			int warrior1Id = 1;
			int warrior2Id = 2;
			IRelocator fakeRelocator = FakeRelocator_ThatPutsTogether_Warriors_ById(warrior1Id, warrior2Id);
			IFightEngine fakeFightEngine = A.Fake<IFightEngine>();

			_game.Attributor = new AttributesHandler();
			_game.Relocator = fakeRelocator;
			_game.BattleField = fakeFightEngine;
			_game.CreateDefaultWarriors();

			Warrior warrior1 = _game.Warriors.First(w => w.Id == warrior1Id);
			Warrior warrior2 = _game.Warriors.First(w => w.Id == warrior2Id);

			// ACT
			_game.NextRound();

			// ASSERT
			A.CallTo(() => fakeFightEngine.FightAndGetWinner(warrior1, warrior2)).MustHaveHappenedOnceExactly();
		}

		[Theory]
		[InlineData(63,23,1)]
		[InlineData(62,63,2)]
		public void GivenAFight_StrongerWarrior_WinsToTheWeaker(int attributes1, int attributes2, int winnerId)
		{
			// ARRANGE
			var warrior1 = new Warrior
			{
				Id = 1,
				Location = new Point(3, 3),
				Attributes = new WarriorAttributes { Health = attributes1/10, Strength = attributes1%10 }
			};

			var warrior2 = new Warrior
			{
				Id = 2,
				Location = new Point(3, 3),
				Attributes = new WarriorAttributes { Health = attributes2/10, Strength = attributes2%10 }
			};

			IRelocator fakeRelocator = A.Fake<IRelocator>();
			A.CallTo(() => fakeRelocator.RelocateWarrior(A<IGameState>._, A<Warrior>._, A<Point>._))
				.ReturnsLazily((IGameState game, Warrior warrior, Point offset) =>
				{
					warrior.Location = new Point(4, 4);
					return warrior.Location;
				}); ;

			_game.BattleField = new FightEngine();
			_game.BattleField.Attributor = _attributor;
			_game.Relocator = fakeRelocator;
			_game.Attributor = _attributor;

			_game.Warriors.Add(warrior1);
			_game.Warriors.Add(warrior2);

			// ACT
			_game.NextRound();

			// ASSERT
			Assert.Contains(_game.Warriors, x => x.Id == winnerId);
			Assert.DoesNotContain(_game.Warriors, x => x.Id != winnerId);
		}

		[Fact]
		public void GivenAFight_BetweenWarriors1_and_2_WorldRegistersAFight_InThatLocation()
		{
			// ARRANGE
			var warrior1Desired = new Warrior { Id = 1, Attributes = new WarriorAttributes { Health = 6, Strength = 2 } };
			var warrior2Desired = new Warrior { Id = 2, Attributes = new WarriorAttributes { Health = 1, Strength = 1 } };

			IRelocator fakeRelocator = FakeRelocator_ThatPutsTogether_Warriors_ById(warrior1Desired.Id, warrior2Desired.Id);
			IAttributesHandler fakeAttributor = new AttributesHandler();

			_game.BattleField = new FightEngine();
			_game.BattleField.Attributor = fakeAttributor;
			_game.Relocator = fakeRelocator;
			_game.Attributor = fakeAttributor;
			_game.CreateDefaultWarriors();

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
			_game.CreateDefaultWarriors();

			   int w1Strength = _game.Warriors.First(w => w.Id == warrior1Id).Attributes.Strength;
			   int w2Strength = _game.Warriors.First(w => w.Id == warrior2Id).Attributes.Strength;

			// ACT
			_game.NextRound();

			// ASSERT
			Assert.Equal(w1Strength + w2Strength + 1, winner?.Attributes.Strength);
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
			_game.CreateDefaultWarriors();

			   int w1Health = _game.Warriors.First(w => w.Id == warrior1Id).Attributes.Health;
			   int w2Health = _game.Warriors.First(w => w.Id == warrior2Id).Attributes.Health;

			// ACT
			_game.NextRound();

			// ASSERT
			   Assert.Equal((winner?.Id == warrior1Id ? w1Health : w2Health) + 3, winner?.Attributes.Health);
		}

		private static IAttributesHandler FakeAttributor_ThatAssignAttributes_ToWarriorsByName(Warrior w1ToAssign, Warrior w2ToAssign)
		{
			   IAttributesHandler fakeAttributor = A.Fake<IAttributesHandler>();
			   A.CallTo(() => fakeAttributor.CreateRandomAttributes())
				   .WhenArgumentsMatch((Warrior warrior) => warrior.Id == w1ToAssign.Id || warrior.Id == w2ToAssign.Id)
				   .Invokes((Warrior warrior) =>
				   {
					   if (warrior.Id == w1ToAssign.Id)
					   {
						   warrior.Attributes.Health = w1ToAssign.Attributes.Health;
						   warrior.Attributes.Strength = w1ToAssign.Attributes.Strength;
					   }
					   else
					   {
						   warrior.Attributes.Health = w2ToAssign.Attributes.Health;
						   warrior.Attributes.Strength = w2ToAssign.Attributes.Strength;
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