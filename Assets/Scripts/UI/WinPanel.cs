using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinPanel : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clip;
    public Button nextLevelButton;

    public void PlaySound()
    {
        audioSource.PlayOneShot(clip);
    }

    public void LoadNextLevel()
    {
        Debug.Log("Next Level Pressed");
        int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneIndex); // Tải scene tiếp theo
        }
        else
        {
            Debug.Log("Không có level tiếp theo!"); // Thông báo nếu đã đạt đến level cuối
        }
    }

}
