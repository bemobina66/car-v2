using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab;
    public int initialSize = 12;
    private readonly List<GameObject> objects = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < initialSize; i++) AddObject();
    }
    private GameObject AddObject()
    {
        var item = Instantiate(prefab, transform);
        item.SetActive(false);
        objects.Add(item);
        return item;
    }
    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        GameObject item = objects.Find(candidate => !candidate.activeSelf);
        if (!item) item = AddObject();
        item.transform.SetPositionAndRotation(position, rotation);
        item.SetActive(true);
        var interactable = item.GetComponent<Interactable>();
        if (interactable) interactable.ResetItem();
        return item;
    }
}
