using System.Drawing;

namespace GameLogic
{
	public class RelocationEngine : IRelocator
	{
		private Random _randomizer;

		public RelocationEngine()
		{
			_randomizer = new Random(this.GetHashCode() + DateTime.Now.Microsecond);
		}

		public Point GetEmptyRandomLocation(IGameState game)
		{
			int x, y;
			Point locationCoords;
			var xLimit = game.Gameboard.XLimit;
			var yLimit = game.Gameboard.YLimit;

			do
			{
				x = _randomizer.Next(xLimit);
				y = _randomizer.Next(yLimit);
				locationCoords = new Point(x, y);
			} while (!IsLocationEmpty(locationCoords, game.Warriors));

			return new Point(x, y);
		}

		public Point RelocateWarrior(IGameState game, Warrior warrior, Point offsetDiameter)
		{
			List<Point> possibleCoordinates = GetAvailableCoordinates(warrior.Location, offsetDiameter);
			bool relocated = false;

			while (!relocated && possibleCoordinates.Count > 0)
			{
				int randomIndex = _randomizer.Next(0, possibleCoordinates.Count() - 1);
				var coordToTry = possibleCoordinates[randomIndex];
				possibleCoordinates.RemoveAt(randomIndex);

				if (IsValidLocationForANewWarrior(game, coordToTry))
				{
					warrior.Location = coordToTry;
					relocated = true;
				}
			}
			return warrior.Location;
		}

		private List<Point> GetAvailableCoordinates(Point location, Point offsetDiameter)
		{
			List<Point> availableCoordinates = new();

			for (int x = location.X - offsetDiameter.X; x <= location.X + offsetDiameter.X; x++)
				for (int y = location.Y - offsetDiameter.Y; y <= location.Y + offsetDiameter.Y; y++)
					availableCoordinates.Add(new Point(x, y));

			return availableCoordinates;
		}

		private bool IsValidLocationForANewWarrior(IGameState game, Point locationCoords)
		{
			var warriors = game.Warriors.Where(w => w.Location == locationCoords);
			bool oneOrZeroWarriors = warriors.Count() < 2;

			var gameBoard = game.Gameboard;
			bool insidePerimeter = locationCoords.X >= gameBoard.XStart && locationCoords.X <= gameBoard.XLimit
				&& locationCoords.Y >= gameBoard.YStart && locationCoords.Y <= gameBoard.YLimit;

			return insidePerimeter && oneOrZeroWarriors;
		}

		private bool IsLocationEmpty(Point locationCoords, IEnumerable<Warrior> warriors)
		{
			return !warriors.Where(w => w.Location == locationCoords).Any();
		}
	}
}
