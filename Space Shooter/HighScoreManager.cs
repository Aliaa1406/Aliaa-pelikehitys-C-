using System.Text.Json;

namespace Space_Shooter
{
    internal static class HighScoreManager
    {
        private static readonly string filePath = "assets/highscores.json";
        public static List<int> Scores { get; private set; } = new List<int>();

        public static void Load()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);

                // Score = new list
                var loadedScores = JsonSerializer.Deserialize<List<int>>(json);
                Scores = loadedScores ?? new List<int> { 0, 0, 0, 0, 0 };
            }
            //if(loadscore == null) same ??
            //{
            //    Scores = new List<int> {0,0,0,}
            //}
           
        }

        public static void Save()
        {
            string? directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonSerializer.Serialize(Scores, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(filePath, json);
        }

        public static void AddScore(int score)
        {
            Scores.Add(score);
            Scores = Scores.OrderByDescending(s => s).Take(5).ToList();
            Save();
        }

        public static bool IsHighScore(int score)
        {
            return Scores.Count < 5 || score > Scores.Min();
        }

        public static int GetRank(int score)
        {
            var tempScores = new List<int>(Scores) { score };
            tempScores = tempScores.OrderByDescending(s => s).ToList();
            return tempScores.IndexOf(score) + 1;
        }

        
    }
}
