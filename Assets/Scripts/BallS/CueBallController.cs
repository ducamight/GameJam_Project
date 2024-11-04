using UnityEngine;
using UnityEngine.UI;

public class CueBallController : MonoBehaviour
{
    public bool canShoot = true; // Biến này sẽ được GameManager quản lý
    public bool isAimlineEnable = true;
    public GameObject arrow; // Mũi tên sẽ là con của bi cái (CueBall)
    public float maxPower = 10f; // Giới hạn lực kéo tối đa
    public float dragSensitivity = 0.5f; // Hệ số điều chỉnh để đạt lực tối đa nhanh hơn

    public AudioSource audioSource;
    public AudioClip audioClip;

    private Vector2 startDragPosition; // Vị trí bắt đầu kéo chuột
    private bool isDragging = false;
    private Rigidbody2D cueBallRigidbody;

    // Thêm slider để hiển thị lực bắn
    public Slider powerSlider; // Thanh hiển thị lực bắn

    // Thêm LineRenderer để hiển thị Aimline
    public LineRenderer aimLine; // LineRenderer để vẽ đường nhắm

    // Tham chiếu tới điểm spawn của aimline
    public Transform aimlineSpawnPoint; // Điểm bắt đầu aimline

    public float maxAimLineLength = 10f; // Độ dài tối đa của aimline
    public LayerMask obstacleMask; // Layer của các vật cản (ví dụ: tường, bi khác)

    void Start()
    {
        cueBallRigidbody = GetComponent<Rigidbody2D>();
        arrow.SetActive(false); // Ẩn mũi tên khi không kéo chuột

        if (powerSlider != null)
        {
            powerSlider.gameObject.SetActive(false); // Ẩn thanh lực bắn khi không kéo chuột
        }
    }

    void Update()
    {
        // Kiểm tra nếu có thể bắn bi cái
        if (!canShoot) return;

        if (Input.GetMouseButtonDown(0) && canShoot == true)
        {
            StartDrag();
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Dragging();
            if (Input.GetMouseButtonDown(1) && isDragging)
            {
                CancelDrag();
            }
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            EndDrag();
        }
    }

    void StartDrag()
    {
        startDragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        isDragging = true;
        arrow.SetActive(true); // Hiển thị mũi tên khi kéo chuột

        // Hiển thị thanh lực bắn
        if (powerSlider != null)
        {
            powerSlider.gameObject.SetActive(true);
            powerSlider.value = 0; // Đặt giá trị ban đầu
        }

        // Hiển thị aimline khi bắt đầu kéo
        if (aimLine != null)
        {
            aimLine.positionCount = 2; // Đặt số điểm là 2 để hiển thị aimline
        }
    }

    void Dragging()
    {
        Vector2 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = startDragPosition - currentMousePosition;

        // Giới hạn khoảng cách kéo của người chơi bằng cách điều chỉnh với hệ số dragSensitivity
        float adjustedDistance = direction.magnitude * dragSensitivity;

        // Xoay CueBall theo hướng ngược lại của hướng kéo chuột
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 270)); // +270 để xoay ngược lại

        // Tính toán lực bắn dựa trên khoảng cách kéo chuột sau khi điều chỉnh với dragSensitivity
        float power = Mathf.Clamp(adjustedDistance, 0, maxPower);

        // Cập nhật giá trị của thanh lực bắn
        if (powerSlider != null)
        {
            powerSlider.value = power / maxPower; // Chuyển đổi giá trị lực bắn thành giá trị 0-1
        }

        // Cập nhật vị trí và hướng của aimline
        if (aimLine != null && aimlineSpawnPoint != null)
        {
            Vector2 spawnPosition = aimlineSpawnPoint.position;
            Vector2 aimDirection = direction.normalized;

            // Độ dài của aimline dựa trên lực kéo
            float aimLineLength = Mathf.Lerp(1f, maxAimLineLength, power / maxPower); // Điều chỉnh độ dài theo lực

            // Hiển thị aimline và kiểm tra va chạm
            if (isAimlineEnable){
                ShowAimlineWithObstacles(spawnPosition, aimDirection, aimLineLength);
            }
        }
    }

    void EndDrag()
    {
        if (audioSource != null && audioClip != null)
        {
            audioSource.PlayOneShot(audioClip);
        }
        Vector2 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = startDragPosition - currentMousePosition;

        // Giới hạn khoảng cách kéo của người chơi bằng cách điều chỉnh với hệ số dragSensitivity
        float adjustedDistance = direction.magnitude * dragSensitivity;

        // Tính toán lực bắn dựa trên khoảng cách kéo chuột sau khi điều chỉnh với dragSensitivity
        float power = Mathf.Clamp(adjustedDistance, 0, maxPower);
        Vector2 force = direction.normalized * power * 10f; // Điều chỉnh lực theo nhu cầu

        // Áp dụng lực cho bi cái
        cueBallRigidbody.AddForce(force, ForceMode2D.Impulse);

        // Ẩn mũi tên và thanh lực bắn
        arrow.SetActive(false);
        isDragging = false;

        if (powerSlider != null)
        {
            powerSlider.gameObject.SetActive(false); // Ẩn thanh lực bắn sau khi bắn
        }

        if (aimLine != null)
        {
            aimLine.positionCount = 0; // Ẩn aimline sau khi bắn
        }
        
        // Sau khi bắn, báo cho GameManager biết rằng bi cái đã được bắn
        GameManager.Instance.OnCueBallShot();

        // Vô hiệu hóa bắn cho đến khi tất cả các viên bi dừng lại
        GameManager.Instance.DisableCueBallControl();
    }

    // Hàm hiển thị aimline, dừng lại khi gặp vật cản
    void ShowAimlineWithObstacles(Vector2 startPosition, Vector2 direction, float length)
    {
        if (aimLine == null) return;

        
        // Tạo raycast để kiểm tra va chạm
        RaycastHit2D hit = Physics2D.Raycast(startPosition, direction, length, obstacleMask);

        // Đặt lại vị trí của LineRenderer với 2 điểm
        aimLine.positionCount = 2;
        aimLine.SetPosition(0, startPosition);

        if (hit.collider != null)
        {
            // Nếu raycast chạm vào vật cản, đặt điểm cuối tại vị trí va chạm
            aimLine.SetPosition(1, hit.point);
        }
        else
        {
            // Nếu không có va chạm, tiếp tục hiển thị aimline theo chiều dài được tính toán
            Vector2 endPosition = startPosition + (direction * length);
            aimLine.SetPosition(1, endPosition);
        }
    }

    // Hàm hủy quá trình kéo khi bấm chuột phải
    void CancelDrag()
    {
        isDragging = false;
        arrow.SetActive(false); // Ẩn mũi tên

        if (powerSlider != null)
        {
            powerSlider.gameObject.SetActive(false); // Ẩn thanh lực bắn
        }

        if (aimLine != null)
        {
            aimLine.positionCount = 0; // Ẩn aimline khi hủy
        }

        Debug.Log("Quá trình kéo lực đã bị hủy.");
    }
}
