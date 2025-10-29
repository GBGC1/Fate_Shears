using UnityEngine;
using Weapon;

namespace Weapon
{
    public class WeaponBody : MonoBehaviour
    {
        public static WeaponType CurrentForm { get; private set; } = WeaponType.Shears;

        public static WeaponStats.WeaponStat Stat { get; private set; }

        void Awake()
        {
            WeaponStats.LoadStats();
        }
        
        void Start()
        {
            LoadStat();
        }

        // Update is called once per frame
        void Update()
        {
        }

        public static void ChangeForm(WeaponType type)
        {
            CurrentForm = type;
            LoadStat();
        }
        
        private static void LoadStat()
        {
            Stat = WeaponStats.GetWeaponStat(CurrentForm);
        }
    }
}