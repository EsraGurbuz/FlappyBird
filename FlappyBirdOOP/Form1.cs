#nullable disable

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FlappyBirdOOP.Entities;

namespace FlappyBirdOOP
{
    public partial class Form1 : Form
    {
        private Bird playerBird;
        private Ground gameGround;
        private Background background;
        private List<Pipe> pipes;

        // Game Logic Variables
        private System.Windows.Forms.Timer gameTimer;
        private int score = 0;
        private Label scoreLabel;
        private Random randomGenerator;

        public Form1()
        {
            InitializeComponent();
            SetupGame();
        }

        private void SetupGame()
        {
            this.Width = 400;
            this.Height = 600;
            this.Text = "Flappy Bird OOP";
            this.DoubleBuffered = true;

            // Initialize Random Generator
            randomGenerator = new Random();

            Image bgImage = Image.FromFile("Assets/sprites/background-day.png");
            Image groundImage = Image.FromFile("Assets/sprites/base.png");
            Image birdImage = Image.FromFile("Assets/sprites/yellowbird-midflap.png");
            Image pipeImage = Image.FromFile("Assets/sprites/pipe-green.png");

            Image topPipeImage = (Image)pipeImage.Clone();
            topPipeImage.RotateFlip(RotateFlipType.Rotate180FlipX);

            background = new Background(0, 0, 400, 600, bgImage);
            gameGround = new Ground(0, 500, 800, 100, groundImage);
            playerBird = new Bird(100, 200, 34, 24, birdImage);

            pipes = new List<Pipe>();

            // Initial pipes
            Pipe bottomPipe = new Pipe(400, 300, 52, 320, pipeImage);
            Pipe topPipe = new Pipe(400, -170, 52, 320, topPipeImage);

            pipes.Add(bottomPipe); // pipes[0] is bottom
            pipes.Add(topPipe);    // pipes[1] is top

            // UI Elements: Score Label
            scoreLabel = new Label
            {
                Text = "Score: 0",
                Location = new Point(10, 10),
                Size = new Size(150, 40),
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent, // Make background transparent
                Parent = background.Sprite // Crucial for transparent background on WinForms
            };

            this.Controls.Add(background.Sprite);
            this.Controls.Add(gameGround.Sprite);
            this.Controls.Add(playerBird.Sprite);

            foreach (Pipe pipe in pipes)
            {
                this.Controls.Add(pipe.Sprite);
                pipe.Sprite.BringToFront();
            }

            gameGround.Sprite.BringToFront();
            playerBird.Sprite.BringToFront();

            // Keep the score text on top of everything
            scoreLabel.BringToFront();

            this.KeyDown += new KeyEventHandler(Form1_KeyDown);

            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 20;
            gameTimer.Tick += new EventHandler(GameLoop);
            gameTimer.Start();
        }

        private void GameLoop(object sender, EventArgs e)
        {
            playerBird.Update();
            gameGround.Update();

            foreach (Pipe pipe in pipes)
            {
                pipe.Update();
            }

            // Pipe Recycling and Scoring Logic
            // If the bottom pipe goes off-screen (X < -100)
            if (pipes[0].X < -100)
            {
                // The space between top and bottom pipe where the bird flies
                int pipeGap = 150;

                // Generate a random Y coordinate for the bottom pipe (between 250 and 450)
                int newBottomY = randomGenerator.Next(250, 450);

                // Calculate the top pipe's Y coordinate based on the bottom pipe and the gap
                int newTopY = newBottomY - pipeGap - pipes[1].Height;

                // Move pipes back to the right side of the screen with new Y coordinates
                pipes[0].SetPosition(400, newBottomY);
                pipes[1].SetPosition(400, newTopY);

                // Successfully passed a pipe, increase score!
                score++;
                scoreLabel.Text = "Score: " + score;
            }

            CheckCollisions();
        }

        private void CheckCollisions()
        {
            if (playerBird.Sprite.Bounds.IntersectsWith(gameGround.Sprite.Bounds))
            {
                GameOver();
            }

            foreach (Pipe pipe in pipes)
            {
                if (playerBird.Sprite.Bounds.IntersectsWith(pipe.Sprite.Bounds))
                {
                    GameOver();
                }
            }

            if (playerBird.Y < -20)
            {
                GameOver();
            }
        }

        private void GameOver()
        {
            gameTimer.Stop();
            MessageBox.Show("Game Over! Final Score: " + score, "Flappy Bird OOP");
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                playerBird.Flap();
            }
        }

        private void Form1_Load(object sender, EventArgs e) { }
    }
}