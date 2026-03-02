# Flappy Bird OOP Clone: The Brave Yellow Canary 🐤

**Developer:** Esra Gürbüz  
**Student ID:** 230543001  

A classic Flappy Bird clone built from scratch using **C#** and **Windows Forms**. This project was developed as an Object-Oriented Programming (OOP) assignment to demonstrate clean software architecture, game loop mechanics, and solid class design in a desktop application environment.

### 🌟 The Story of Our Protagonist
The protagonist of this game is inspired by the resilient **Yellow Canary**. For centuries, canaries have been loyal companions to humans—bringing joy to our homes with their melodies and historically even protecting miners by warning them of unseen dangers. In this project, our little virtual canary acts as your companion, guiding you through a frustratingly fun maze of pipes, testing your patience, reflexes, and determination to never give up!

---

## 🎮 Features

* **Custom Sprite Rendering:** Uses original game assets for the UI (Ready/Game Over screens), score digits, and entities, avoiding native WinForms UI limitations.
* **Sprite Animation:** The canary features a multi-frame flapping animation cycle (Mid -> Down -> Mid -> Up) for a realistic and organic flight feel.
* **State Management:** Seamless transitions between "Ready", "Playing", and "Game Over" states without reloading the form or resetting the application.
* **Object Pooling:** Pipes are recycled and repositioned once they leave the screen to optimize memory usage and prevent memory leaks, maintaining a stable 50 FPS.
* **AABB Collision Detection:** Accurate Axis-Aligned Bounding Box (AABB) collision algorithms between the bird, pipes, and the ground.

## 🏗️ OOP Principles Demonstrated

This project strictly adheres to the four pillars of Object-Oriented Programming:

1. **Inheritance:** A central `GameEntity` base class provides shared attributes (X, Y, Width, Height, Sprite). The `Bird`, `Pipe`, `Ground`, and `Background` classes inherit from this base class to prevent code duplication (DRY principle).
2. **Polymorphism:** The `Update()` method is declared as virtual in the base class. Entities override this method to exhibit unique behaviors (e.g., the Bird applies gravity and animation, while Pipes and Ground apply horizontal scrolling).
3. **Encapsulation:** Internal physics variables (velocity, gravity, jump strength) and the score are kept private. The main game loop interacts with entities only through safe, controlled public methods like `Flap()` and `ResetPhysics()`.
4. **Abstraction:** The `Form1` class acts purely as a Game Manager. The complex mathematics of physics and animations are abstracted away into their respective entity classes.

## 💻 Tech Stack

* **Language:** C#
* **Framework:** .NET Framework (Windows Forms)
* **IDE:** Visual Studio

## 🚀 How to Play

1. Run the application.
2. Press **SPACE** to start the game and help the canary flap its wings.
3. Navigate through the randomly generated pipe gaps.
4. If you hit a pipe or the ground, press **ENTER** to restart your journey.

---
*Developed with a focus on clean code, structural engineering, and a little bit of canary resilience.*
