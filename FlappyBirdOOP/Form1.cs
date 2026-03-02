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

            // UI Elements: Score Label (UPDATED FOR VISIBILITY)
            scoreLabel = new Label
            {
                Text = "Score: 0",
                Location = new Point(10, 10),
                AutoSize = true, // Let the box resize itself based on the text length
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = Color.Black, // Black text stands out against the sky
                BackColor = Color.LightGoldenrodYellow, // A solid background to prevent WinForms transparency bugs
                BorderStyle = BorderStyle.FixedSingle // A nice border around our score box
            };

            // 1. Add the label directly to the Form's control list
            this.Controls.Add(scoreLabel);
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

            // 1. SCORING LOGIC (For immediate updates)
            // If the right edge of the pipe (X + Width) has passed the left edge of the bird (X)
            // AND we haven't scored from this pipe yet:
            if (pipes[0].X + pipes[0].Width < playerBird.X && !pipes[0].IsScored)
            {
                score++;
                scoreLabel.Text = "Score: " + score;

                // OOP State Update: We got the score from this pipe, set it to true so that
                // the game loop (running at 50 FPS) doesn't increment the score repeatedly.
                pipes[0].IsScored = true;
            }

            // 2. PIPE RECYCLING LOGIC (When it goes off-screen)
            if (pipes[0].X < -100)
            {
                int pipeGap = 150;
                int newBottomY = randomGenerator.Next(250, 450);
                int newTopY = newBottomY - pipeGap - pipes[1].Height;

                pipes[0].SetPosition(400, newBottomY);
                pipes[1].SetPosition(400, newTopY);

                // CRUCIAL: When the pipe loops back, to act like a brand new pipe,
                // we reset its scored state back to false!
                pipes[0].IsScored = false;
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