#nullable disable // Relaxing modern C#'s strict null checks for this specific file.

using System;
using System.Drawing;
using System.Windows.Forms;
using FlappyBirdOOP.Entities; // Adjust this namespace according to your project name

namespace FlappyBirdOOP
{
    public partial class Form1 : Form
    {
        // Aggregation: The Form class "has a" Bird, Ground, and Background (Has-A relationship).
        private Bird playerBird;
        private Ground gameGround;
        private Background background;

        // Explicitly stating that we are using the Windows Forms Timer.
        private System.Windows.Forms.Timer gameTimer;

        public Form1()
        {
            InitializeComponent();
            SetupGame();
        }

        private void SetupGame()
        {
            // 1. Form Settings
            this.Width = 400;
            this.Height = 600;
            this.Text = "Flappy Bird OOP";

            // WinForms Trade-off Solution: Enable DoubleBuffering to prevent screen flickering.
            this.DoubleBuffered = true;

            // 2. Load the Images
            // IMPORTANT: Ensure "Copy to Output Directory" is set to "Copy if newer" for ALL these images.
            Image bgImage = Image.FromFile("Assets/sprites/background-day.png");
            Image groundImage = Image.FromFile("Assets/sprites/base.png");
            Image birdImage = Image.FromFile("Assets/sprites/yellowbird-midflap.png");

            // 3. Instantiate the Game Objects
            // Background covers the whole screen (Width: 400, Height: 600)
            background = new Background(0, 0, 400, 600, bgImage);

            // Ground is positioned at the bottom (Y: 500). 
            // We make it twice as wide (800) to create a seamless scrolling illusion.
            gameGround = new Ground(0, 500, 800, 100, groundImage);

            // Bird starts in the air
            playerBird = new Bird(100, 200, 34, 24, birdImage);

            // 4. Add Sprites to the Form's controls
            // The order matters in WinForms. The first one added stays at the bottom (Z-Index).
            this.Controls.Add(background.Sprite);
            this.Controls.Add(gameGround.Sprite);
            this.Controls.Add(playerBird.Sprite);

            // Ensure the bird and ground are rendered in front of the background
            gameGround.Sprite.BringToFront();
            playerBird.Sprite.BringToFront();

            // 5. Setup Input Listener
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);

            // 6. Setup the Game Loop
            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 20; // 50 FPS
            gameTimer.Tick += new EventHandler(GameLoop);
            gameTimer.Start();
        }

        // Game Loop: Runs continuously every 20 milliseconds.
        private void GameLoop(object sender, EventArgs e)
        {
            // Update the physics and positions of moving entities
            playerBird.Update();
            gameGround.Update();
        }

        // Player Input
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // If the pressed key is the Spacebar
            if (e.KeyCode == Keys.Space)
            {
                playerBird.Flap();
            }
        }

        // Fix for the Visual Studio Designer's expected Form Load method
        private void Form1_Load(object sender, EventArgs e)
        {
            // Left empty intentionally, as we handle setup in SetupGame().
        }
    }
}