#nullable disable // Relaxing modern C#'s strict null checks for this specific file.

using System;
using System.Drawing;
using System.Windows.Forms;
using FlappyBirdOOP.Entities; // Adjust this namespace according to your project name

namespace FlappyBirdOOP
{
    public partial class Form1 : Form
    {
        // Aggregation: The Form class "has a" Bird object (Has-A relationship).
        private Bird playerBird;

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

            // 2. Load the Image
            // IMPORTANT: Ensure "Copy to Output Directory" is set to "Copy if newer" for this image.
            Image birdImage = Image.FromFile("Assets/sprites/yellowbird-midflap.png");

            // 3. Instantiate the Bird Object
            // Creating a bird at X: 100, Y: 200 with dimensions 34x24.
            playerBird = new Bird(100, 200, 34, 24, birdImage);

            // 4. Add the Bird's Sprite (PictureBox) to the Form's controls so it appears on screen
            this.Controls.Add(playerBird.Sprite);

            // 5. Setup Input Listener
            // Event-Driven Programming: Trigger the Form1_KeyDown method when a key is pressed.
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);

            // 6. Setup the Game Loop
            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 20; // 20 milliseconds = 50 Frames Per Second (FPS)
            gameTimer.Tick += new EventHandler(GameLoop);
            gameTimer.Start();
        }

        // Game Loop: This method runs continuously every 20 milliseconds.
        private void GameLoop(object sender, EventArgs e)
        {
            // The power of OOP: The Form doesn't know the math of how the bird falls. 
            // It simply tells the bird to "Update yourself".
            playerBird.Update();
        }

        // Player Input
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // If the pressed key is the Spacebar
            if (e.KeyCode == Keys.Space)
            {
                // The power of OOP: The Form doesn't know how the jump physics work.
                // It just tells the bird to "Flap".
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