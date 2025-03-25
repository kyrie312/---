using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BookToMainMenu : MonoBehaviour // 类名需与文件名一致
{
    public Button backButton;

    void Start()
    {
        backButton.onClick.AddListener(ReturnToMainScene);
    }

    void ReturnToMainScene()
    {
        SceneManager.LoadScene("Scene_Main");
    }
}