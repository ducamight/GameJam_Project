using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }
    private void Awake()
    {
        // Đảm bảo rằng SceneManager là Singleton (chỉ có 1 instance tồn tại trong game)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    // Hàm gọi khi người chơi thua và cần chơi lại
    public void RestartGame()
    {
        // Tải lại cảnh hiện tại
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneName);
    }
}
