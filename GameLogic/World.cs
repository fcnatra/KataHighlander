using System.Drawing;
using System.Net.Http.Headers;

namespace GameLogic
{
	public class World
	{
		public enum Direction { Left, Right, Up, Down }
		public readonly int XStart = 0;
		public readonly int YStart = 0;

		public World(int xLimit, int yLimit)
		{
			this.XLimit = xLimit;
			this.YLimit = yLimit;
		}

		public int XLimit { get; internal set; }
		public int YLimit { get; internal set; }
		public List<Point> FightLocations { get; set; } = new List<Point>();

		public static Direction GetDirection(Point from, Point to)
		{
			Direction direction = Direction.Right;
			if (from.Y > to.Y) direction = Direction.Up; else if (from.Y < to.Y) { direction = Direction.Down; }
			if (from.X > to.X) direction = Direction.Left; else if (from.X < to.X) { direction = Direction.Right; }

			return direction;
		}

        public List<Point> GetSanctuaryLocations()
        {
			return [ new Point(0, 0) ];
        }
    }
}