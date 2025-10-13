using System;

namespace Script.Manager.Events
{
    public class ChangeWeaponEventData
    {
        public string Type { get; private set; }

        public ChangeWeaponEventData(String type)
        {
            Type = type;
        }
    }
}   