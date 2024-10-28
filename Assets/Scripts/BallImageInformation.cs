using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Để làm việc với sự kiện UI
using TMPro;

public class BallImageInformation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject infoPanel; // Tham chiếu tới panel chứa thông tin về Ball
    public TextMeshProUGUI infoText; // Tham chiếu tới Text trên panel để hiển thị thông tin
    public string ballInfo; // Thông tin của Ball (có thể là màu sắc, tên, điểm, v.v.)

    private bool isPointerOver = false; // Kiểm tra nếu chuột đang nằm trên BallImage hoặc panel

    void Start()
    {
        infoPanel.SetActive(false); // Ban đầu ẩn panel thông tin
    }

    void Update()
    {
        // Kiểm tra nếu chuột không còn nằm trên BallImage và panel thì ẩn panel đi
        if (!isPointerOver && !RectTransformUtility.RectangleContainsScreenPoint(infoPanel.GetComponent<RectTransform>(), Input.mousePosition))
        {
            HideInfoPanel();
        }
    }

    // Hàm này được gọi khi người dùng rê chuột vào BallImage
    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
        ShowInfoPanel();
    }

    // Hàm này được gọi khi người dùng rê chuột ra khỏi BallImage
    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
    }

    // Hiển thị panel chứa thông tin về Ball
    void ShowInfoPanel()
    {
        infoPanel.SetActive(true); // Hiển thị panel
        infoText.text = ballInfo; // Cập nhật nội dung của panel bằng thông tin của Ball
    }

    // Ẩn panel chứa thông tin
    void HideInfoPanel()
    {
        infoPanel.SetActive(false); // Ẩn panel
    }
}
