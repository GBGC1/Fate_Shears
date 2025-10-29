using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject inventoryUI;

    [Header("Inventory Settings")]
    public static InventoryManager Instance;
    public List<ItemDataSO> itemList = new List<ItemDataSO>();

    public Transform itemContent;
    public GameObject InventoryItem;

    public InventoryItemController[] InventoryItems;

    private void Awake()
    {
        Instance = this;

        // 인벤토리 이벤트 구독
        if (playerInput == null || inventoryUI == null)
        {
            Debug.LogError("InventoryManager : 필수 컴포넌트가 할당되지 않았습니다.");
        }
        
        playerInput.OnInventoryEvent += ToggleInventoryUI;
    }

    private void OnDestroy()
    {
        // 인벤토리 이벤트 구독 해지
        playerInput.OnInventoryEvent -= ToggleInventoryUI;
    }

    public void AddItem(ItemDataSO item)
    {
        itemList.Add(item);
    }

    public void RemoveItem(ItemDataSO item)
    {
        itemList.Remove(item);
    }

    public void ListItems()
    {
        foreach (Transform item in itemContent)
        {
            Destroy(item.gameObject);
        }

        // itemList를 순회하며 UI 생성
        foreach (var item in itemList)
        {
            GameObject obj = Instantiate(InventoryItem, itemContent);

            Transform iconTransform = obj.transform.Find("ItemIcon");

            if (iconTransform != null)
            {
                var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();

                if (itemIcon != null)
                {
                    itemIcon.sprite = item.icon;
                }
                else Debug.LogError("ItemIcon 오브젝트에 Image 컴포넌트가 없습니다.");
            }
            else Debug.LogError("ItemSlot 프리팹의 ItemIcon을 찾을 수 없습니다.");
        }

        SetInventoryItems();
    }

    private void ToggleInventoryUI()
    {
        ListItems();
        inventoryUI.SetActive(!inventoryUI.activeSelf);
    }

    private void SetInventoryItems()
    {
        InventoryItems = itemContent.GetComponentsInChildren<InventoryItemController>();

        Debug.Log($"itemList.Count: {itemList.Count}, InventoryItems.Length: {InventoryItems.Length}");

        for (int i = 0; i < itemList.Count; i++)
        {
            if (i < InventoryItems.Length)
            {
                InventoryItems[i].AddItem(itemList[i]); 
            }
            else
            {
                Debug.LogError($"InventoryItems 배열 길이보다 itemList가 더 큽니다. i: {i}, InventoryItems.Length: {InventoryItems.Length}");
            }
        }
    }
}
