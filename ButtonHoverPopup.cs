using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHoverPopup : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // 这个是鼠标悬停/点击时要显示的弹出内容(含视频和文字介绍)
    public GameObject popupPanel;

    // 用于标识当前弹窗是否已被“固定”（点击按钮后）
    private bool isPinned = false;

    void Start()
    {
        popupPanel.SetActive(false);
    }

    // 当鼠标移到按钮上时触发
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 如果尚未固定，就先显示弹出内容
        if (!isPinned)
        {
            popupPanel.SetActive(true);
        }
    }

    // 当鼠标移开按钮时触发
    public void OnPointerExit(PointerEventData eventData)
    {
        // 如果尚未固定，就隐藏弹出内容
        if (!isPinned)
        {
            popupPanel.SetActive(false);
        }
    }

    // 当鼠标点击按钮时触发
    public void OnPointerClick(PointerEventData eventData)
    {
        // 点击后，如果之前没有固定弹窗，那么固定之；如果已经固定，则解锁
        isPinned = !isPinned;

        // 如果固定了，则强制弹窗显示；如果解锁了，则根据鼠标是否还在按钮上来决定是否显示
        if (isPinned)
        {
            popupPanel.SetActive(true);
        }
        else
        {
            // 这里要判断一下鼠标是否还在按钮上（EventSystem 里可以检测当前指针对象）
            // 简单处理：先把弹窗隐藏，再看需不需要恢复
            popupPanel.SetActive(false);

            // 如果你想更严谨地根据鼠标还在不在按钮上来恢复显示，可以额外做判断：
            // if (PointerIsOverThisButton()) popupPanel.SetActive(true);
        }
    }
}
