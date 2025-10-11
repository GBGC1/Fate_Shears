using UnityEngine;
using Weapon;

public class WeaponBody : MonoBehaviour
{
    public static WeaponStats.WeaponStat Stat { get; private set; }

    void Awake()
    {
        WeaponStats.LoadStats();
    }
    void Start()
    {
        ChangeStat(WeaponType.Shears);
    }

    public static void ChangeStat(WeaponType type)
    {
        Stat = WeaponStats.GetWeaponStat(type);
        Debug.Log(Stat.Type);
    }
}