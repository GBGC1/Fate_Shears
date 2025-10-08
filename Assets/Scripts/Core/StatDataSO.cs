using UnityEngine;

// 기본 스탯 타입 정의
public enum StatType
{
    // StatDataSO 초기 기본 스탯
    MaxHP,
    MaxStamina,
    AttackPower,
    DefensePower,
    MoveSpeed
}

/// <summary>
/// 캐릭터의 초기 스탯을 정의하는 ScriptableObject(SO) 클래스
/// 런타임 변경에 영향받지 않아 원본 데이터 손상 방지
/// </summary>
[CreateAssetMenu(fileName = "NewStatData", menuName = "Stat System/Stat Data")]
public class StatDataSO : ScriptableObject
{
    [Header("Initial Stats")]
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float attackPower = 10f;
    [SerializeField] private float defensePower = 5f;
    [SerializeField] private float moveSpeed = 5f;

    // 특정 스탯 초기값 반환
    public float GetInitialStat(StatType statType)
    {
        return statType switch
        {
            StatType.MaxHP => maxHP,
            StatType.MaxStamina => maxStamina,
            StatType.AttackPower => attackPower,
            StatType.DefensePower => defensePower,
            StatType.MoveSpeed => moveSpeed,
            _ => 0f
        };
    }
}
