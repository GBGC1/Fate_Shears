using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 씬 전환을 관리하는 클래스
/// 씬 전환 시 필요한 기능을 정의하고 중복 로드 방지
/// </summary>
public class SceneLoader : MonoBehaviour
{
    private static SceneLoader instance;    // 싱글톤 SceneLoader 인스턴스 저장

    private void Awake()
    {
        // 이미 인스턴스가 존재하면 새로운 SceneLoader 오브젝트 제거
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 인스턴스가 없을 경우 이 오브젝트를 인스턴스로 설정
        instance = this;

        // 씬 전환 시에도 오브젝트가 파괴되지 않도록 설정
        DontDestroyOnLoad(gameObject);
    }

    #region Scene Load Methods
    public void LoadTestScene()
    {
        SceneManager.LoadScene("TestScene");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadBasement3()
    {
        SceneManager.LoadScene("Basement3");
    }

    public void LoadBasement2()
    {
        SceneManager.LoadScene("Basement2");
    }

    public void LoadBasement1()
    {
        SceneManager.LoadScene("Basement1");
    }
    #endregion
}
