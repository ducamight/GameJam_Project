using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public BallSelector ballSelector;
    public List<HoleController> holeControllers; // Danh sách các lỗ
    public List<Rigidbody2D> ballRigidbodies; // Danh sách Rigidbody2D của các viên bi
    public CueBallController cueBallController; // Tham chiếu tới CueBallController
    public BallImageManager ballImageManager;

    private bool isFirstShot = true;
    private bool hasBallSelected = false;
    private bool ballFellIntoHole = false; // Biến theo dõi nếu có viên bi nào đã rơi vào lỗ
    private bool hasCueBallShot = false; // Biến theo dõi nếu bi cái đã được bắn
    private bool turnEnded = false; // Biến theo dõi nếu lượt bắn đã hoàn tất
    public int totalBalls;
    private int ballInHole;

    private Image currentVisibleImage = null; // Hình ảnh hiện đang hiển thị
    private HashSet<string> ballsInHoles = new HashSet<string>(); // Lưu trạng thái các bi đã rơi vào lỗ

    public UIManager uiManager; // Tham chiếu đến UIManager

    void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        ballInHole = 0;
        totalBalls = ballRigidbodies.Count -1;
        StartFirstShot();
    }

    void Update()
    {
        if (CheckIfAllBallsInHole())
        {
            PlayerWins(); // Gọi hàm thông báo người chơi thắng
            uiManager.ShowWinPopup();
        }
        // Chỉ kiểm tra nếu tất cả các viên bi đã dừng sau khi bi cái đã được bắn trong lượt đầu tiên
        if (isFirstShot && hasCueBallShot)
        {
            if (CheckIfAllBallsStopped())
            {
                Debug.Log("Phát bắn đầu tiên hoàn tất!");
                EnableCueBallControl(); // Cho phép bắn lại khi tất cả các bi đã dừng
                EndFirstShot();
            }
        }
        else if (!isFirstShot && hasBallSelected && hasCueBallShot)  // Kiểm tra khi người chơi đã chọn bi và bắn
        {
            // Chỉ kiểm tra trạng thái bi sau khi bi cái đã được bắn
            if (CheckIfAllBallsStopped() && !turnEnded)
            {
                // Kiểm tra nếu tất cả bi đã dừng và không có viên bi nào rơi vào lỗ
                if (!ballFellIntoHole)
                {
                    Debug.Log("Người chơi đã thua vì không có viên bi nào rơi vào lỗ.");
                    StartCoroutine(GameOver());
                }
                else
                {
                    Debug.Log("Lượt bắn hoàn tất!");
                    ShowBallSelectionButtons(); // Hiển thị các button chọn bi sau khi lượt hoàn tất
                }

                DisableCueBallControl(); // Vô hiệu hóa bắn cho đến khi người chơi chọn bi
                turnEnded = true; // Đánh dấu rằng lượt đã hoàn tất
            }
        }
    }

    public bool CheckIfAllBallsInHole()
    {
        // Kiểm tra nếu số lượng bi đã rơi vào hố bằng tổng số bi trong game
        return ballInHole == totalBalls;
    }

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1f);
        uiManager.ShowGameOverPopup();
    }

    // Bắt đầu phát bắn đầu tiên
    void StartFirstShot()
    {
        Debug.Log("Phát bắn đầu tiên, không cần chọn bi.");
        isFirstShot = true;
        ballSelector.enabled = false;
        hasCueBallShot = false; // Reset trạng thái bắn bi cái
        EnableCueBallControl(); // Đảm bảo người chơi có thể bắn bi cái trong phát đầu tiên
        HideBallSelectionButtons(); // Ẩn các button chọn bi khi người chơi chuẩn bị bắn
    }

    // Kết thúc phát bắn đầu tiên
    void EndFirstShot()
    {
        isFirstShot = false;
        ballSelector.enabled = true;
        turnEnded = false; // Reset trạng thái lượt
        hasBallSelected = false; // Người chơi chưa chọn bi trong lượt tiếp theo
        hasCueBallShot = false; // Đặt lại trạng thái bi chưa bắn
        Debug.Log("Từ đây trở đi, người chơi phải chọn bi trước khi bắn.");
        DisableCueBallControl(); // Vô hiệu hóa việc bắn cho đến khi người chơi chọn bi
        ShowBallSelectionButtons(); // Hiển thị các button chọn bi để người chơi chọn bi trước lượt bắn
    }

    // Gọi hàm này khi người chơi đã chọn bi để chuẩn bị bắn
    public void SelectBallForNextShot()
    {
        hasBallSelected = true;
        ballFellIntoHole = false;
        turnEnded = false; // Reset trạng thái lượt mới
        EnableCueBallControl(); // Cho phép bắn sau khi người chơi đã chọn bi
        HideBallSelectionButtons(); // Ẩn các button sau khi người chơi chọn bi
        Debug.Log("Người chơi đã chọn bi cho lượt bắn tiếp theo.");
    }

    // Reset trạng thái cho lượt bắn tiếp theo
    void ResetTurn()
    {
            hasBallSelected = false; // Đặt lại trạng thái bi chưa được chọn
            ballFellIntoHole = false; // Đặt lại trạng thái rơi bi
            hasCueBallShot = false; // Đặt lại trạng thái bắn bi cái
            turnEnded = false; // Reset trạng thái lượt
            Debug.Log("Chọn bi cho lượt bắn tiếp theo.");
            DisableCueBallControl(); // Tắt tính năng bắn bi cho đến khi người chơi chọn bi
            ShowBallSelectionButtons(); // Hiển thị các button chọn bi để người chơi có thể chọn 
    }

    // Kiểm tra nếu tất cả các viên bi đã dừng lại
    public bool CheckIfAllBallsStopped()
    {
        foreach (Rigidbody2D rb in ballRigidbodies)
        {
            if (rb.velocity.magnitude > 0.01f)
            {
                return false; // Có ít nhất một viên bi vẫn còn di chuyển
            }
        }
        return true; // Tất cả viên bi đã dừng
    }

    // Vô hiệu hóa bắn CueBall cho đến khi người chơi chọn bi
    public void DisableCueBallControl()
    {
        if (cueBallController != null)
        {
            cueBallController.canShoot = false; // Ngăn không cho bắn
        }
    }

    // Kích hoạt lại việc bắn CueBall
    public void EnableCueBallControl()
    {
        if (cueBallController != null)
        {
            cueBallController.canShoot = true; // Cho phép bắn lại
        }
    }

    // Gọi từ CueBallController khi bi cái đã được bắn
    public void OnCueBallShot()
    {
        hasCueBallShot = true;
        DisableCueBallControl(); // Vô hiệu hóa bắn cho đến khi tất cả các bi dừng lại
    }

    // Gọi từ HoleController khi một viên bi rơi vào lỗ
    public void OnBallEnterHole(GameObject ball, HoleController hole)
    {
        ballFellIntoHole = true;
        ball.SetActive(false); // Ẩn viên bi khi rơi vào lỗ


        // Lưu lại bi đã rơi vào lỗ để không hiển thị lại button chọn
        ballsInHoles.Add(ball.tag);

        // Trong phát bắn đầu tiên, không cần kiểm tra bi đã chọn
        if (ball.CompareTag("CueBall"))
        {
            Debug.Log("Người chơi đã thua");
            StartCoroutine(GameOver());
        }

        if (isFirstShot)
        {
            Debug.Log("Bi " + ball.name + " đã rơi vào lỗ " + hole.name + " trong phát bắn đầu tiên.");
            ballInHole++;
        }
        else
        {
            // Sau lượt bắn đầu tiên, kiểm tra bi đã chọn
            GameObject selectedBall = ballSelector.GetSelectedBall();

            if (ball == selectedBall)
            {
                Debug.Log("Chính xác! Bi " + ballSelector.GetSelectedTag() + " đã rơi vào lỗ " + hole.name);
                ballInHole++;
                ResetTurn();
            }
            else
            {
                Debug.Log("Người chơi đã thua, viên bi không trùng với viên bi đã chọn!");
                StartCoroutine(GameOver());
            }
        }
        if (CheckIfAllBallsInHole())
        {
            PlayerWins();
        }

        // Ẩn nút tương ứng với viên bi đã rơi
        ballSelector.HideButtonByTag(ball.tag);

        // Ẩn hình ảnh tương ứng với viên bi đã rơi
        HideImageByTag(ball.tag);
    }
    public void PlayerWins()
    {
        Debug.Log("Người chơi đã thắng!");
        // Hiển thị popup hoặc xử lý thắng cuộc ở đây
        // Có thể gọi một UIManager để hiển thị thông báo chiến thắng
    }

    // Hàm ẩn hình ảnh tương ứng với viên bi
    public void HideImageByTag(string tag)
    {
        for (int i = 0; i < ballRigidbodies.Count; i++)
        {
            if (ballRigidbodies[i].gameObject.CompareTag(tag))
            {
                ballImageManager.ballImages[i].gameObject.SetActive(false); // Ẩn hình ảnh tương ứng
                break;
            }
        }
    }

    // Hàm ẩn hình ảnh đang hiển thị
    public void HideCurrentVisibleImage()
    {
        if (currentVisibleImage != null)
        {
            currentVisibleImage.gameObject.SetActive(false); // Ẩn hình ảnh hiện tại
            Debug.Log("Đã ẩn hình ảnh hiện tại");
            currentVisibleImage = null; // Đặt lại giá trị
        }
    }

    // Hàm hiển thị hình ảnh và lưu lại hình ảnh đã hiển thị
    public void ShowImageByTag(string tag)
    {
        HideCurrentVisibleImage(); // Ẩn hình ảnh hiện tại trước

        for (int i = 0; i < ballRigidbodies.Count; i++)
        {
            if (ballRigidbodies[i].gameObject.CompareTag(tag))
            {
                ballImageManager.ballImages[i].gameObject.SetActive(true); // Hiển thị hình ảnh tương ứng
                currentVisibleImage = ballImageManager.ballImages[i]; // Lưu lại hình ảnh hiện đang hiển thị
                Debug.Log("Hiển thị hình ảnh của bi: " + tag);
                break;
            }
        }
    }

    // Hàm để ẩn tất cả các button chọn bi
    public void HideBallSelectionButtons()
    {
        ballSelector.HideAllButtons(); // Ẩn tất cả các button chọn bi
    }

    // Hàm để hiển thị tất cả các button chọn bi nhưng bỏ qua bi đã rơi vào lỗ
    public void ShowBallSelectionButtons()
    {
        ballSelector.ShowAllButtonsExcept(ballsInHoles); // Hiển thị tất cả các button trừ những bi đã rơi vào lỗ
    }
}
