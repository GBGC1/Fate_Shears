using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ShadowFragmentUI : MonoBehaviour
{
    [Header("Component References")]
    [Tooltip("데이터를 가지고 있는 PlayerAbility 스크립트")]
    [SerializeField] private PlayerAbility playerAbility;

    private TextMeshProUGUI fragmentText;

    private void Awake()
    {
        fragmentText = GetComponent<TextMeshProUGUI>();

        if (playerAbility == null)
        {
            Debug.LogError("ShadowFragmentUI: PlayerAbility 참조가 인스펙터에 할당되지 않았습니다!", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (playerAbility != null)
        {
            // PlayerAbility의 이벤트에 UpdateFragmentText 함수를 구독(연결)
            playerAbility.OnShadowFragmentsChanged += UpdateFragmentText;
        }
    }

    private void OnDisable()
    {
        if (playerAbility != null)
        {
            playerAbility.OnShadowFragmentsChanged -= UpdateFragmentText;
        }
    }

    private void Start()
    {
        // 게임이 시작될 때 현재 그림자 조각 개수로 텍스트를 초기화
        if (playerAbility != null && fragmentText != null)
        {
            UpdateFragmentText(playerAbility.ShadowFragments);
        }
    }

    /// <summary>
    /// OnShadowFragmentsChanged 이벤트가 호출할 함수
    /// </summary>
    /// <param name="newAmount">새로운 재화 개수</param>
    private void UpdateFragmentText(int newAmount)
    {
        if (fragmentText != null)
        {
            // 정수(int)를 문자열(string)로 변환하여 텍스트에 표시
            fragmentText.text = newAmount.ToString();
        }
    }
}