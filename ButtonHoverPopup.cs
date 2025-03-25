using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHoverPopup : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // ����������ͣ/���ʱҪ��ʾ�ĵ�������(����Ƶ�����ֽ���)
    public GameObject popupPanel;

    // ���ڱ�ʶ��ǰ�����Ƿ��ѱ����̶����������ť��
    private bool isPinned = false;

    void Start()
    {
        popupPanel.SetActive(false);
    }

    // ������Ƶ���ť��ʱ����
    public void OnPointerEnter(PointerEventData eventData)
    {
        // �����δ�̶���������ʾ��������
        if (!isPinned)
        {
            popupPanel.SetActive(true);
        }
    }

    // ������ƿ���ťʱ����
    public void OnPointerExit(PointerEventData eventData)
    {
        // �����δ�̶��������ص�������
        if (!isPinned)
        {
            popupPanel.SetActive(false);
        }
    }

    // ���������ťʱ����
    public void OnPointerClick(PointerEventData eventData)
    {
        // ��������֮ǰû�й̶���������ô�̶�֮������Ѿ��̶��������
        isPinned = !isPinned;

        // ����̶��ˣ���ǿ�Ƶ�����ʾ����������ˣ����������Ƿ��ڰ�ť���������Ƿ���ʾ
        if (isPinned)
        {
            popupPanel.SetActive(true);
        }
        else
        {
            // ����Ҫ�ж�һ������Ƿ��ڰ�ť�ϣ�EventSystem ����Լ�⵱ǰָ�����
            // �򵥴����Ȱѵ������أ��ٿ��費��Ҫ�ָ�
            popupPanel.SetActive(false);

            // ���������Ͻ��ظ�����껹�ڲ��ڰ�ť�����ָ���ʾ�����Զ������жϣ�
            // if (PointerIsOverThisButton()) popupPanel.SetActive(true);
        }
    }
}
