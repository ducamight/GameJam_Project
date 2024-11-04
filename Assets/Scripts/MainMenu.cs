using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public string easyLevel, mediumLevel, hardLevel, superHardLevel;
    public GameObject SelectLevelPanel;
    /*    public AudioSource audioSource;
        public AudioClip audioClip;*/

    private void Start()
    {
        //SelectLevelPanel.SetActive(false);
    }
    public void OnStartButtonPressed()
    {
       SelectLevelPanel.SetActive(true);
    }

    public void OnEasyLevelButtonPressed()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(easyLevel);
    }
    public void OnMediumLevelButtonPressed()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(mediumLevel);
    }
    public void OnHardLevelButtonPressed()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(hardLevel);
    }
    public void OnSuperHardLEvelButtonPressed()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(superHardLevel);

    }
}
