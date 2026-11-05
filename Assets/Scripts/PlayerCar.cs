using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCar : MonoBehaviour
{
    public InputActionAsset controls;
    public float laneWidth = 3.2f;
    public float laneChangeSpeed = 9f;
    public ParticleSystem sparks, pickup;
    public Light underglow;
    public Renderer body;

    private InputAction left, right, pause;
    private int lane;
    private float flashTime;
    private Color bodyColor;

    private void Awake() { if (body) bodyColor = body.material.color; }
    private void OnEnable()
    {
        var map = controls.FindActionMap("Driving", true);
        left = map.FindAction("Left", true);
        right = map.FindAction("Right", true);
        pause = map.FindAction("Pause", true);
        map.Enable();
    }
    private void OnDisable() { if (controls) controls.FindActionMap("Driving").Disable(); }

    private void Update()
    {
        var game = GameManager.Instance;
        if (pause.WasPressedThisFrame()) game.TogglePause();
        if (game.State != GameState.Playing) return;
        if (left.WasPressedThisFrame()) lane = Mathf.Max(-1, lane - 1);
        if (right.WasPressedThisFrame()) lane = Mathf.Min(1, lane + 1);

        var position = transform.position;
        position.x = Mathf.Lerp(position.x, lane * laneWidth, Time.deltaTime * laneChangeSpeed);
        position.z += game.Speed * Time.deltaTime;
        transform.position = position;
        float lean = (lane * laneWidth - position.x) * -4f;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, lean), Time.deltaTime * 7f);
        if (underglow) underglow.intensity = Mathf.Lerp(2f, 8f, (game.Speed - 18f) / 24f);
        if (flashTime > 0f)
        {
            flashTime -= Time.deltaTime;
            if (flashTime <= 0f && body) body.material.color = bodyColor;
        }
    }
    public void Hit()
    {
        GameManager.Instance.Damage();
        if (sparks) sparks.Play();
        if (body) { body.material.color = Color.red; flashTime = .3f; }
        if (AudioDirector.Instance) AudioDirector.Instance.Collision();
    }
    public void Collect()
    {
        if (pickup) pickup.Play();
        if (AudioDirector.Instance) AudioDirector.Instance.Collect();
    }
}
