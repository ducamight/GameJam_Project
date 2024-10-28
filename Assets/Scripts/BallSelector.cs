using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallSelector : MonoBehaviour
{
    public GameObject[] balls; // Các viên bi trong game
    public Button[] colorButtons; // Các nút để chọn màu (lá bài màu)
    public BallImageManager ballImageManager;

    private string selectedTag; // Tag của viên bi mà người chơi đã chọn
    public GameObject selectedBall; // Viên bi được chọn

    void Start()
    {
        // Gán sự kiện click cho từng nút màu (lá bài)
        colorButtons[0].onClick.AddListener(() => SelectBallByTag("RedBall"));
        colorButtons[1].onClick.AddListener(() => SelectBallByTag("BlueBall"));
        colorButtons[2].onClick.AddListener(() => SelectBallByTag("YellowBall"));
        colorButtons[3].onClick.AddListener(() => SelectBallByTag("GreenBall"));
        colorButtons[4].onClick.AddListener(() => SelectBallByTag("OrangeBall"));
        colorButtons[5].onClick.AddListener(() => SelectBallByTag("PurpleBall"));
    }

    // Hàm ẩn tất cả các button
    public void HideAllButtons()
    {
        foreach (Button button in colorButtons)
        {
            button.gameObject.SetActive(false); // Ẩn từng button
        }
    }

    // Hàm hiển thị tất cả các button
    public void ShowAllButtons()
    {
        foreach (Button button in colorButtons)
        {
            button.gameObject.SetActive(true); // Hiển thị từng button
        }
    }

    // Hàm hiển thị tất cả các button trừ những bi đã rơi vào lỗ
    public void ShowAllButtonsExcept(HashSet<string> ballsInHoles)
    {
        for (int i = 0; i < colorButtons.Length; i++)
        {
            string ballTag = balls[i].tag; // Lấy tag của viên bi tương ứng với button

            if (ballsInHoles.Contains(ballTag))
            {
                colorButtons[i].gameObject.SetActive(false); // Ẩn button nếu viên bi đã rơi vào lỗ
            }
            else
            {
                colorButtons[i].gameObject.SetActive(true); // Hiển thị button nếu viên bi chưa rơi vào lỗ
            }
        }
    }

    // Hàm xử lý khi chọn viên bi thông qua Tag
    public void SelectBallByTag(string tag)
    {
        selectedTag = tag; // Lưu Tag đã chọn
        selectedBall = GetBallByTag(tag); // Lấy viên bi tương ứng với Tag đã chọn

        if (selectedBall != null)
        {
            Debug.Log("Người chơi đã chọn viên bi với tag: " + tag);
            GameManager.Instance.SelectBallForNextShot(); // Báo cho GameManager biết người chơi đã chọn bi
            ballImageManager.ShowImageByTag(tag); // Hiển thị hình ảnh tương ứng với viên bi đã chọn
        }
        else
        {
            Debug.LogWarning("Không có viên bi nào khớp với tag: " + tag);
        }
    }

    // Hàm lấy viên bi tương ứng với Tag đã chọn
    GameObject GetBallByTag(string tag)
    {
        foreach (GameObject ball in balls)
        {
            if (ball.CompareTag(tag))
            {
                return ball; // Trả về viên bi có Tag đã chọn
            }
        }
        return null;
    }

    // Hàm lấy viên bi đã chọn (để sử dụng cho HoleController)
    public GameObject GetSelectedBall()
    {
        return selectedBall;
    }

    // Hàm lấy Tag đã chọn (cho HoleController)
    public string GetSelectedTag()
    {
        return selectedTag;
    }

    // Hàm ẩn button dựa trên tag của viên bi
    public void HideButtonByTag(string tag)
    {
        for (int i = 0; i < colorButtons.Length; i++)
        {
            if (balls[i].CompareTag(tag))
            {
                colorButtons[i].gameObject.SetActive(false); // Ẩn button tương ứng với viên bi
                break;
            }
        }
    }
}
