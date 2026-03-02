#nullable disable // Relaxing modern C#'s strict null checks for this specific file.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FlappyBirdOOP.Entities; // Adjust this namespace according to your project name

namespace FlappyBirdOOP
{
    public partial class Form1 : Form
    {
        // Aggregation: The Form class "has" these objects (Has-A relationship).
        private Bird playerBird;
        private Ground gameGround;
        private Background background;

        // Data Structure: A list to hold multiple pipe objects (Top and Bottom pipes)
        private List<Pipe> pipes;

        // Game Logic Variables
        private System.Windows.Forms.Timer gameTimer;
        private int score = 0;
        private Label messageLabel;
        private Random randomGenerator;

        // State Management: Tracks whether the game is currently running or over
        private bool isGameOver = false;
        private bool isGameStarted = false;

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

            // WinForms Trade-off Solution: Enable DoubleBuffering to prevent screen flickering
            this.DoubleBuffered = true;

            // Initialize Random Generator for pipe heights
            randomGenerator = new Random();

            // 2. Load the Images
            // IMPORTANT: Ensure "Copy to Output Directory" is set to "Copy if newer" for ALL these images.
            Image bgImage = Image.FromFile("Assets/sprites/background-day.png");
            Image groundImage = Image.FromFile("Assets/sprites/base.png");
            Image birdImage = Image.FromFile("Assets/sprites/yellowbird-midflap.png");
            Image pipeImage = Image.FromFile("Assets/sprites/pipe-green.png");

            // Create a flipped version of the pipe image for the top pipe
            Image topPipeImage = (Image)pipeImage.Clone();
            topPipeImage.RotateFlip(RotateFlipType.Rotate180FlipX);

            // 3. Instantiate Base Objects
            background = new Background(0, 0, 400, 600, bgImage);
            gameGround = new Ground(0, 500, 800, 100, groundImage);
            playerBird = new Bird(100, 200, 34, 24, birdImage);

            // 4. Instantiate Pipes and add them to the list
            pipes = new List<Pipe>();
            Pipe bottomPipe = new Pipe(400, 300, 52, 320, pipeImage);
            Pipe topPipe = new Pipe(400, -170, 52, 320, topPipeImage);

            pipes.Add(bottomPipe); // pipes[0] is the bottom pipe
            pipes.Add(topPipe);    // pipes[1] is the top pipe

            // 5. UI Elements: Aesthetically pleasing centered UI box
            messageLabel = new Label
            {
                AutoSize = true, // FIX: This prevents text from ever being cut off
                Font = new Font("Segoe UI", 18, FontStyle.Bold), // Modern and cleaner font
                ForeColor = Color.White,
                BackColor = Color.DarkSlateBlue, // Solid, elegant color instead of buggy WinForms transparency
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15), // Gives breathing room between the text and the border
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(messageLabel);

            // 6. Add Sprites and UI to the Form's controls
            this.Controls.Add(background.Sprite);
            this.Controls.Add(gameGround.Sprite);
            this.Controls.Add(playerBird.Sprite);

            foreach (Pipe pipe in pipes)
            {
                this.Controls.Add(pipe.Sprite);
                pipe.Sprite.BringToFront();
            }

            this.Controls.Add(messageLabel);

            // Ensure correct Z-Order (Rendering order)
            gameGround.Sprite.BringToFront();
            playerBird.Sprite.BringToFront();

            // Set the initial message using our new helper method
            ShowCenteredMessage("Ready?\nPress SPACE");

            // 7. Setup Input Listener
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);

            // 8. Setup the Game Loop
            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 20; // 50 FPS
            gameTimer.Tick += new EventHandler(GameLoop);
            // gameTimer.Start();
        }

        // Game Loop: Runs continuously every 20 milliseconds.
        private void GameLoop(object sender, EventArgs e)
        {
            // Update Entity Physics and Positions
            playerBird.Update();
            gameGround.Update();

            foreach (Pipe pipe in pipes)
            {
                pipe.Update();
            }

            // 1. SCORING LOGIC
            if (pipes[0].X + pipes[0].Width < playerBird.X && !pipes[0].IsScored)
            {
                score++;
                // messageLabel.Text = "Score: " + score;  --> THIS LINE IS DELETED! 
                pipes[0].IsScored = true;
            }

            // 2. PIPE RECYCLING LOGIC (When it goes off-screen)
            if (pipes[0].X < -100)
            {
                int pipeGap = 150;
                int newBottomY = randomGenerator.Next(250, 450);
                int newTopY = newBottomY - pipeGap - pipes[1].Height;

                // Move pipes back to the right side
                pipes[0].SetPosition(400, newBottomY);
                pipes[1].SetPosition(400, newTopY);

                // CRUCIAL: Reset the scored state for the recycled pipe
                pipes[0].IsScored = false;
            }

            // 3. Check for Game Over conditions
            CheckCollisions();
        }

        private void CheckCollisions()
        {
            // AABB Collision: Bird hits the ground
            if (playerBird.Sprite.Bounds.IntersectsWith(gameGround.Sprite.Bounds))
            {
                GameOver();
            }

            // AABB Collision: Bird hits any of the pipes
            foreach (Pipe pipe in pipes)
            {
                if (playerBird.Sprite.Bounds.IntersectsWith(pipe.Sprite.Bounds))
                {
                    GameOver();
                }
            }

            // Boundary Collision: Bird flies too high off the screen
            if (playerBird.Y < -20)
            {
                GameOver();
            }
        }

        private void GameOver()
        {
            gameTimer.Stop();
            isGameOver = true;

            // Use our helper to show and center the Game Over screen
            ShowCenteredMessage("Game Over!\nScore: " + score + "\nPress ENTER");
        }

        // UI Helper: Updates the text, adjusts size automatically, and perfectly centers it
        private void ShowCenteredMessage(string text)
        {
            messageLabel.Text = text;

            // Math to perfectly center the label on the Form
            messageLabel.Left = (this.ClientSize.Width - messageLabel.Width) / 2;
            messageLabel.Top = (this.ClientSize.Height - messageLabel.Height) / 2;

            messageLabel.Visible = true;
            messageLabel.BringToFront(); // Ensure it stays on top of everything
        }

        private void RestartGame()
        {
            isGameOver = false;
            isGameStarted = false;
            score = 0;

            // Back to the waiting screen
            ShowCenteredMessage("Ready?\nPress SPACE");

            // Reset Entity Positions and Physics
            playerBird.SetPosition(100, 200);
            playerBird.ResetPhysics();

            pipes[0].SetPosition(400, 300);
            pipes[0].IsScored = false;

            pipes[1].SetPosition(400, -170);
            pipes[1].IsScored = false;
        }

        // Player Input
        // Player Input
        // Player Input
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!isGameStarted && !isGameOver && e.KeyCode == Keys.Space)
            {
                isGameStarted = true;
                messageLabel.Visible = false; // HIDE the center message when playing!

                playerBird.Flap();
                gameTimer.Start();
            }
            // If the game is currently playing and the user presses SPACE: JUST FLAP
            else if (isGameStarted && !isGameOver && e.KeyCode == Keys.Space)
            {
                playerBird.Flap();
            }
            // If the game is over and the user presses ENTER: RESTART THE GAME
            else if (isGameOver && e.KeyCode == Keys.Enter)
            {
                RestartGame();
            }
        }

        // Fix for the Visual Studio Designer's expected Form Load method
        private void Form1_Load(object sender, EventArgs e)
        {
            // Left empty intentionally, as we handle setup in SetupGame().
        }
    }
}