using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 피로도 시스템의 시각적 효과(시야 축소/차단)를 관리하는 클래스
/// UI Image의 Alpha 값을 조절하여 투명도를 제어합니다.
/// </summary>
public class FatigueEffectVisual : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private Image overlayImage;

    [Header("Reference")]
    [SerializeField] private FatigueSystem fatigueSystem;

    [Header("Effect Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float lightDarknessAlpha = 0.5f;   // 시야 축소를 위한 알파값
    [SerializeField] private float darknessDuration = 3f;       // 시야 차단 지속시간
    
    private Coroutine visionObstructionCoroutine;

    private void Start()
    {
        if (overlayImage == null || fatigueSystem == null)
        {
            //Debug.LogError("필수 컴포넌트가 할당되지 않았습니다.");
            enabled = false;
            return;
        }

        // 투명도를 0으로 설정하여 시작
        SetOverlayAlpha(0f);
    }

    // 시야 축소 효과 해제
    public void RemoveVisionShrink()
    {
        SetOverlayAlpha(0f);
    }

    // 시야 축소 효과 활성화 - 많이 피로함 (90% 이상) 단계에 사용
    public void ActivateVisionShrink()
    {
        // 시야 차단 코루틴이 실행 중이면 무시
        if (visionObstructionCoroutine != null) return;

        float targetAlpha;
        targetAlpha = lightDarknessAlpha;

        // 알파값을 옅은 어둠 값으로 설정
        SetOverlayAlpha(targetAlpha);
    }

    // 시야 차단 효과 활성화 - 피로도 100% 시 사용
    public void ActivateVisionObstruction()
    {
        // 시야 차단 코루틴이 실행 중이라면 중복 실행 방지
        if (visionObstructionCoroutine != null)
        {
            StopCoroutine(visionObstructionCoroutine);
        }

        visionObstructionCoroutine = StartCoroutine(SwitchDarkness());
    }

    // 화면을 완전히 까맣게 했다가 기존의 시야 축소 상태로 복구
    private IEnumerator SwitchDarkness()
    {
        float fadeTime = 0.5f; // 페이드 인/아웃 시간

        // 페이드인 실행 (시야 차단)
        yield return StartCoroutine(FadeOverlay(1.0f, fadeTime));

        // 지속시간만큼 화면을 검게 유지
        yield return new WaitForSeconds(darknessDuration);

        // 페이드아웃 실행 (시야 축소 상태로 복구)
        yield return StartCoroutine(FadeOverlay(lightDarknessAlpha, fadeTime));

        visionObstructionCoroutine = null;

        fatigueSystem.IsExhaustedFinished = true;
        fatigueSystem.IsExhaustedTriggered = false;
    }

    // 알파값을 부드럽게 전환하여 페이드 처리
    private IEnumerator FadeOverlay(float targetAlpha, float duration)
    {
        Color color = overlayImage.color;
        float startAlpha = color.a;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            SetOverlayAlpha(newAlpha);
            yield return null;
        }

        SetOverlayAlpha(targetAlpha);
    }

    // 이미지의 알파값을 설정
    private void SetOverlayAlpha(float alpha)
    {
        if (overlayImage != null)
        {
            Color color = overlayImage.color;
            color.a = alpha;
            overlayImage.color = color;
        }
    }
}
