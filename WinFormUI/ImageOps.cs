using GameLogic;

namespace WinFormUI
{
	internal class ImageOps(Form UI, List<Warrior> Warriors)
	{
        public Image? FightImage { get; set; }

        public List<WarriorUI> LoadImages()
		{
			var warriorsUI = new List<WarriorUI>();
			var warriors = Warriors;

			foreach (var warrior in warriors)
				warriorsUI.Add(LoadWarriorImages(warrior));

			LoadFlameImage();

			return warriorsUI;
		}

		private void LoadFlameImage()
		{
			this.FightImage = Image.FromFile("Resources\\Flame.png");
		}

		private WarriorUI LoadWarriorImages(Warrior warrior)
		{
			var name = warrior.Name;
			if (!Directory.GetFiles($"Resources\\Warriors\\", $"{name}*.png").Any())
				name = "default";

			var warriorUI = new WarriorUI(warrior.Id, name);

			var imageFiles = Directory.GetFiles($"Resources\\Warriors\\", $"{name}*.png").ToList();
			imageFiles.ForEach(file =>
			{
				WarriorImage image = WarriorImage.FromFile(file);
				warriorUI.Images.Add(image);
			});

			return warriorUI;
		}

		public void SetupBackground()
		{
			UI.BackgroundImageLayout = ImageLayout.Stretch;
			UI.BackgroundImage = Image.FromFile("Resources\\Solid-Color-HD-Wallpapers.jpg");
		}

		internal static Image DefaultImage() =>  new Bitmap(71, 71);
	}
}
