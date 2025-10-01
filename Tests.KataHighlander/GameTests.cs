using FakeItEasy;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

#nullable enable

namespace Tests.KataHighlander
{
	public partial class GameTests
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
			var (warrior1, warrior2) = TestTools.GimmeTwoWarriors();

			_game.Attributor = A.Fake<IAttributesHandler>();
			_game.Relocator = new RelocationEngine();
			_game.BattleField = new FightEngine { Attributor = _attributor };

			_game.Warriors.Add(warrior1);
			_game.Warriors.Add(warrior2);

			var locations = _game.Warriors.Select(w => w.Location).ToList();

			// ACT
			_game.NextRound();

			// ASSERT
			var locationsNextRound = _game.Warriors.Select(w => w.Location);
			Assert.NotEqual(locations, locationsNextRound);
		}

		[Fact]
		public void GivenNextRound_WarriorsAttributesAreIncremented_ByOne()
		{
			// ARRANGE
			var (warrior1, warrior2) = TestTools.GimmeTwoWarriors();
			warrior1.Location = new Point(1, 1);
			warrior2.Location = new Point(8, 8);// far away

			_game.Relocator = new RelocationEngine();
			_game.BattleField = new FightEngine { Attributor = _attributor };
			_game.Attributor = _attributor;

			_game.Warriors.Add(warrior1);
			_game.Warriors.Add(warrior2);

			var warriors = _game.Warriors;

			// persist the result now to avoid having a different result after the ACT
            List<int> ages = warriors.Select(w => w.Attributes.Age + 1).ToList();
            List<int> healths = warriors.Select(w => w.Attributes.Health + 1).ToList();
			List<int> strengths = warriors.Select(w => w.Attributes.Strength + 1).ToList();

			// ACT
			_game.NextRound();

            // ASSERT
            IEnumerable<int> agesNextRound = warriors.Select(w => w.Attributes.Age);
            IEnumerable<int> healthsNextRound = warriors.Select(w => w.Attributes.Health);
            IEnumerable<int> strengthsNextRound = warriors.Select(w => w.Attributes.Strength);

			Assert.Equal(ages, agesNextRound);
			Assert.Equal(healths, healthsNextRound);
			Assert.Equal(strengths, strengthsNextRound);
		}

		[Fact]
		public void GivenNextRound_AllWarriors_ContinueInsideTheWorldLimits()
		{
			// ARRANGE
			_game.Relocator = new RelocationEngine();
			_game.Attributor = new AttributesHandler();
			_game.BattleField = new FightEngine { Attributor = _attributor };
			
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
			var world = new World(1, 1);
			_game = new Game(world);

			var (warrior1, warrior2) = TestTools.GimmeTwoWarriors();
			var (warrior3, warrior4) = TestTools.GimmeTwoWarriors();

			warrior1.Location = new Point(0, 1);
			warrior2.Location = new Point(1, 1);
			warrior3.Location = new Point(0, 0);
			warrior4.Location = new Point(1, 0);

			_game.BattleField = A.Fake<IFightEngine>();
			_game.Relocator = _relocationEngine;
			_game.Attributor = _attributor;

			_game.Warriors.Add(warrior1);
			_game.Warriors.Add(warrior2);
			_game.Warriors.Add(warrior3);
			_game.Warriors.Add(warrior4);

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
			var (warrior1, warrior2) = TestTools.GimmeTwoWarriors();

			IRelocator fakeRelocator = TestTools.FakeRelocator_ThatPutsWarriorsTogether_OnLocation_4_4();
			IFightEngine fakeFightEngine = A.Fake<IFightEngine>();

			_game.Attributor = new AttributesHandler();
			_game.Relocator = fakeRelocator;
			_game.BattleField = fakeFightEngine;

			_game.Warriors.Add(warrior1);
			_game.Warriors.Add(warrior2);

			// ACT
			_game.NextRound();

			// ASSERT
			A.CallTo(() => fakeFightEngine.FightAndGetWinner(warrior1, warrior2)).MustHaveHappenedOnceExactly();
		}

		[Theory]
		[InlineData(63, 23, 1)]
		[InlineData(62, 63, 2)]
		public void GivenAFight_StrongerWarrior_WinsToTheWeaker(int attributes1, int attributes2, int winnerId)
		{
			// ARRANGE
			var (warrior1, warrior2) = TestTools.GimmeTwoWarriors();

			warrior1.Attributes.Health = attributes1 / 10;
			warrior1.Attributes.Strength = attributes1 % 10;

			warrior2.Attributes.Health = attributes2 / 10;
			warrior2.Attributes.Strength = attributes2 % 10;

			IRelocator fakeRelocator = TestTools.FakeRelocator_ThatPutsWarriorsTogether_OnLocation_4_4();

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
		public void GivenAFight_WorldRegistersAFight_InThatLocation()
		{
			// ARRANGE
			var (warrior1, warrior2) = TestTools.GimmeTwoWarriors();

			IRelocator fakeRelocator = TestTools.FakeRelocator_ThatPutsWarriorsTogether_OnLocation_4_4();

			_game.BattleField = new FightEngine { Attributor = _attributor };
			_game.Relocator = fakeRelocator;
			_game.Attributor = _attributor;

			_game.Warriors.Add(warrior1);
			_game.Warriors.Add(warrior2);

			// ACT
			_game.NextRound();

			// ASSERT
			var location = warrior1.Location;
			Assert.Contains(location, _game.Gameboard.FightLocations);
		}

		[Fact]
		public void GivenAFight_WinnerTakesAttributesFromLoser()
		{
			// ARRANGE
			var (warriorWinner, warriorLooser) = TestTools.GimmeTwoWarriors();

			warriorWinner.Attributes.Health = 6;
			warriorWinner.Attributes.Strength = 3;

			warriorLooser.Attributes.Health = 1;
			warriorLooser.Attributes.Strength = 1;

			IRelocator fakeRelocator = TestTools.FakeRelocator_ThatPutsWarriorsTogether_OnLocation_4_4();

			IFightEngine fakeBattleField = new FightEngine { Attributor = _attributor };
			_game.Relocator = fakeRelocator;
			_game.Attributor = new AttributesHandler();
			_game.BattleField = fakeBattleField;

			_game.Warriors.Add(warriorWinner);
			_game.Warriors.Add(warriorLooser);

			int healthWinner = warriorWinner.Attributes.Health;
			int strengthWinner = warriorWinner.Attributes.Strength;
			int strengthLooser = warriorLooser.Attributes.Strength;

			// ACT
			_game.NextRound();

			// ASSERT
			Assert.Equal(healthWinner + 3, warriorWinner.Attributes.Health);
			Assert.Equal(strengthWinner + strengthLooser + 1, warriorWinner?.Attributes.Strength);
		}
	}
}