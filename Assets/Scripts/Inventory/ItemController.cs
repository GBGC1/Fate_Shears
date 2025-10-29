using UnityEngine;

public class ItemController : MonoBehaviour
{
    public ItemDataSO itemData;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager.Instance.AddItem(itemData);
            Destroy(gameObject);
        }
    }
}
