using UnityEngine;

public class Fireworks : MonoBehaviour
{
    public Transform player;
    public ParticleSystem burst;
    [SerializeField] private Vector2 interval = new Vector2(4f, 8f);
    private float timer = 3f;

    private void Update()
    {
        if (GameManager.Instance.State != GameState.Playing) return;
        timer -= Time.deltaTime;
        if (timer > 0f) return;
        timer = Random.Range(interval.x, interval.y);
        transform.position = player.position + new Vector3(Random.Range(-20f, 20f), Random.Range(14f, 23f), Random.Range(45f, 80f));
        var main = burst.main;
        main.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, .5f, .15f), new Color(.45f, .75f, 1f));
        burst.Play();
    }
}
