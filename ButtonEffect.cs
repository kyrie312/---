using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    // ��ť����ͼ
    public Sprite normalSprite;    // Ĭ��״̬��ͼ
    public Sprite hoverSprite;     // �����ͣ��ͼ
    public Sprite pressedSprite;   // ���ʱ��ͼ

    // ��Ƶ����
    public AudioClip hoverSound;   // �����ͣ��Ч
    public AudioClip clickSound;   // �����Ч

    private Image buttonImage;     // ��ť��Image���
    private AudioSource audioSource; // ��Ƶ�������

    void Start()
    {
        // ��ȡ��ť��Image��AudioSource���
        buttonImage = GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();

        // ����Ĭ����ͼ
        if (normalSprite != null)
        {
            buttonImage.sprite = normalSprite;
        }
    }

    // �����밴ť����ʱ����
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSprite != null)
        {
            buttonImage.sprite = hoverSprite; // �л�����ͣ��ͼ
        }
        if (hoverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverSound); // ������ͣ��Ч
        }
    }

    // ����뿪��ť����ʱ����
    public void OnPointerExit(PointerEventData eventData)
    {
        if (normalSprite != null)
        {
            buttonImage.sprite = normalSprite; // �ָ�Ĭ����ͼ
        }
    }

    // ��갴��ʱ����
    public void OnPointerDown(PointerEventData eventData)
    {
        if (pressedSprite != null)
        {
            buttonImage.sprite = pressedSprite; // �л��������ͼ
        }
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound); // ���ŵ����Ч
        }
    }

    // ����ɿ�ʱ����
    public void OnPointerUp(PointerEventData eventData)
    {
        if (hoverSprite != null)
        {
            buttonImage.sprite = hoverSprite; // �ɿ���ָ���ͣ��ͼ��������ڰ�ť�����ڣ�
        }
    }
}
