using UnityEngine;

public class MovingObstacle : Interactable
{
    public float lateralRange = 1f;
    private float origin;
    private float phase;

    public override void ResetItem()
    {
        base.ResetItem();
        origin = transform.position.x;
        phase = Random.Range(0f, Mathf.PI * 2f);
    }
    private void Update()
    {
        if (GameManager.Instance.State != GameState.Playing) return;
        var position = transform.position;
        position.x = origin + Mathf.Sin(Time.time * 1.5f + phase) * lateralRange;
        transform.position = position;
    }
    public override void Interact(PlayerCar player) { player.Hit(); }
}
