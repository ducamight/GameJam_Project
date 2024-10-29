using UnityEngine;

using System.Collections;

public class HoleController : MonoBehaviour
{
    public Animator anim;

    private Camera mainCamera; // Tham chiếu tới camera chính
    public float shakeDuration = 0.5f; // Thời gian lắc camera
    public float shakeMagnitude = 0.2f; // Độ mạnh của hiệu ứng lắc

    private Vector3 originalCameraPosition; // Vị trí ban đầu của camera
    private bool isShaking = false; // Biến kiểm tra trạng thái lắc camera

    private void Start()
    {
        mainCamera = FindAnyObjectByType<Camera>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Khi viên bi rơi vào lỗ, gọi GameManager để xử lý
        GameManager.Instance.OnBallEnterHole(collision.gameObject, this);
    }

    public void HoleTriggerAnimation()
    {
        anim.SetBool("Eat", false);

    }

    // Hàm gọi từ Animator để thực hiện hiệu ứng lắc camera
    public void ShakeCameraEffect()
    {
        if (!isShaking)
        {
            StartCoroutine(ShakeCamera());
        }
    }

    // Coroutine để lắc camera
    private IEnumerator ShakeCamera()
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;

            // Tạo vị trí mới của camera dựa trên vị trí ban đầu và độ lắc
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;

            mainCamera.transform.position = new Vector3(originalCameraPosition.x + offsetX, originalCameraPosition.y + offsetY, originalCameraPosition.z);

            yield return null; // Chờ một frame
        }

        // Đặt lại vị trí camera về ban đầu sau khi lắc xong
        mainCamera.transform.position = originalCameraPosition;
        isShaking = false;
    }
}
