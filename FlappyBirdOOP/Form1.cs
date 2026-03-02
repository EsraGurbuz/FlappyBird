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

        private System.Windows.Forms.Timer gameTimer;
        private int score = 0;
        private Random randomGenerator;

        private bool isGameOver = false;
        private bool isGameStarted = false;

        private PictureBox readyUI;
        private PictureBox gameoverUI;
        private Image[] numberSprites;
        private List<PictureBox> scoreDigits;

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

            randomGenerator = new Random();

            // 1. Load Base Images
            Image bgImage = Image.FromFile("Assets/sprites/background-day.png");
            Image groundImage = Image.FromFile("Assets/sprites/base.png");
            Image pipeImage = Image.FromFile("Assets/sprites/pipe-green.png");

            Image topPipeImage = (Image)pipeImage.Clone();
            topPipeImage.RotateFlip(RotateFlipType.Rotate180FlipX);

            // --- SPRITE ANIMATION LOAD ---
            // Load Bird Animation Frames (Mid -> Down -> Mid -> Up for smooth cycle)
            Image[] birdFrames = new Image[4];
            birdFrames[0] = Image.FromFile("Assets/sprites/yellowbird-midflap.png");
            birdFrames[1] = Image.FromFile("Assets/sprites/yellowbird-downflap.png");
            birdFrames[2] = Image.FromFile("Assets/sprites/yellowbird-midflap.png");
            birdFrames[3] = Image.FromFile("Assets/sprites/yellowbird-upflap.png");

            // 2. Load Number Sprites for the Score (0 to 9)
            numberSprites = new Image[10];
            for (int i = 0; i < 10; i++)
            {
                numberSprites[i] = Image.FromFile($"Assets/sprites/{i}.png");
            }
            scoreDigits = new List<PictureBox>();

            // 3. Instantiate Game Objects (Pass the birdFrames array to the Bird)
            background = new Background(0, 0, 400, 600, bgImage);
            gameGround = new Ground(0, 500, 800, 100, groundImage);
            playerBird = new Bird(100, 200, 34, 24, birdFrames);

            pipes = new List<Pipe>();
            pipes.Add(new Pipe(400, 300, 52, 320, pipeImage));
            pipes.Add(new Pipe(400, -170, 52, 320, topPipeImage));

            // 4. UNIFIED RENDERING LAYER
            this.Controls.Add(background.Sprite);

            background.Sprite.Controls.Add(gameGround.Sprite);
            background.Sprite.Controls.Add(playerBird.Sprite);

            foreach (Pipe pipe in pipes)
            {
                background.Sprite.Controls.Add(pipe.Sprite);
            }

            // 5. Setup Visual UI Objects
            readyUI = new PictureBox
            {
                Image = Image.FromFile("Assets/sprites/message.png"),
                SizeMode = PictureBoxSizeMode.AutoSize,
                BackColor = Color.Transparent,
                Visible = true
            };
            background.Sprite.Controls.Add(readyUI);

            gameoverUI = new PictureBox
            {
                Image = Image.FromFile("Assets/sprites/gameover.png"),
                SizeMode = PictureBoxSizeMode.AutoSize,
                BackColor = Color.Transparent,
                Visible = false
            };
            background.Sprite.Controls.Add(gameoverUI);

            readyUI.Left = (this.ClientSize.Width - readyUI.Width) / 2;
            readyUI.Top = (this.ClientSize.Height - readyUI.Height) / 2;

            gameoverUI.Left = (this.ClientSize.Width - gameoverUI.Width) / 2;
            gameoverUI.Top = (this.ClientSize.Height - gameoverUI.Height) / 2;

            // 6. PERFECT Z-ORDER STACKING
            gameGround.Sprite.BringToFront();
            playerBird.Sprite.BringToFront();
            readyUI.BringToFront();

            // 7. Setup Inputs and Timer
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);

            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 20;
            gameTimer.Tick += new EventHandler(GameLoop);
        }

        // --- UI HELPER METHODS ---

        private void UpdateScoreDisplay()
        {
            string scoreString = score.ToString();

            while (scoreDigits.Count < scoreString.Length)
            {
                PictureBox pb = new PictureBox
                {
                    SizeMode = PictureBoxSizeMode.AutoSize,
                    BackColor = Color.Transparent
                };
                background.Sprite.Controls.Add(pb);
                scoreDigits.Add(pb);
            }

            for (int i = scoreString.Length; i < scoreDigits.Count; i++)
            {
                scoreDigits[i].Visible = false;
            }

            int totalWidth = 0;
            for (int i = 0; i < scoreString.Length; i++)
            {
                int digit = scoreString[i] - '0';
                scoreDigits[i].Image = numberSprites[digit];
                totalWidth += scoreDigits[i].Width;
            }

            int currentX = (this.ClientSize.Width - totalWidth) / 2;
            for (int i = 0; i < scoreString.Length; i++)
            {
                scoreDigits[i].Location = new Point(currentX, 50);
                scoreDigits[i].Visible = true;
                scoreDigits[i].BringToFront();
                currentX += scoreDigits[i].Width;
            }
        }

        // --- GAME LOGIC ---

        private void GameLoop(object sender, EventArgs e)
        {
            playerBird.Update();
            gameGround.Update();

            foreach (Pipe pipe in pipes)
            {
                pipe.Update();
            }

            if (pipes[0].X + pipes[0].Width < playerBird.X && !pipes[0].IsScored)
            {
                score++;
                UpdateScoreDisplay();
                pipes[0].IsScored = true;
            }

            if (pipes[0].X < -100)
            {
                int pipeGap = 150;
                int newBottomY = randomGenerator.Next(250, 450);
                int newTopY = newBottomY - pipeGap - pipes[1].Height;

                pipes[0].SetPosition(400, newBottomY);
                pipes[1].SetPosition(400, newTopY);
                pipes[0].IsScored = false;
            }

            CheckCollisions();
        }

        private void CheckCollisions()
        {
            if (playerBird.Sprite.Bounds.IntersectsWith(gameGround.Sprite.Bounds))
                GameOver();

            foreach (Pipe pipe in pipes)
            {
                if (playerBird.Sprite.Bounds.IntersectsWith(pipe.Sprite.Bounds))
                    GameOver();
            }

            if (playerBird.Y < -20)
                GameOver();
        }

        private void GameOver()
        {
            gameTimer.Stop();
            isGameOver = true;

            foreach (PictureBox pb in scoreDigits) pb.Visible = false;

            readyUI.Visible = false;
            gameoverUI.Visible = true;
            gameoverUI.BringToFront();
        }

        private void RestartGame()
        {
            isGameOver = false;
            isGameStarted = false;
            score = 0;

            readyUI.Visible = true;
            readyUI.BringToFront();
            gameoverUI.Visible = false;

            playerBird.SetPosition(100, 200);
            playerBird.ResetPhysics();

            pipes[0].SetPosition(400, 300);
            pipes[0].IsScored = false;

            pipes[1].SetPosition(400, -170);
            pipes[1].IsScored = false;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!isGameStarted && !isGameOver && e.KeyCode == Keys.Space)
            {
                isGameStarted = true;

                readyUI.Visible = false;
                gameoverUI.Visible = false;

                UpdateScoreDisplay();
                playerBird.Flap();
                gameTimer.Start();
            }
            else if (isGameStarted && !isGameOver && e.KeyCode == Keys.Space)
            {
                playerBird.Flap();
            }
            else if (isGameOver && e.KeyCode == Keys.Enter)
            {
                RestartGame();
            }
        }

        private void Form1_Load(object sender, EventArgs e) { }
    }
}