namespace Game;

using Godot;
/// <summary>
/// A class that manages the user interface (UI) elements of the game, such as health display and inventory display.
/// </summary>
[GlobalClass]
public partial class UiManager : CanvasLayer
{
    [Export] private Label _scoreLiteral;
    [Export] private Label _healthLiteral;
    [Export] private Label _middleScreenLabel;
    [Export] private Timer _messageTimer;
    public bool isIngame { get; set; } = false;
    public bool isGameOver { get; set; } = false;
    public void Update(double delta, int playerHeath, int playerScore)
    {
        if (isGameOver)
        {
            _scoreLiteral.Hide();
            _healthLiteral.Hide();
            string gameOverString = $"Game Over!\nScore: {_scoreLiteral.Text}\nPress Enter to Restart";
            DisplayMessage(gameOverString, 999);
        }
        if (_messageTimer.TimeLeft == 0)
            _middleScreenLabel.Hide();
        _scoreLiteral.Text = playerScore.ToString("D8");
        _healthLiteral.Text = playerHeath.ToString("D2");
    }
    public void NewGame(double countdown)
    {
        Show();
        _scoreLiteral.Show();
        _healthLiteral.Show();
        DisplayMessage($"Get Ready!\n{countdown:0.0}", countdown);
    }
    private void DisplayMessage(string message, double duration = 2.0)
    {
        _middleScreenLabel.Text = message;
        _middleScreenLabel.Show();
        _messageTimer.Start(duration);
    }
}