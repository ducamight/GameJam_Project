using System.Collections;
using UnityEngine;

public class SlowMotionEffect : MonoBehaviour
{
    public float slowMotionFactor = 0.2f; // Tốc độ thời gian khi slow motion (giảm thời gian)
    public float slowMotionDurationTime = 2f; // Thời gian slow motion diễn ra
    public float zoomSpeed = 2f; // Tốc độ zoom của camera
    public float zoomAmount = 5f; // Mức độ zoom vào viên bi
    private bool isZooming = false; // Kiểm tra nếu đang zoom
    private Camera mainCamera; // Tham chiếu tới camera chính
    private float defaultCameraSize; // Kích thước mặc định của camera
    private Vector3 defaultCameraPosition; // Vị trí mặc định của camera

    void Start()
    {
        // Lấy tham chiếu tới camera chính
        mainCamera = Camera.main;
        defaultCameraSize = mainCamera.orthographicSize; // Lưu kích thước mặc định của camera
        defaultCameraPosition = mainCamera.transform.position; // Lưu vị trí mặc định của camera
    }

    // Hàm này được gọi khi một Collider2D kích hoạt Trigger với slowMotionZone
    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra nếu đối tượng va chạm có tag chứa từ "Ball"
        if (IsBallTag(other.tag))
        {
            StartCoroutine(TriggerSlowMotionAndZoom(other.transform)); // Bắt đầu slow motion và zoom vào bi
        }
    }

    // Kiểm tra nếu tag của đối tượng có chứa từ "Ball"
    bool IsBallTag(string tag)
    {
        // Kiểm tra nếu tag có chứa từ "Ball"
        return tag.Contains("Ball");
    }

    // Hàm xử lý SlowMotion và Zoom
    IEnumerator TriggerSlowMotionAndZoom(Transform ballTransform)
    {
        if (isZooming) yield break; // Nếu đang zoom, không cho phép thực hiện lại
        isZooming = true;

        // Bắt đầu slow motion
        Time.timeScale = slowMotionFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f; // Đảm bảo vật lý hoạt động bình thường khi slow motion

        float elapsedTime = 0f;
        Vector3 targetCameraPosition = new Vector3(ballTransform.position.x, ballTransform.position.y, defaultCameraPosition.z); // Tính toán vị trí mới của camera

        // Zoom camera vào viên bi trong thời gian slowMotionDurationTime
        while (elapsedTime < slowMotionDurationTime)
        {
            elapsedTime += Time.unscaledDeltaTime; // Tính thời gian trôi qua mà không bị ảnh hưởng bởi Time.timeScale

            // Zoom camera gần viên bi
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, zoomAmount, zoomSpeed * Time.unscaledDeltaTime);

            // Di chuyển camera đến vị trí của viên bi
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetCameraPosition, zoomSpeed * Time.unscaledDeltaTime);

            yield return null;
        }

        // Đợi thêm một khoảng thời gian để slow motion tiếp tục (bằng slowMotionDurationTime)
        yield return new WaitForSecondsRealtime(slowMotionDurationTime);

        // Khôi phục thời gian và camera về trạng thái ban đầu
        ResetTimeAndZoom();
    }

    // Hàm khôi phục thời gian và camera về trạng thái ban đầu
    void ResetTimeAndZoom()
    {
        isZooming = false;

        // Khôi phục thời gian bình thường
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        // Khôi phục camera về trạng thái bình thường
        StartCoroutine(ZoomOutCamera());
    }

    // Zoom camera trở về vị trí và kích thước ban đầu
    IEnumerator ZoomOutCamera()
    {
        while (mainCamera.orthographicSize < defaultCameraSize || mainCamera.transform.position != defaultCameraPosition)
        {
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, defaultCameraSize, zoomSpeed * Time.unscaledDeltaTime);
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, defaultCameraPosition, zoomSpeed * Time.unscaledDeltaTime);
            yield return null;
        }
    }
}
