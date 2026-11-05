using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Menu, Playing, Paused, GameOver }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private static bool startAfterReload;

    [Header("Run settings")]
    [SerializeField] private int startingLives = 3;
    [SerializeField] private float maximumFuel = 100f;
    [SerializeField] private float fuelPerSecond = 1.1f;
    [SerializeField] private float startingSpeed = 18f;
    [SerializeField] private float maximumSpeed = 42f;
    [SerializeField] private float accelerationPerMeter = 0.012f;

    public GameState State { get; private set; } = GameState.Menu;
    public int Lives { get; private set; }
    public int Coins { get; private set; }
    public float Fuel { get; private set; }
    public float Distance { get; private set; }
    public float FuelFraction => maximumFuel > 0f ? Fuel / maximumFuel : 0f;
    public int StartingLives => startingLives;
    public float Speed => Mathf.Min(maximumSpeed, startingSpeed + Distance * accelerationPerMeter);
    public int Score => Mathf.FloorToInt(Distance) + Coins * 100;
    public int HighScore => Mathf.Max(PlayerPrefs.GetInt("HighScore", 0), Score);
    public string EndReason { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        Lives = startingLives;
        Fuel = maximumFuel;
        Time.timeScale = 1f;
        if (startAfterReload) { startAfterReload = false; State = GameState.Playing; }
    }

    private void Update()
    {
        if (State != GameState.Playing) return;
        Distance += Speed * Time.deltaTime;
        Fuel = Mathf.Max(0f, Fuel - fuelPerSecond * Time.deltaTime);
        if (Fuel <= 0f) EndRun("OUT OF FUEL");
    }

    public void StartGame() { State = GameState.Playing; Time.timeScale = 1f; }
    public void TogglePause()
    {
        if (State == GameState.Playing) { State = GameState.Paused; Time.timeScale = 0f; }
        else if (State == GameState.Paused) { State = GameState.Playing; Time.timeScale = 1f; }
    }
    public void Damage()
    {
        if (State != GameState.Playing) return;
        Lives = Mathf.Max(0, Lives - 1);
        if (Lives == 0) EndRun("CAR WRECKED");
    }
    public void AddCoin() { if (State == GameState.Playing) Coins++; }
    public void AddFuel() { if (State == GameState.Playing) Fuel = Mathf.Min(maximumFuel, Fuel + 35f); }
    private void EndRun(string reason)
    {
        EndReason = reason;
        PlayerPrefs.SetInt("HighScore", HighScore);
        PlayerPrefs.Save();
        State = GameState.GameOver;
        Time.timeScale = 0f;
    }
    public void Restart() { startAfterReload = true; Reload(); }
    public void MainMenu() { startAfterReload = false; Reload(); }
    private void Reload() { Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
    public void Quit() { Application.Quit(); }
}
