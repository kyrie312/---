using UnityEngine;
using UnityEngine.UI;

public class EnterKeyButtonTrigger : MonoBehaviour
{
    public Button targetButton;

    void Update()
    {
        // �ж��Ƿ��»س��������������̺�С���̣�
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (targetButton != null && targetButton.interactable)
            {
                targetButton.onClick.Invoke(); // ���� Button �ĵ���¼�
            }
        }
    }
}
