using UnityEngine;

/// <summary>
/// 메인 메뉴 UI 이벤트를 처리하는 클래스
/// 게임 시작, 설정, 종료 버튼 처리
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SceneLoader sceneLoader;   // 씬 전환을 위한 컴포넌트 참조
    [SerializeField] private GameObject optionPanel;

    // 게임 시작 버튼 클릭 시 Basement3 씬으로 전환
    public void OnClickStart()
    {
        Debug.Log("게임 시작");
        sceneLoader.LoadBasement3();
    }

    // 옵션 버튼 클릭 시 옵션 패널 활성화
    public void OnClickOptions()
    {
        Debug.Log("옵션 활성화");
        optionPanel.SetActive(true);
    }

    // 옵션 패널의 X 버튼 클릭 시 옵션 패널 비활성화
    public void OnClickBack()
    {
        optionPanel.SetActive(false);
    }

    // 게임 종료 버튼 클릭 시 애플리케이션 종료 (유니티 상에서는 실행 종료)
    public void OnClickExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
