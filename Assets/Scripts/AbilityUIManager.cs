using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위해 이 줄이 있는지 확인하세요.

/// <summary>
/// 능력치(Ability) 창의 UI를 관리하는 클래스.
/// PlayerInput과 PlayerAbility의 이벤트를 구독하여 UI를 업데이트합니다.
/// </summary>
public class AbilityUIManager : MonoBehaviour
{
    [Header("연결")]
    [SerializeField] private GameObject abilityPanel;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerAbility playerAbility;

    [Header("UI 텍스트")]
    // 능력치 '값'과 '레벨'을 표시할 텍스트를 명확히 구분합니다.
    [SerializeField] private TextMeshProUGUI mightValueText;
    [SerializeField] private TextMeshProUGUI mightLevelText;
    [SerializeField] private TextMeshProUGUI temperanceValueText;
    [SerializeField] private TextMeshProUGUI temperanceLevelText;
    [SerializeField] private TextMeshProUGUI spiritValueText;
    [SerializeField] private TextMeshProUGUI spiritLevelText;
    [SerializeField] private TextMeshProUGUI insightValueText;
    [SerializeField] private TextMeshProUGUI insightLevelText;
    [SerializeField] private TextMeshProUGUI agilityValueText;
    [SerializeField] private TextMeshProUGUI agilityLevelText;
    [SerializeField] private TextMeshProUGUI shadowFragmentsText;

    private void Awake()
    {
        // PlayerInput과 PlayerAbility의 이벤트를 구독합니다.
        playerInput.OnToggleAbilityWindowEvent += ToggleAbilityWindow;
        playerAbility.OnAbilityUpgraded += UpdateAbilityText;
        playerAbility.OnShadowFragmentsChanged += UpdateShadowFragmentsText;
    }

    private void Start()
    {
        // 게임 시작 시 패널을 끄고 UI를 초기화합니다.
        abilityPanel.SetActive(false);
        UpdateAllTexts();
    }

    private void OnDestroy()
    {
        // 메모리 누수 방지를 위해 이벤트 구독을 해지합니다.
        if (playerInput != null)
        {
            playerInput.OnToggleAbilityWindowEvent -= ToggleAbilityWindow;
        }
        if (playerAbility != null)
        {
            playerAbility.OnAbilityUpgraded -= UpdateAbilityText;
            playerAbility.OnShadowFragmentsChanged -= UpdateShadowFragmentsText;
        }
    }

    // 능력치 창을 켜고 끄는 함수입니다.
    private void ToggleAbilityWindow()
    {
        bool isActive = abilityPanel.activeSelf;
        abilityPanel.SetActive(!isActive);

        if (!isActive)
        {
            UpdateAllTexts();
        }
    }

    // 모든 UI 텍스트를 업데이트하는 함수입니다.
    private void UpdateAllTexts()
    {
        // '값'과 '레벨'을 각각의 프로퍼티에서 가져와 올바르게 표시합니다.
        mightValueText.text = playerAbility.MightValue.ToString();
        mightLevelText.text = "Lv " + playerAbility.Might.ToString();
        
        temperanceValueText.text = playerAbility.TemperanceValue.ToString();
        temperanceLevelText.text = "Lv " + playerAbility.Temperance.ToString();

        spiritValueText.text = playerAbility.SpiritValue.ToString();
        spiritLevelText.text = "Lv " + playerAbility.Spirit.ToString();

        insightValueText.text = playerAbility.InsightValue.ToString();
        insightLevelText.text = "Lv " + playerAbility.Insight.ToString();

        agilityValueText.text = playerAbility.AgilityValue.ToString();
        agilityLevelText.text = "Lv " + playerAbility.Agility.ToString();

        shadowFragmentsText.text = playerAbility.ShadowFragments.ToString();
    }

    // 특정 능력치 UI만 업데이트하는 함수 (이벤트 수신용)
    private void UpdateAbilityText(PlayerAbility.AbilityType type, int newLevel)
    {
        // 레벨업 시 '값'도 함께 업데이트합니다.
        switch (type)
        {
            case PlayerAbility.AbilityType.Might:
                mightValueText.text = playerAbility.MightValue.ToString();
                mightLevelText.text = "Lv " + newLevel.ToString();
                break;
            case PlayerAbility.AbilityType.Temperance:
                temperanceValueText.text = playerAbility.TemperanceValue.ToString();
                temperanceLevelText.text = "Lv " + newLevel.ToString();
                break;
            case PlayerAbility.AbilityType.Spirit:
                spiritValueText.text = playerAbility.SpiritValue.ToString();
                spiritLevelText.text = "Lv " + newLevel.ToString();
                break;
            case PlayerAbility.AbilityType.Insight:
                insightValueText.text = playerAbility.InsightValue.ToString();
                insightLevelText.text = "Lv " + newLevel.ToString();
                break;
            case PlayerAbility.AbilityType.Agility:
                agilityValueText.text = playerAbility.AgilityValue.ToString();
                agilityLevelText.text = "Lv " + newLevel.ToString();
                break;
        }
    }

    // 재화 UI만 업데이트하는 함수 (이벤트 수신용)
    private void UpdateShadowFragmentsText(int currentFragments)
    {
        shadowFragmentsText.text = currentFragments.ToString();
    }
}

