using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public UIManager uimanager;

    // 스탯 값
    public int might = 5;
    public int temperance = 5;
    public int spirit = 3;
    public int insight = 2;
    public int agility = 4;

    public int mightLevel = 1;
    public int temperanceLevel = 1;
    public int spiritLevel = 1;
    public int insightLevel = 1;
    public int agilityLevel = 1;

    public int shadowFragments = 20;
    public int upgradeCost = 1;

    public void UpgradeMight()
    {
        if (UpgradeStat(ref might))
        {
            mightLevel++;
            uimanager.UpdateSpecificStatUI("Might", might, mightLevel, shadowFragments);
        }
    }

    public void UpgradeTemperance()
    {
        if (UpgradeStat(ref temperance))
        {
            temperanceLevel++;
            uimanager.UpdateSpecificStatUI("Temperance", temperance, temperanceLevel, shadowFragments);
        }
    }

    public void UpgradeSpirit()
    {
        if (UpgradeStat(ref spirit))
        {
            spiritLevel++;
            uimanager.UpdateSpecificStatUI("Spirit", spirit, spiritLevel, shadowFragments);
        }
    }

    public void UpgradeInsight()
    {
        if (UpgradeStat(ref insight))
        {
            insightLevel++;
            uimanager.UpdateSpecificStatUI("Insight", insight, insightLevel, shadowFragments);
        }
    }

    public void UpgradeAgility()
    {
        if (UpgradeStat(ref agility))
        {
            agilityLevel++;
            uimanager.UpdateSpecificStatUI("Agility", agility, agilityLevel, shadowFragments);
        }
    }

    private bool UpgradeStat(ref int stat)
    {
        if (shadowFragments >= upgradeCost)
        {
            shadowFragments -= upgradeCost;
            upgradeCost++;
            stat++;
            Debug.Log("스탯 업그레이드! 현재 그림자 조각: " + shadowFragments);
            return true;
        }
        else
        {
            Debug.Log("그림자 조각이 부족합니다.");
            return false;
        }
    }
}