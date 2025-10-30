using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItemController : MonoBehaviour, IPointerClickHandler
{
    ItemDataSO item;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            Debug.Log("더블 클릭으로 인벤토리 내에서 아이템 사용");

            if (item != null)
            {
                if (ItemUsageHandler.Instance != null)
                {
                    ItemUsageHandler.Instance.UseItem(item);
                    RemoveItem();
                }
            }
            else Debug.LogError("사용하려는 아이템이 없습니다.");
        }
    }

    public void RemoveItem()
    {
        InventoryManager.Instance.RemoveItem(item);
        Destroy(gameObject);
    }

    public void AddItem(ItemDataSO newItem)
    {
        item = newItem;
    }
}
