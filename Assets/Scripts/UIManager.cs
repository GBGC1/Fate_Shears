using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI mightui;
    public TextMeshProUGUI mightLv;
    public TextMeshProUGUI temperanceui;
    public TextMeshProUGUI temperanceLv;
    public TextMeshProUGUI spiritui;
    public TextMeshProUGUI spiritLv;
    public TextMeshProUGUI insightui;
    public TextMeshProUGUI insightLv;
    public TextMeshProUGUI agilityui;
    public TextMeshProUGUI agilityLv;
    public TextMeshProUGUI shadowcount;
    public PlayerStats playerstats;
    public GameObject statusPanel;

    

    void Awake()
    {
        
        if (statusPanel != null)
        {
            statusPanel.SetActive(false);
        }

        if (playerstats != null)
        {
            UpdateAllStatTexts();
        }
    }

    // 모든 스탯 텍스트를 초기화하는 함수
    public void UpdateAllStatTexts()
    {
        if (playerstats != null)
        {
            mightui.text = playerstats.might.ToString();
            mightLv.text = playerstats.mightLevel.ToString();

            temperanceui.text = playerstats.temperance.ToString();
            temperanceLv.text = playerstats.temperanceLevel.ToString();

            spiritui.text = playerstats.spirit.ToString();
            spiritLv.text = playerstats.spiritLevel.ToString();

            insightui.text = playerstats.insight.ToString();
            insightLv.text = playerstats.insightLevel.ToString();

            agilityui.text = playerstats.agility.ToString();
            agilityLv.text = playerstats.agilityLevel.ToString();

            shadowcount.text = playerstats.shadowFragments.ToString();
        }
    }

    public void UpdateSpecificStatUI(string statName, int statValue, int statLevel, int shadowFragments)
    {
        switch (statName)
        {
            case "Might":
                mightui.text = statValue.ToString();
                mightLv.text = statLevel.ToString();
                shadowcount.text = shadowFragments.ToString();
                break;
            case "Temperance":
                temperanceui.text = statValue.ToString();
                temperanceLv.text = statLevel.ToString();
                shadowcount.text = shadowFragments.ToString();
                break;
            case "Spirit":
                spiritui.text = statValue.ToString();
                spiritLv.text = statLevel.ToString();
                shadowcount.text = shadowFragments.ToString();
                break;
            case "Insight":
                insightui.text = statValue.ToString();
                insightLv.text = statLevel.ToString();
                shadowcount.text = shadowFragments.ToString();
                break;
            case "Agility":
                agilityui.text = statValue.ToString();
                agilityLv.text = statLevel.ToString();
                shadowcount.text = shadowFragments.ToString();
                break;
        }
    }
    
     public void OnToggleStatus(InputValue value)
    {
        // 키가 눌렸을 때만 실행
        if (value.isPressed && statusPanel != null)
        {
            // 패널의 현재 활성화 상태를 뒤집음 (켜져있으면 끄고, 꺼져있으면 켬)
            bool isActive = statusPanel.activeSelf;
            statusPanel.SetActive(!isActive);

            // 패널이 켜졌을 때만 텍스트를 최신 정보로 업데이트
            if (!isActive)
            {
                UpdateAllStatTexts();
            }
        }
    }
}