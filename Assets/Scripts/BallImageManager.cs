using UnityEngine;
using UnityEngine.UI;

public class BallImageManager : MonoBehaviour
{
    public Image[] ballImages; // Danh sách các Image tương ứng với các viên bi
    public GameObject[] balls; // Danh sách các viên bi trong game

    void Start()
    {
        HideAllImages(); // Ẩn tất cả các Image ban đầu
    }

    // Hàm ẩn tất cả các Image
    public void HideAllImages()
    {
        foreach (Image img in ballImages)
        {
            img.gameObject.SetActive(false); // Ẩn Image
        }
    }

    // Hàm hiển thị Image dựa trên Tag của viên bi được chọn
    public void ShowImageByTag(string tag)
    {
        // Ẩn tất cả các Image trước
        HideAllImages();

        // Tìm Image tương ứng với Tag và hiển thị nó
        for (int i = 0; i < balls.Length; i++)
        {
            if (balls[i].CompareTag(tag))
            {
                ballImages[i].gameObject.SetActive(true); // Hiển thị Image tương ứng với viên bi đã chọn
                Debug.Log("Hiển thị hình ảnh của bi: " + tag);
                break;
            }
        }
    }
}
