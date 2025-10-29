using UnityEngine;

// 아이템 종류 정의
public enum ItemType
{
    None,           // 기본값(미지정)
    Heal_HP,        // HP 회복 - HP 포션
    Cure_Bleeding,  // 출혈 치료 - 붕대
    Cure_Fracture,  // 골절 치료 - 부목
    Cure_Poisoning, // 중독 치료 - 해독제, 약초
    Cure_Burn,      // 화상 치료 - 화상 연고
    Artifact,       // 유물
    Clue            // 스토리 단서
}

/// <summary>
/// 모든 아이템의 기본 정보를 정의하는 ScriptableObject(SO) 클래스
/// 런타임 변경에 영향받지 않아 원본 데이터 손상 방지
/// </summary>
[CreateAssetMenu(fileName = "ItemDataSO", menuName = "Inventory System/Item Data")]
public class ItemDataSO : ScriptableObject
{
    [Header("Item Info")]
    public int id;
    public string itemName;
    public ItemType type = ItemType.None;
    public Sprite icon;
}
