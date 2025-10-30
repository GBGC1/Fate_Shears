using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 메인 메뉴의 UI에 페이드 효과를 적용하는 클래스
/// 로고에서 메인 메뉴 버튼으로 자연스럽게 전환
/// </summary>
public class FadeEffectController : MonoBehaviour
{
    [Header("Main Menu Component")]
    [SerializeField] private GameObject logoImage;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject backgroundImage;

    [Header("Fade Effect Settings")]
    [SerializeField] private float logoDisplayTime = 2f;  // 로고 유지 시간
    [SerializeField] private float fadeDuration = 1f;     // 페이드 효과 지속시간

    private Image logo;
    private CanvasGroup menu;
    private Image background;

    void Start()
    {
        logo = logoImage.GetComponent<Image>();
        menu = menuPanel.GetComponent<CanvasGroup>();
        background = backgroundImage.GetComponent<Image>();

        // 페이드 효과를 적용할 컴포넌트 초기 상태 설정
        if (logo != null)
        {
            Color c = logo.color;
            c.a = 1f;
            logo.color = c;
        }

        if (menu != null)
        {
            menu.interactable = false;
        }

        if (background != null)
        {
            Color c = background.color;
            c.a = 1f;
            background.color = c;
        }

        StartCoroutine(StartIntro());
    }

    // 게임 인트로 시작 코루틴 함수
    IEnumerator StartIntro()
    {
        // 로고 유지 시간만큼 대기
        yield return new WaitForSeconds(logoDisplayTime);

        // 로고 페이드 아웃
        yield return StartCoroutine(PlayFadeEffects(logo, 1f, 0f, fadeDuration));
        logo.raycastTarget = false;

        // 배경색 페이드 아웃 (-> 메뉴 패널 페이드 인 효과)
        yield return StartCoroutine(PlayFadeEffects(background, 1f, 0f, fadeDuration));
        background.raycastTarget = false;

        // 메뉴 패널 상호작용 활성화
        menu.interactable = true;
    }

    // 페이드 효과 실행 코루틴 함수
    IEnumerator PlayFadeEffects(Component target, float from, float to, float duration)
    {
        float elapsed = 0f;

        if (target.GetType() == typeof(Image))
        {
            Image image = (Image)target;
            Color c = image.color;

            while (elapsed < duration)
            {
                c.a = Mathf.Lerp(from, to, elapsed / duration);
                image.color = c;
                elapsed += Time.deltaTime;
                yield return null;
            }
            c.a = to;
            image.color = c;
        }
    }
}
