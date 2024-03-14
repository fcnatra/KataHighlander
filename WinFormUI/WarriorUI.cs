using GameLogic;

namespace WinFormUI
{
	internal record WarriorUI(int Id, string Name)
	{
        public List<WarriorImage> Images { get; set; } = [];
		public World.Direction LastDirection { get; set; }
		public int LastDirectionStep { get; set; }
    }
}
