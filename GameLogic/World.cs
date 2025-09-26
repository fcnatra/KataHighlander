using System.Drawing;
using System.Net.Http.Headers;

namespace GameLogic
{
	public class World
	{
		public enum Direction { Left, Right, Up, Down }
		public readonly int XStart = 0;
		public readonly int YStart = 0;


		private readonly HashSet<Point> _sanctuaryLocations = new HashSet<Point>();

		public World(int xLimit, int yLimit)
        {
            this.XLimit = xLimit;
            this.YLimit = yLimit;

            AddSanctuariesAtRandomLocations();
        }

		private void AddSanctuariesAtRandomLocations()
        {
            int count = CalcSanctuariesToBuild();

            Random rand = new Random();
            while (_sanctuaryLocations.Count < count)
            {
                int x = rand.Next(XStart, XLimit + 1);
                int y = rand.Next(YStart, YLimit + 1);
                var pt = new Point(x, y);
				
                if (!_sanctuaryLocations.Contains(pt))
					_sanctuaryLocations.Add(pt);
            }
        }

        private int CalcSanctuariesToBuild()
        {
            int area = XLimit * YLimit;
            return Math.Max(1, area / 33); // sample ratio: 3 for 10x10
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

		public HashSet<Point> GetSanctuaryLocations()
		{
			return new HashSet<Point>(_sanctuaryLocations);
		}
    }
}