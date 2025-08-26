using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Raylib_cs;

namespace Space_Shooter
{
    public class HighScoreManager
    {
        private List<(string name, int score)> highScores = new List<(string, int)>();
        private string currentPlayerName = "";
        private readonly int maxNameLength = 10;
        private readonly string highScoreFilePath = "highscores.txt";
        private readonly int maxHighScores = 10;

        public enum State
        {
            EnteringName,
            ShowingScores,
            Inactive
        }

        public State CurrentState = State.Inactive;
        public int CurrentScore;

        public HighScoreManager()
        {
            LoadHighScores();
        }

        public void StartNameEntry(int score)
        {
            CurrentScore = score;
            currentPlayerName = "";
            CurrentState = State.EnteringName;
        }

        public void UpdateNameEntry()
        {
            if (CurrentState != State.EnteringName) return;

            // Handle character input
            int key = Raylib.GetCharPressed();
            while (key > 0)
            {
               
                if (key >= 32 && key <= 125 && currentPlayerName.Length < maxNameLength)
                {
                    currentPlayerName += (char)key;
                }
                key = Raylib.GetCharPressed();
            }

            // Handle backspace
            if (Raylib.IsKeyPressed(KeyboardKey.Backspace) && currentPlayerName.Length > 0)
            {
                currentPlayerName = currentPlayerName.Substring(0, currentPlayerName.Length - 1);
            }

            // Handle enter to submit
            if (Raylib.IsKeyPressed(KeyboardKey.Enter) && currentPlayerName.Length > 0)
            {
                AddHighScore(currentPlayerName, CurrentScore);
                CurrentState = State.ShowingScores;
            }

            // Handle escape to cancel
            if (Raylib.IsKeyPressed(KeyboardKey.Escape))
            {
                CurrentState = State.Inactive;
            }
        }

        public void DrawNameEntry(int screenWidth, int screenHeight)
        {
            if (CurrentState != State.EnteringName) return;

            
            Raylib.DrawRectangle(0, 0, screenWidth, screenHeight, new Color(0, 0, 0, 180));

            // Title
            string title = "NEW HIGH SCORE!";
            int titleWidth = Raylib.MeasureText(title, 40);
            Raylib.DrawText(title, screenWidth / 2 - titleWidth / 2, screenHeight / 2 - 120, 40, Color.Yellow);

            // Score display
            string scoreText = $"Score: {CurrentScore}";
            int scoreWidth = Raylib.MeasureText(scoreText, 30);
            Raylib.DrawText(scoreText, screenWidth / 2 - scoreWidth / 2, screenHeight / 2 - 70, 30, Color.White);

            // Name input prompt
            //string prompt = "Enter your name:";
            //int promptWidth = Raylib.MeasureText(prompt, 24);
            //Raylib.DrawText(prompt, screenWidth / 2 - promptWidth / 2, screenHeight / 2 - 20, 24, Color.White);

            // Name input box
            int boxWidth = 300;
            int boxHeight = 40;
            int boxX = screenWidth / 2 - boxWidth / 2;
            int boxY = screenHeight / 2 + 20;

            Raylib.DrawRectangle(boxX, boxY, boxWidth, boxHeight, Color.DarkGray);
            Raylib.DrawRectangleLines(boxX, boxY, boxWidth, boxHeight, Color.White);

            // Display current name being typed
            string displayName = currentPlayerName;
            if (Math.Floor(Raylib.GetTime() * 2) % 2 == 0) // Blinking cursor
            {
                displayName += "_";
            }

            Raylib.DrawText(displayName, boxX + 10, boxY + 8, 24, Color.White);

            // Instructions
            //string instructions = "Press ENTER to submit, ESCAPE to cancel";
            //int instrWidth = Raylib.MeasureText(instructions, 16);
            //Raylib.DrawText(instructions, screenWidth / 2 - instrWidth / 2, screenHeight / 2 + 80, 16, Color.LightGray);
        }

        public void DrawHighScores(int screenWidth, int screenHeight)
        {
            if (CurrentState != State.ShowingScores) return;

            // Semi-transparent background
            Raylib.DrawRectangle(0, 0, screenWidth, screenHeight, new Color(0, 0, 0, 200));

            // Title
            string title = "HIGH SCORES";
            int titleWidth = Raylib.MeasureText(title, 40);
            Raylib.DrawText(title, screenWidth / 2 - titleWidth / 2, 100, 40, Color.Yellow);

            // Draw scores
            int startY = 180;
            for (int i = 0; i < highScores.Count && i < maxHighScores; i++)
            {
                var (name, score) = highScores[i];
                string scoreText = $"{i + 1}. {name.PadRight(12)} {score:N0}";

                Color textColor = Color.White;
                if (name == currentPlayerName && score == CurrentScore)
                {
                    textColor = Color.Yellow; // Highlight the new score
                }

                Raylib.DrawText(scoreText, screenWidth / 2 - 150, startY + i * 30, 24, textColor);
            }

            // Instructions
            //string instructions = "Press SPACE to play again, ESCAPE to exit";
            //int instrWidth = Raylib.MeasureText(instructions, 20);
            //Raylib.DrawText(instructions, screenWidth / 2 - instrWidth / 2, screenHeight - 80, 20, Color.LightGray);
        }

        public void UpdateShowingScores()
        {
            if (CurrentState != State.ShowingScores) return;

            if (Raylib.IsKeyPressed(KeyboardKey.Space))
            {
                CurrentState = State.Inactive;
            }

            if (Raylib.IsKeyPressed(KeyboardKey.Escape))
            {
                CurrentState = State.Inactive;
            }
        }

        public bool IsHighScore(int score)
        {
            if (highScores.Count < maxHighScores) return true;
            return score > highScores.Last().score;
        }

        private void AddHighScore(string name, int score)
        {
            highScores.Add((name, score));
            highScores = highScores.OrderByDescending(x => x.score).Take(maxHighScores).ToList();
            SaveHighScores();
        }

        private void SaveHighScores()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(highScoreFilePath))
                {
                    foreach (var (name, score) in highScores)
                    {
                        writer.WriteLine($"{name},{score}");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error saving high scores: {e.Message}");
            }
        }

        private void LoadHighScores()
        {
            try
            {
                if (File.Exists(highScoreFilePath))
                {
                    string[] lines = File.ReadAllLines(highScoreFilePath);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(',');
                        if (parts.Length == 2 && int.TryParse(parts[1], out int score))
                        {
                            highScores.Add((parts[0], score));
                        }
                    }
                    highScores = highScores.OrderByDescending(x => x.score).ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error loading high scores: {e.Message}");
                highScores.Clear();
            }
        }
    }
}