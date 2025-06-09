public class HighScoreManager
{
    private List<(string name, int score)> highScores = new List<(string, int)>();
    private string currentPlayerName = "";
    private readonly int maxNameLength = 10;
    private readonly string highScoreFilePath = "highscores.txt";

    public enum State
    {
        EnteringName,
        ShowingScores,
        Inactive
    }

    public State CurrentState = State.Inactive;

    // Metodit nimen syöttöä varten
    public void StartNameEntry(int score)
    {
        CurrentScore = score;
        currentPlayerName = "";
        CurrentState = State.EnteringName;
    }

    // Muut toiminnallisuudet
    public int CurrentScore;

    // Nimen syötön päivitys
    public void UpdateNameEntry()
    {
        // Näppäimistön käsittely täällä...
    }

    
    public void DrawNameEntry(int screenWidth, int screenHeight)
    {
        // Piirtäminen täällä...
    }

    
}