using System.Drawing;

namespace FlappyBirdOOP.Entities
{
    // Inheritance: Pipe is a GameEntity.
    public class Pipe : GameEntity
    {
        // Encapsulation: The speed at which the pipe moves left. 
        // This should match the Ground's speed to look realistic.
        private int speed = 5;

        public Pipe(int x, int y, int width, int height, Image image)
            : base(x, y, width, height, image)
        {
        }

        // Polymorphism: Overriding the Update method for Pipe-specific behavior.
        public override void Update()
        {
            // Move the pipe to the left
            X -= speed;

            // When the pipe goes off-screen to the left, recycle it by moving it to the right.
            // This is a basic form of Object Pooling, saving memory instead of constantly destroying and creating objects.
            if (X < -100)
            {
                X = 800; // Move it far to the right to come back again
            }

            // Call the parent to update the actual PictureBox location
            base.Update();
        }
    }
}