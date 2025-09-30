using System.Drawing;

namespace GameLogic
{
	public class Warrior
	{
		private Point _location, _previousLocation;
		public Point PreviousLocation => _previousLocation;

		public Point Location
		{
			get { return _location; }
			set { _previousLocation = _location; _location = value; }
		}
		public WarriorAttributes Attributes { get; set; } = new WarriorAttributes();
		public string Name { get; set; } = string.Empty;
		public int Id { get; set; }
	}
}