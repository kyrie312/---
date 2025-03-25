using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BookToMainMenu : MonoBehaviour // ���������ļ���һ��
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