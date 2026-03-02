using System.Drawing;
using System.Windows.Forms;

namespace FlappyBirdOOP.Entities // Adjust this namespace according to your project name
{
    // abstract: We cannot instantiate this class directly using "new GameEntity()".
    // Because a generic "Game Entity" cannot exist on the screen; it must be a specific object like a Bird or a Pipe.
    public abstract class GameEntity
    {
        // Encapsulation: Using Properties.
        // In C#, instead of writing separate getter/setter methods like in Java, we use this property structure.
        // 'protected set' means: Only this class and derived classes (Bird, Pipe) can modify this value.
        // It is read-only (get) from the outside (e.g., from the main Form).
        public int X { get; protected set; }
        public int Y { get; protected set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }

        // Composition: Every GameEntity contains its own visual component (PictureBox).
        public PictureBox Sprite { get; private set; }

        // Constructor
        public GameEntity(int x, int y, int width, int height, Image image)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;

            // Dynamically creating the PictureBox via code.
            Sprite = new PictureBox
            {
                Location = new Point(X, Y),
                Size = new Size(Width, Height),
                Image = image,
                SizeMode = PictureBoxSizeMode.StretchImage, // Fit the image into the box bounds
                BackColor = Color.Transparent // Set background to transparent
            };
        }

        // Polymorphism: A virtual Update method.
        // virtual: Derived classes can override this method according to their specific behaviors.
        // For instance, Update means applying gravity for the Bird, but it means moving left for the Pipe.
        public virtual void Update()
        {
            // Core logic: Update the actual position of the PictureBox on the screen when X and Y change.
            Sprite.Location = new Point(X, Y);
        }

        // Encapsulation: Allow external managers (like the Game Loop) to safely reposition the entity.
        public void SetPosition(int newX, int newY)
        {
            X = newX;
            Y = newY;
            Sprite.Location = new Point(X, Y);
        }
    }
}