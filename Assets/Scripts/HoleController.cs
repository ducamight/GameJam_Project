using UnityEngine;

public class HoleController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Khi viên bi rơi vào lỗ, gọi GameManager để xử lý
        GameManager.Instance.OnBallEnterHole(collision.gameObject, this);
    }
}
