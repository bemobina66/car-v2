using System.Collections;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    private Vector3 originalScale;
    private Collider trigger;

    protected virtual void Awake()
    {
        originalScale = transform.localScale;
        trigger = GetComponent<Collider>();
    }
    public virtual void ResetItem()
    {
        transform.localScale = originalScale;
        if (trigger) trigger.enabled = true;
    }
    public abstract void Interact(PlayerCar player);

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponentInParent<PlayerCar>();
        if (!player || !trigger || !trigger.enabled || GameManager.Instance.State != GameState.Playing) return;
        trigger.enabled = false;
        Interact(player);
        if (this is CoinItem || this is FuelItem) StartCoroutine(Disappear());
        else gameObject.SetActive(false);
    }
    private IEnumerator Disappear()
    {
        float elapsed = 0f;
        while (elapsed < .25f)
        {
            elapsed += Time.deltaTime;
            transform.localScale = originalScale * Mathf.Clamp01(1f - elapsed / .25f);
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
