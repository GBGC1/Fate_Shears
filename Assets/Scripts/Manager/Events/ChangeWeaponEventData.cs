using System;
using Weapon;

namespace Script.Manager.Events
{
    public class ChangeWeaponEventData
    {
        public WeaponType Type { get; private set; }

        public ChangeWeaponEventData(WeaponType type)
        {
            Type = type;
        }
    }
}   