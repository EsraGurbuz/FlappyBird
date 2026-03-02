#nullable disable

using System;
using System.Collections.Generic; // Added for List data structure
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

        // Data Structure: A list to hold multiple pipe objects (Top and Bottom pipes)
        private List<Pipe> pipes;

        private System.Windows.Forms.Timer gameTimer;

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

            // 1. Load the Images (Ensure 'Copy if newer' is set!)
            Image bgImage = Image.FromFile("Assets/sprites/background-day.png");
            Image groundImage = Image.FromFile("Assets/sprites/base.png");
            Image birdImage = Image.FromFile("Assets/sprites/yellowbird-midflap.png");
            Image pipeImage = Image.FromFile("Assets/sprites/pipe-green.png");

            // Create a flipped version of the pipe image for the top pipe
            Image topPipeImage = (Image)pipeImage.Clone();
            topPipeImage.RotateFlip(RotateFlipType.Rotate180FlipX);

            // 2. Instantiate Base Objects
            background = new Background(0, 0, 400, 600, bgImage);
            gameGround = new Ground(0, 500, 800, 100, groundImage);
            playerBird = new Bird(100, 200, 34, 24, birdImage);

            // 3. Instantiate Pipes and add them to the list
            pipes = new List<Pipe>();

            // Bottom Pipe (X: 400, Y: 300)
            Pipe bottomPipe = new Pipe(400, 300, 52, 320, pipeImage);
            // Top Pipe (X: 400, Y: -150) -> Positioned higher up
            Pipe topPipe = new Pipe(400, -150, 52, 320, topPipeImage);

            pipes.Add(bottomPipe);
            pipes.Add(topPipe);

            // 4. Add Sprites to the Form
            this.Controls.Add(background.Sprite);
            this.Controls.Add(gameGround.Sprite);
            this.Controls.Add(playerBird.Sprite);

            // Add pipe sprites to the form
            foreach (Pipe pipe in pipes)
            {
                this.Controls.Add(pipe.Sprite);
                pipe.Sprite.BringToFront();
            }

            gameGround.Sprite.BringToFront();
            playerBird.Sprite.BringToFront();

            this.KeyDown += new KeyEventHandler(Form1_KeyDown);

            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 20;
            gameTimer.Tick += new EventHandler(GameLoop);
            gameTimer.Start();
        }

        private void GameLoop(object sender, EventArgs e)
        {
            // 1. Update Entities
            playerBird.Update();
            gameGround.Update();

            foreach (Pipe pipe in pipes)
            {
                pipe.Update();
            }

            // 2. Collision Detection (AABB IntersectsWith method)
            CheckCollisions();
        }

        private void CheckCollisions()
        {
            // Check collision with the ground
            if (playerBird.Sprite.Bounds.IntersectsWith(gameGround.Sprite.Bounds))
            {
                GameOver();
            }

            // Check collision with any of the pipes
            foreach (Pipe pipe in pipes)
            {
                if (playerBird.Sprite.Bounds.IntersectsWith(pipe.Sprite.Bounds))
                {
                    GameOver();
                }
            }

            // Check if the bird flies too high (off the top of the screen)
            if (playerBird.Y < -20)
            {
                GameOver();
            }
        }

        private void GameOver()
        {
            // Stop the game loop
            gameTimer.Stop();
            MessageBox.Show("Game Over! You hit an obstacle.", "Flappy Bird OOP");
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