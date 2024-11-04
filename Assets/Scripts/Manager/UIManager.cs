using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject gameOverPanel; // Tham chiếu đến Popup Game Over
    //public Button gameOverRestartButton; // Nút Restart
    //public Button winRestartButton;
    public GameObject winPanel; // Tham chiếu tới Popup Win
    public GameObject selectLevelPanel; //Tham chiếu tới slect level panel
    private void Start()
    {
        // Ẩn popup khi game bắt đầu
        gameOverPanel.SetActive(false);
        winPanel.SetActive(false);  
        selectLevelPanel.SetActive(false);

        // Gán sự kiện cho nút Restart
        //gameOverRestartButton.onClick.AddListener(OnRestartButtonClick);
        //winRestartButton.onClick.AddListener(OnRestartButtonClick);

    }

    // Hiển thị popup khi người chơi thua
    public void ShowGameOverPopup()
    {
        gameOverPanel.GetComponent<GameOverPanel>().PlaySound();
        gameOverPanel.SetActive(true); // Hiển thị popup Game Over
    }

    public void ShowWinPopup()
    {
        winPanel.GetComponent<WinPanel>().PlaySound();
        winPanel.SetActive(true);
    }

    // Xử lý sự kiện khi người chơi nhấn nút Restart
    public void OnRestartButtonClick()
    {
        SceneManager.Instance.RestartGame(); // Gọi hàm restart từ SceneManager
    }
    public void OnSelectLevelButtonClick()
    {
        selectLevelPanel.SetActive(true);
    }
}
