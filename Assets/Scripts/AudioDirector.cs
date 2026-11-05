using UnityEngine;

public class AudioDirector : MonoBehaviour
{
    public static AudioDirector Instance { get; private set; }
    public AudioClip music, engine, collision, collect;
    private AudioSource musicSource, engineSource, effects;

    private void Awake()
    {
        Instance = this;
        musicSource = gameObject.AddComponent<AudioSource>();
        engineSource = gameObject.AddComponent<AudioSource>();
        effects = gameObject.AddComponent<AudioSource>();
        musicSource.clip = music;
        musicSource.loop = true;
        musicSource.volume = .22f;
        musicSource.Play();
        engineSource.clip = engine;
        engineSource.loop = true;
        engineSource.volume = .15f;
        engineSource.Play();
    }
    private void Update()
    {
        var game = GameManager.Instance;
        engineSource.pitch = .8f + game.Speed / 60f;
        engineSource.volume = game.State == GameState.Playing ? .15f : 0f;
    }
    public void Collision() { if (collision) effects.PlayOneShot(collision, .5f); }
    public void Collect() { if (collect) effects.PlayOneShot(collect, .5f); }
}
