using GameLogic;
using System.Threading;

namespace WinFormUI
{
	public partial class FormWorld : Form
	{
		private ImageOps? _imageOps;
		private List<WarriorUI> _warriorsUI = new();
		private Game? _game;

		private const int WORLD_X_WIDTH = 70;
		private const int WORLD_Y_HEIGHT = 70;
		private const int STANDARD_IMAGE_SIZE = 10;
		private const int MOVEMENT_SPEED_MS = 50;

		private Point AdjustLocationToCanvas(Point gameLocation) => new Point(gameLocation.X * STANDARD_IMAGE_SIZE, gameLocation.Y * STANDARD_IMAGE_SIZE);

		public FormWorld()
		{
			InitializeComponent();
		}

		private void FormWorld_Load(object sender, EventArgs e)
		{
			ImproveGraphicsPerformance();
			InitializeGame();

			if (_game is null)
			{
				MessageBox.Show("Can't load the game.");
				return;
			}

			_imageOps = new ImageOps(this, _game.Warriors);
			try
			{
				_imageOps.SetupBackground();
				_warriorsUI = _imageOps.LoadImages();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Can't load the images: {ex}");
				return;
			}

			this.Text = $"WARRIORS: {_game.Warriors.Count()}";
			StartPlaying();
		}

		private void ImproveGraphicsPerformance()
		{
			this.DoubleBuffered = true;
		}

		private void InitializeGame()
		{
			var world = new GameLogic.World(WORLD_X_WIDTH, WORLD_Y_HEIGHT);
			this._game = new GameLogic.Game(world);
			this._game.Attributor = new GameLogic.AttributesHandler();
			this._game.Relocator = new GameLogic.RelocationEngine();
			this._game.BattleField = new GameLogic.FightEngine();

			_game.CreateDefaultWarriors();
		}

		private void StartPlaying()
		{
			movementTimer.Interval = MOVEMENT_SPEED_MS;
			movementTimer.Start();
		}

		private void FormWorld_Paint(object sender, PaintEventArgs e)
		{
			if (_game is null) return;

			Graphics canvas = e.Graphics;
			var warriors = _game.Warriors;

			foreach (var warrior in warriors)
			{
				var warriorUI = _warriorsUI.First(w => w.Id == warrior.Id);
				var location = warrior.Location;
				if (_game.Gameboard.FightLocations.Contains(location))
					DrawFightInLocation(canvas, location);
				else
					DrawWarrior(canvas, warriorUI, warrior);
			}
		}

		private void DrawFightInLocation(Graphics canvas, Point location)
		{
			UpdateStatusInformation();
			if (_imageOps?.FightImage is null) return;

			var canvasLocation = AdjustLocationToCanvas(location);
			canvas.DrawImage(_imageOps.FightImage, canvasLocation);
		}

		private void DrawWarrior(Graphics canvas, WarriorUI warriorUI, Warrior warrior)
		{
			var canvasLocation = AdjustLocationToCanvas(warrior.Location);
			var direction = World.GetDirection(warrior.PreviousLocation, warrior.Location);
			var availableImages = GetAllImagesForWarrior(warriorUI, direction);

			int step = GetDirectionStepToDraw(warriorUI, direction, availableImages.Count());
			var image = GetImageToDraw(direction, availableImages, step);

			canvas.DrawImage(image, canvasLocation);
			canvas.DrawString(warriorUI.Name, SystemFonts.DialogFont, Brushes.White, canvasLocation.X + 50, canvasLocation.Y - 3);

			warriorUI.LastDirection = direction;
			warriorUI.LastDirectionStep = step;
		}

		private static IEnumerable<WarriorImage> GetAllImagesForWarrior(WarriorUI warriorUI, World.Direction direction)
		{
			return warriorUI.Images.Where(x => x.Name.Contains(direction.ToString(), StringComparison.InvariantCultureIgnoreCase));
		}

		private static Image GetImageToDraw(World.Direction direction, IEnumerable<WarriorImage> availableImages, int step)
		{
			var image = availableImages.First(x => x.Name.Contains($"{direction}.{step}", StringComparison.InvariantCultureIgnoreCase)).Image;
			if (image is null)
				image = ImageOps.DefaultImage();
			return image;
		}

		private static int GetDirectionStepToDraw(WarriorUI warriorUI, World.Direction direction, int availableSteps)
		{
			var step = warriorUI.LastDirection != direction ? 1 : warriorUI.LastDirectionStep + 1;
			if (step > availableSteps) step = 1;
			return step;
		}

		private void movementTimer_Tick(object sender, EventArgs e)
		{
			_game?.NextRound();
			this.Invalidate();
		}

		private void FormWorld_Click(object sender, EventArgs e)
		{
			movementTimer.Enabled = !movementTimer.Enabled;
		}

		private void UpdateStatusInformation()
		{
			int? warriorsLeft = _game?.Warriors.Count();
			if (warriorsLeft == 1)
			{
				movementTimer.Enabled = false;
				movementTimer_Tick(this, new EventArgs());
			}

			this.Text = $"WARRIORS: {warriorsLeft}";
		}
	}
}
