using UnityEngine;

public class FuelItem : Interactable
{
    [SerializeField] private float rotationSpeed = 45f;
    private void Update() { transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f); }
    public override void Interact(PlayerCar player)
    {
        GameManager.Instance.AddFuel();
        player.Collect();
    }
}
