using System.Collections.Generic;
using UnityEngine;

public class RoadDirector : MonoBehaviour
{
    public Transform player;
    public ObjectPool road, stationary, moving, coins, fuel;
    public float segmentLength = 40f;
    public int visibleSegments = 7;

    private readonly Queue<GameObject> segments = new Queue<GameObject>();
    private readonly List<GameObject> spawned = new List<GameObject>();
    private float nextZ;

    private void Start()
    {
        nextZ = -segmentLength;
        for (int i = 0; i < visibleSegments; i++) AddSegment();
    }
    private void Update()
    {
        if (GameManager.Instance.State != GameState.Playing) return;
        while (nextZ < player.position.z + (visibleSegments - 2) * segmentLength) AddSegment();
        while (segments.Count > 0 && segments.Peek().transform.position.z + segmentLength < player.position.z - 30f)
            segments.Dequeue().SetActive(false);
        for (int i = spawned.Count - 1; i >= 0; i--)
        {
            if (!spawned[i].activeSelf || spawned[i].transform.position.z < player.position.z - 15f)
            {
                spawned[i].SetActive(false);
                spawned.RemoveAt(i);
            }
        }
    }
    private void AddSegment()
    {
        float z = nextZ;
        segments.Enqueue(road.Get(new Vector3(0, 0, z), Quaternion.identity));
        nextZ += segmentLength;
        if (z < 40f) return;
        float difficulty = Mathf.Clamp01(GameManager.Instance.Distance / 1500f);
        int rows = Random.value < difficulty ? 3 : 2;
        for (int row = 0; row < rows; row++)
        {
            float rowZ = z + 8f + row * 12f;
            int lane = Random.Range(-1, 2);
            Spawn(Random.value < .45f ? moving : stationary, lane, rowZ);
            if (Random.value < .65f) Spawn(coins, (lane + 2) % 3 - 1, rowZ + 5f);
        }
        if (Random.value < .5f) Spawn(fuel, Random.Range(-1, 2), z + 35f);
    }
    private void Spawn(ObjectPool pool, int lane, float z)
    {
        spawned.Add(pool.Get(new Vector3(lane * 3.2f, .8f, z), Quaternion.identity));
    }
}
