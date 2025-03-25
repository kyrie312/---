using UnityEngine;
using UnityEngine.UI;

public class EnterKeyButtonTrigger : MonoBehaviour
{
    public Button targetButton;

    void Update()
    {
        // 判断是否按下回车键（包括主键盘和小键盘）
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (targetButton != null && targetButton.interactable)
            {
                targetButton.onClick.Invoke(); // 触发 Button 的点击事件
            }
        }
    }
}
