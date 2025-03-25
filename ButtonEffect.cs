using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    // 按钮的贴图
    public Sprite normalSprite;    // 默认状态贴图
    public Sprite hoverSprite;     // 鼠标悬停贴图
    public Sprite pressedSprite;   // 点击时贴图

    // 音频剪辑
    public AudioClip hoverSound;   // 鼠标悬停音效
    public AudioClip clickSound;   // 点击音效

    private Image buttonImage;     // 按钮的Image组件
    private AudioSource audioSource; // 音频播放组件

    void Start()
    {
        // 获取按钮的Image和AudioSource组件
        buttonImage = GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();

        // 设置默认贴图
        if (normalSprite != null)
        {
            buttonImage.sprite = normalSprite;
        }
    }

    // 鼠标进入按钮区域时触发
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSprite != null)
        {
            buttonImage.sprite = hoverSprite; // 切换到悬停贴图
        }
        if (hoverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverSound); // 播放悬停音效
        }
    }

    // 鼠标离开按钮区域时触发
    public void OnPointerExit(PointerEventData eventData)
    {
        if (normalSprite != null)
        {
            buttonImage.sprite = normalSprite; // 恢复默认贴图
        }
    }

    // 鼠标按下时触发
    public void OnPointerDown(PointerEventData eventData)
    {
        if (pressedSprite != null)
        {
            buttonImage.sprite = pressedSprite; // 切换到点击贴图
        }
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound); // 播放点击音效
        }
    }

    // 鼠标松开时触发
    public void OnPointerUp(PointerEventData eventData)
    {
        if (hoverSprite != null)
        {
            buttonImage.sprite = hoverSprite; // 松开后恢复悬停贴图（如果仍在按钮区域内）
        }
    }
}
