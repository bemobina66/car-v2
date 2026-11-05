using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 5, -11);
    [SerializeField] private float followSpeed = 6f;
    [SerializeField] private float lookAhead = 12f;

    private void LateUpdate()
    {
        if (!target) return;
        transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.unscaledDeltaTime * followSpeed);
        transform.LookAt(target.position + Vector3.forward * lookAhead + Vector3.up);
    }
}
