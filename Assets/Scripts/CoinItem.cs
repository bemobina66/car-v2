using UnityEngine;

public class CoinItem : Interactable
{
    [SerializeField] private float rotationSpeed = 100f;
    private void Update() { transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f); }
    public override void Interact(PlayerCar player)
    {
        GameManager.Instance.AddCoin();
        player.Collect();
    }
}
