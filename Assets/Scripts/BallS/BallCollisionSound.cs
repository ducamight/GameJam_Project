using UnityEngine;

public class BallCollisionSound : MonoBehaviour
{
    public AudioSource audioSource; // Tham chiếu tới AudioSource
    public AudioClip collisionSound; // Âm thanh va chạm giữa các viên bi

    void Start()
    {
        // Đảm bảo có AudioSource được gắn
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // Tự động thêm AudioSource nếu chưa gắn từ Inspector
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Kiểm tra nếu đối tượng va chạm có chứa từ "Ball" trong tag
        if (collision.gameObject.tag.Contains("Ball") || collision.gameObject.CompareTag("Table"))
        {
            // Phát âm thanh khi va chạm
            if (audioSource != null && collisionSound != null)
            {
                audioSource.PlayOneShot(collisionSound);
            }
        }
    }
}
