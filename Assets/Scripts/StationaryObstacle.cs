public class StationaryObstacle : Interactable
{
    public override void Interact(PlayerCar player) { player.Hit(); }
}
