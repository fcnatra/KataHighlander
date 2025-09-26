using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FakeItEasy;

namespace Tests.KataHighlander
{
    public class SanctuaryTests
    {
        [Fact]
        public void AtLeastOneSanctuaryExistsAfterInitialization()
        {
            // Arrange
            var world = new World(10, 10);

            // Act
            HashSet<Point> sanctuaries = world.GetSanctuaryLocations();

            // Assert
            Assert.True(sanctuaries != null && sanctuaries.Count > 0, "There should be at least one sanctuary square after initialization.");
        }

        [Fact]
        public void WarriorsOnSanctuary_DoNotFight()
        {
            // Arrange
            var world = new World(10, 10);
            var sanctuary = world.GetSanctuaryLocations().First();
            var warrior1 = new Warrior { Id = 1, Location = sanctuary, Health = 10, Strength = 5 };
            var warrior2 = new Warrior { Id = 2, Location = sanctuary, Health = 10, Strength = 5 };
            var warriors = new List<Warrior> { warrior1, warrior2 };
            var game = new Game(world) { Warriors = warriors };

            // Use a fake fight engine to detect if a fight is attempted
            var fakeFightEngine = FakeItEasy.A.Fake<IFightEngine>();
            game.BattleField = fakeFightEngine;

            // Act
            game.NextRound();

            // Assert
            FakeItEasy.A.CallTo(() => fakeFightEngine.FightAndGetWinner(warrior1, warrior2)).MustNotHaveHappened();
            Assert.Contains(warrior1, game.Warriors);
            Assert.Contains(warrior2, game.Warriors);
        }
    }
}
