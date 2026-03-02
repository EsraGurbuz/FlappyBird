using System.Drawing;

namespace FlappyBirdOOP.Entities
{
    public class Bird : GameEntity
    {
        // Physics variables
        private int gravity = 2;
        private int velocity = 0;
        private int jumpStrength = -15;

        // --- ANIMATION VARIABLES ---
        private Image[] frames;
        private int currentFrame = 0;
        private int animationCounter = 0;
        private int animationSpeed = 5; // Change wing frame every 5 game ticks (100ms) for smooth flapping

        // Constructor now accepts an array of images instead of just one
        public Bird(int x, int y, int width, int height, Image[] birdFrames)
            : base(x, y, width, height, birdFrames[0]) // Send the first frame to the base GameEntity
        {
            frames = birdFrames;
        }

        public override void Update()
        {
            // 1. Physics Logic
            velocity += gravity;
            Y += velocity;

            // 2. Sprite Animation Logic
            animationCounter++;
            if (animationCounter >= animationSpeed)
            {
                animationCounter = 0;
                currentFrame++;

                // Loop back to the first frame if we reached the end of the array
                if (currentFrame >= frames.Length)
                {
                    currentFrame = 0;
                }

                // Update the visual representation of the bird
                Sprite.Image = frames[currentFrame];
            }

            // Update the underlying PictureBox location
            base.Update();
        }

        public void Flap()
        {
            velocity = jumpStrength;
        }

        public void ResetPhysics()
        {
            velocity = 0;
        }
    }
}