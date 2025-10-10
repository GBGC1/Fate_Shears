using UnityEngine;
using Weapon;

public class WeaponBody : MonoBehaviour
{
    public WeaponStats.WeaponStat Stat { get; private set; }
    
    void Start()
    {
        ChangeStat(WeaponType.Shears);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ChangeStat(WeaponType type)
    {
        Stat = WeaponStats.GetWeaponStat(type);
    }
}