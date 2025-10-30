using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 적 사망 시 확률에 따른 아이템 드롭을 담당하는 클래스
/// 드롭 시 리스트에 등록된 아이템 중 무작위로 선택하여 월드에 생성
/// </summary>
public class LootDropper : MonoBehaviour
{
    [System.Serializable]
    // 전리품 아이템 클래스 정의
    public class LootItem
    {
        [Header("Loot Settings")]
        // 드롭할 전리품 아이템 프리팹
        public GameObject itemPrefab;
    }

    [Range(0f, 1f)]
    [SerializeField] private float dropChance = 0.5f; // 아이템 드롭 확률

    [Header("Loot Item List")]
    [SerializeField] private List<LootItem> lootItemList = new List<LootItem>();

    public void DropLoot()
    {
        // 아이템 드롭 여부 결정
        float randomDropCheck = Random.Range(0f, 1f);

        // 드롭 확률보다 작을 경우 드롭 실패
        if (randomDropCheck < dropChance)
        {
            return;
        }

        // 아이템 리스트 비어있는지 확인
        if (lootItemList.Count == 0)
        {
            Debug.LogWarning("LootDropper : 아이템 리스트가 비어있습니다.");
            return;
        }

        // 드롭 성공 시, 아이템 리스트에서 무작위로 하나 선택
        int randomIndex = Random.Range(0, lootItemList.Count);
        LootItem selectedLoot = lootItemList[randomIndex];

        // 선택된 아이템 드롭 실행
        if (selectedLoot != null)
        {
            Vector3 dropPosition = transform.position;
            InstantiateItem(selectedLoot.itemPrefab, dropPosition);
        }

    }

    private void InstantiateItem(GameObject itemPrefab, Vector3 position)
    {
        if (itemPrefab == null)
        {
            Debug.LogWarning("드롭할 아이템 프리팹이 설정되지 않았습니다! - " + gameObject.name);
            return;
        }

        // 드롭 위치에 아이템 프리팹 생성
        Instantiate(itemPrefab, position, Quaternion.identity);
        Debug.Log($"아이템 드롭 성공! {itemPrefab.name.Replace("(Clone)", "")} 생성됨.");
    }
}
