using System.Drawing;

namespace FlappyBirdOOP.Entities
{
    // Inheritance: Bird "is a" GameEntity. 
    // In C#, we use ':' instead of the 'extends' keyword used in Java.
    public class Bird : GameEntity
    {
        // Encapsulation: Bird-specific physics variables.
        // We keep them private because nothing outside needs to change gravity directly.
        private int gravity = 2;     // Pulls the bird down (increases Y)
        private int velocity = 0;    // Current vertical speed
        private int lift = -15;      // Upward force when flapping (negative because Y=0 is at the top)

        // Constructor
        // 'base' is C#'s equivalent to 'super' in Java. It calls the parent (GameEntity) constructor.
        public Bird(int x, int y, int width, int height, Image image)
            : base(x, y, width, height, image)
        {
        }

        // Polymorphism: Overriding the base Update method
        public override void Update()
        {
            // 1. Apply gravity to velocity (accelerate downwards)
            velocity += gravity;

            // 2. Apply velocity to the Y position
            Y += velocity;

            // 3. Call the parent class's Update method to actually move the PictureBox on the screen
            // Similar to super.Update() in Java.
            base.Update();
        }

        // Bird-specific behavior: Jumping
        public void Flap()
        {
            // When the player presses a key, we override the current downward velocity 
            // with a sharp upward lift.
            velocity = lift;
        }
        // Encapsulation: The Form doesn't know about 'velocity', it just tells the bird to reset its physics.
        public void ResetPhysics()
        {
            velocity = 0;
        }
    }
}