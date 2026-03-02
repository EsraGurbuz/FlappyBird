using System.Drawing;

namespace FlappyBirdOOP.Entities
{
    // Inheritance: Pipe is a GameEntity.
    public class Pipe : GameEntity
    {
        private int speed = 5;

        public bool IsScored { get; set; } = false;

        public Pipe(int x, int y, int width, int height, Image image)
            : base(x, y, width, height, image)
        {
        }

        public override void Update()
        {
            // Just move the pipe to the left. 
            // The Form (Game Manager) will handle recycling and scoring.
            X -= speed;
            base.Update();
        }
    }
}