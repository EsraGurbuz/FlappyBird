// Background.cs
using System.Drawing;
namespace FlappyBirdOOP.Entities
{
    // A simple static background that just inherits properties without extra logic.
    public class Background : GameEntity
    {
        public Background(int x, int y, int width, int height, Image image) : base(x, y, width, height, image) { }
    }
}