using System.Drawing;

namespace FlappyBirdOOP.Entities
{
    // Inheritance: Ground "is a" GameEntity.
    public class Ground : GameEntity
    {
        // Encapsulation: The speed at which the ground moves left.
        private int speed = 5;

        public Ground(int x, int y, int width, int height, Image image)
            : base(x, y, width, height, image)
        {
        }

        // Polymorphism: Overriding the Update method for Ground-specific behavior.
        public override void Update()
        {
            // Move the ground to the left to create the illusion of forward flight.
            X -= speed;

            // Infinite Scrolling Logic:
            // When the ground moves too far to the left, we seamlessly snap it back to the right.
            // This assumes we will make the ground PictureBox wider than the screen.
            if (X < -100)
            {
                X = 0;
            }

            // Call the parent to update the actual PictureBox location on screen.
            base.Update();
        }
    }
}