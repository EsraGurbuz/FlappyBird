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
        private Label scoreLabel;
        private Random randomGenerator;

        // State Management: Tracks whether the game is currently running or over
        private bool isGameOver = false;

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

            // 5. UI Elements: Score Label
            scoreLabel = new Label
            {
                Text = "Score: 0",
                Location = new Point(10, 10),
                AutoSize = true,
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = Color.Black,
                BackColor = Color.LightGoldenrodYellow, // Solid background to prevent WinForms transparency bugs
                BorderStyle = BorderStyle.FixedSingle
            };

            // 6. Add Sprites and UI to the Form's controls
            this.Controls.Add(background.Sprite);
            this.Controls.Add(gameGround.Sprite);
            this.Controls.Add(playerBird.Sprite);

            foreach (Pipe pipe in pipes)
            {
                this.Controls.Add(pipe.Sprite);
                pipe.Sprite.BringToFront();
            }

            this.Controls.Add(scoreLabel);

            // Ensure correct Z-Order (Rendering order)
            gameGround.Sprite.BringToFront();
            playerBird.Sprite.BringToFront();
            scoreLabel.BringToFront();

            // 7. Setup Input Listener
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);

            // 8. Setup the Game Loop
            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 20; // 50 FPS
            gameTimer.Tick += new EventHandler(GameLoop);
            gameTimer.Start();
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

            // 1. SCORING LOGIC (For immediate updates)
            // If the right edge of the pipe has passed the left edge of the bird
            // AND we haven't scored from this pipe yet:
            if (pipes[0].X + pipes[0].Width < playerBird.X && !pipes[0].IsScored)
            {
                score++;
                scoreLabel.Text = "Score: " + score;

                // OOP State Update: Mark as scored to prevent infinite points
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
            // Stop the game loop and update the game state
            gameTimer.Stop();
            isGameOver = true;

            // Update UI to show instructions
            scoreLabel.Text = "Game Over! Score: " + score + " [Press ENTER]";
        }

        private void RestartGame()
        {
            // 1. Reset Game State and UI
            isGameOver = false;
            score = 0;
            scoreLabel.Text = "Score: " + score;

            // 2. Reset Entity Positions and Physics
            playerBird.SetPosition(100, 200);
            playerBird.ResetPhysics();

            // Put pipes back to their starting positions and reset their score states
            pipes[0].SetPosition(400, 300);
            pipes[0].IsScored = false;

            pipes[1].SetPosition(400, -170);
            pipes[1].IsScored = false;

            // 3. Restart the Game Loop
            gameTimer.Start();
        }

        // Player Input
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // Flap only if the game is actively playing
            if (e.KeyCode == Keys.Space && !isGameOver)
            {
                playerBird.Flap();
            }
            // Restart only if the game is over and the user pressed Enter
            else if (e.KeyCode == Keys.Enter && isGameOver)
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