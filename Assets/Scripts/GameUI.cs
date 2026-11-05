using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public GameObject menu, pause, over, hud;
    public Text speed, lives, score, best, coins, finalScore, reason;
    public Image fuelFill;

    private void Update()
    {
        var game = GameManager.Instance;
        menu.SetActive(game.State == GameState.Menu);
        pause.SetActive(game.State == GameState.Paused);
        over.SetActive(game.State == GameState.GameOver);
        hud.SetActive(game.State == GameState.Playing || game.State == GameState.Paused);
        speed.text = Mathf.RoundToInt(game.Speed * 6f) + " KM/H";
        lives.text = "LIVES  " + game.Lives + " / " + game.StartingLives;
        score.text = "SCORE  " + game.Score.ToString("D6");
        best.text = "BEST  " + game.HighScore.ToString("D6");
        coins.text = "COINS  " + game.Coins;
        fuelFill.fillAmount = game.FuelFraction;
        finalScore.text = "SCORE  " + game.Score + "     BEST  " + game.HighScore;
        reason.text = game.EndReason;
    }
}
