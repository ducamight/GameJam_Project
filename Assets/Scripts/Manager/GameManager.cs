using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public BallSelector ballSelector;
    public HoleManager holeManager;
    public List<Rigidbody2D> ballRigidbodies; // Danh sách Rigidbody2D của các viên bi
    public CueBallController cueBallController; // Tham chiếu tới CueBallController
    public BallImageManager ballImageManager;
    [SerializeField] private bool isSuperHardLevel = false;

    private bool isFirstShot = true;
    private bool hasBallSelected = false;
    private bool ballFellIntoHole = false; // Biến theo dõi nếu có viên bi nào đã rơi vào lỗ
    private bool hasCueBallShot = false; // Biến theo dõi nếu bi cái đã được bắn
    private bool turnEnded = false; // Biến theo dõi nếu lượt bắn đã hoàn tất
    private int ballInHole;

    private bool gameEnded = false; // Kiểm tra xem game đã kết thúc chưa

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
        StartFirstShot();
    }

    void Update()
    {
        // Kiểm tra xem game có đang kết thúc không để không hiển thị popup nữa
        if (gameEnded) return;

        // Nếu tất cả bi đã rơi vào lỗ và dừng thì người chơi thắng
        if (CheckIfAllBallsInHole() && CheckIfAllBallsStopped())
        {
            PlayerWins();
            uiManager.ShowWinPopup();
            gameEnded = true; // Đánh dấu game đã kết thúc
        }

        // Kiểm tra lượt đầu tiên
        if (isFirstShot && hasCueBallShot)
        {
            if (CheckIfAllBallsStopped())
            {
                Debug.Log("Phát bắn đầu tiên hoàn tất!");
                EnableCueBallControl();
                EndFirstShot();
            }
        }
        // Kiểm tra khi đã chọn bi và bắn xong
        else if (!isFirstShot && hasBallSelected && hasCueBallShot)
        {
            if (CheckIfAllBallsStopped() && !turnEnded)
            {
                if (!ballFellIntoHole)
                {
                    Debug.Log("Người chơi đã thua vì không có viên bi nào rơi vào lỗ.");
                    StartCoroutine(GameOver(0.5f));
                    gameEnded = true; // Đánh dấu game đã kết thúc
                }
                else
                {
                    Debug.Log("Lượt bắn hoàn tất!");
                    ShowBallSelectionButtons(); // Hiển thị button chọn bi
                }

                DisableCueBallControl();
                turnEnded = true;
            }
        }
    }

    // Kiểm tra nếu tất cả bi đã rơi vào lỗ
    public bool CheckIfAllBallsInHole()
    {
        return ballInHole == ballRigidbodies.Count - 1;
    }

    IEnumerator GameOver(float delay)
    {
        /*yield return new WaitForSeconds(delay);
        if (!gameEnded) // Kiểm tra lại để tránh hiển thị popup nếu game đã kết thúc
        {
            uiManager.ShowGameOverPopup();
            gameEnded = true; // Đánh dấu game đã kết thúc
        }*/
        yield return new WaitUntil(() => CheckIfAllBallsStopped());
        uiManager.ShowGameOverPopup();
    }

    // Bắt đầu phát bắn đầu tiên
    void StartFirstShot()
    {
        Debug.Log("Phát bắn đầu tiên, không cần chọn bi.");
        isFirstShot = true;
        ballSelector.enabled = false;
        hasCueBallShot = false;
        EnableCueBallControl();
        HideBallSelectionButtons();
    }

    // Kết thúc phát bắn đầu tiên
    void EndFirstShot()
    {
        isFirstShot = false;
        ballSelector.enabled = true;
        turnEnded = false;
        hasBallSelected = false;
        hasCueBallShot = false;
        DisableCueBallControl();
        ShowBallSelectionButtons();
    }

    // Gọi khi người chơi chọn bi để chuẩn bị bắn
    public void SelectBallForNextShot()
    {
        if (isSuperHardLevel)
        {
            holeManager.ResetHolePositions();
        }
        hasBallSelected = true;
        ballFellIntoHole = false;
        turnEnded = false;
        EnableCueBallControl();
        HideBallSelectionButtons();
        Debug.Log("Người chơi đã chọn bi cho lượt bắn tiếp theo.");
    }

    // Reset trạng thái cho lượt tiếp theo
    void ResetTurn()
    {
        hasBallSelected = false;
        ballFellIntoHole = false;
        hasCueBallShot = false;
        turnEnded = false;
        DisableCueBallControl();
        ShowBallSelectionButtons();
    }

    // Kiểm tra nếu tất cả các bi đã dừng lại
    public bool CheckIfAllBallsStopped()
    {
        foreach (Rigidbody2D rb in ballRigidbodies)
        {
            if (rb.velocity.magnitude > 0.01f)
            {
                return false;
            }
        }
        return true;
    }

    // Vô hiệu hóa bắn CueBall cho đến khi người chơi chọn bi
    public void DisableCueBallControl()
    {
        if (cueBallController != null)
        {
            cueBallController.canShoot = false;
        }
    }

    // Kích hoạt lại việc bắn CueBall
    public void EnableCueBallControl()
    {
        if (cueBallController != null)
        {
            cueBallController.canShoot = true;
        }
    }

    // Gọi khi bi cái được bắn
    public void OnCueBallShot()
    {
        hasCueBallShot = true;
        DisableCueBallControl();
    }

    // Gọi khi một viên bi rơi vào lỗ
    public void OnBallEnterHole(GameObject ball, HoleController hole)
    {
        if (gameEnded) return; // Kiểm tra nếu game đã kết thúc

        hole.anim.SetBool("Eat", true);
        ballFellIntoHole = true;
        StartCoroutine(HideBallFellInHole(ball));

        ballsInHoles.Add(ball.tag);

        if (ball.CompareTag("CueBall"))
        {
            StartCoroutine(GameOver(1f));
            Debug.Log("Người chơi đã thua");
        }
        else
        {
            ballInHole++;
            if (!isFirstShot)
            {
                GameObject selectedBall = ballSelector.GetSelectedBall();
                if (ball == selectedBall)
                {
                    Debug.Log("Chính xác! Bi " + ballSelector.GetSelectedTag() + " đã rơi vào lỗ.");
                    StartCoroutine(WaitAndResetTurn());
                }
                else
                {
                    StartCoroutine(GameOver(0.5f));
                    Debug.Log("Người chơi đã thua, viên bi không trùng với viên bi đã chọn!");
                }
            }
        }
        ballSelector.HideButtonByTag(ball.tag);
        HideImageByTag(ball.tag);
    }

    IEnumerator WaitAndResetTurn()
    {
        yield return new WaitUntil(() => CheckIfAllBallsStopped());
        ResetTurn();
    }

    IEnumerator HideBallFellInHole(GameObject ball)
    {
        yield return new WaitForSeconds(0.025f);
        ball.SetActive(false);
    }

    public void PlayerWins()
    {
        Debug.Log("Người chơi đã thắng!");
        uiManager.ShowWinPopup();
        gameEnded = true; // Đánh dấu game đã kết thúc
    }

    public void HideImageByTag(string tag)
    {
        for (int i = 0; i < ballRigidbodies.Count; i++)
        {
            if (ballRigidbodies[i].gameObject.CompareTag(tag))
            {
                ballImageManager.ballImages[i].gameObject.SetActive(false);
                break;
            }
        }
    }

    public void HideCurrentVisibleImage()
    {
        if (currentVisibleImage != null)
        {
            currentVisibleImage.gameObject.SetActive(false);
            currentVisibleImage = null;
        }
    }

    public void ShowImageByTag(string tag)
    {
        HideCurrentVisibleImage();

        for (int i = 0; i < ballRigidbodies.Count; i++)
        {
            if (ballRigidbodies[i].gameObject.CompareTag(tag))
            {
                ballImageManager.ballImages[i].gameObject.SetActive(true);
                currentVisibleImage = ballImageManager.ballImages[i];
                break;
            }
        }
    }

    public void HideBallSelectionButtons()
    {
        ballSelector.HideAllButtons();
    }

    public void ShowBallSelectionButtons()
    {
        ballSelector.ShowAllButtonsExcept(ballsInHoles);
    }
}
