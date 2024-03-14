namespace WinFormUI
{
	internal class WarriorImage
	{
		private Image? _image;
		private string _name = string.Empty;

		public Image? Image => _image;
		public string Name => _name;

        private WarriorImage()
        {}

        public static WarriorImage FromFile(string fileName)
		{
			Image image = Image.FromFile(fileName);
			return new WarriorImage { _image = image, _name = fileName };
		}
	}
}
