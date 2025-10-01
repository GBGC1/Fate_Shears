using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Script;
using UnityEngine;

namespace Weapon
{
    public static class WeaponStats
    {
        private static Dictionary<WeaponType, WeaponStat> weponStats;

        public static void LoadStats()
        {
            weponStats = new Dictionary<WeaponType, WeaponStat>();
            string json = File.ReadAllText(Constant.JsonPath.WEAPONSTAT);
            Debug.Log(json);

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            };

            List<WeaponStat> weaponStats =
                JsonSerializer.Deserialize<List<WeaponStat>>(json, options);

            foreach (WeaponStat stat in weaponStats)
            {
                Debug.Log(stat.Type + " " + stat.Damage + " " + stat.AttackSpeed);
                weponStats.Add(stat.Type, stat);
            }
        }

        public static WeaponStat GetWeaponStat(WeaponType weaponType)
        {
            return weponStats[weaponType];
        }

        public class WeaponStat
        {
            public WeaponType Type { get; set; }
            public int Damage { get; set; }
            public float AttackSpeed { get; set; }
            public float Range { get; set; }
        }
    }
}