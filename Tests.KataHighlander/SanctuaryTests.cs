using System.Collections.Generic;
using System.Drawing;
using Xunit;
using GameLogic;

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
            List<Point> sanctuaries = world.GetSanctuaryLocations();

            // Assert
            Assert.True(sanctuaries != null && sanctuaries.Count > 0, "There should be at least one sanctuary square after initialization.");
        }
    }
}
