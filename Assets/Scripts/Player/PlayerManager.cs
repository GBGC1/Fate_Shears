using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    public StatManager PlayerStat { get; private set; }
    public Transform PlayerTransform { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 싱글톤 인스턴스 설정
        Instance = this;

        // 씬 전환 시에도 파괴되지 않도록 설정
        DontDestroyOnLoad(gameObject);

        // 참조 설정
        PlayerStat = GetComponent<StatManager>();
        PlayerTransform = transform;
    }
}
