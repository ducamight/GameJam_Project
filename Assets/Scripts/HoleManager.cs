using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleManager : MonoBehaviour
{
    public List<GameObject> allHoles; // Danh sách chứa tất cả các Hole
    private List<GameObject> selectedHoles = new List<GameObject>(); // Danh sách chứa các Hole được chọn

    public int numberOfHolesToActivate = 3; // Số lượng Hole sẽ xuất hiện ngẫu nhiên

    void Start()
    {
        RandomizeHoles(); // Gọi hàm random khi scene bắt đầu
    }

    // Hàm random các Hole xuất hiện trên màn chơi
    public void RandomizeHoles()
    {
        // Đảm bảo số lượng hole để kích hoạt không vượt quá tổng số hole
        if (numberOfHolesToActivate > allHoles.Count)
        {
            Debug.LogError("Số lượng Hole được chọn lớn hơn số lượng Hole có sẵn!");
            return;
        }

        // Tắt tất cả các Hole trước
        foreach (GameObject hole in allHoles)
        {
            hole.SetActive(false);
        }

        // Tạo danh sách các Hole chưa được chọn
        List<GameObject> remainingHoles = new List<GameObject>(allHoles);

        // Random và chọn các Hole
        for (int i = 0; i < numberOfHolesToActivate; i++)
        {
            int randomIndex = Random.Range(0, remainingHoles.Count); // Random vị trí trong danh sách còn lại
            GameObject selectedHole = remainingHoles[randomIndex]; // Lấy Hole tại vị trí random
            selectedHole.SetActive(true); // Hiển thị Hole này
            selectedHoles.Add(selectedHole); // Thêm vào danh sách các Hole đã được chọn
            remainingHoles.RemoveAt(randomIndex); // Loại bỏ Hole đã chọn khỏi danh sách còn lại
        }
    }
}
