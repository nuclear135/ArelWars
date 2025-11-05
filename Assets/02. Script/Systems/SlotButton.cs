using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// 슬롯 하나의 표시/클릭 동작을 담당
// 아무것도 할당하지 않으면 EMPTY로 표시되고 버튼이 비활성화된다
public class SlotButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Text labelText;

    [SerializeField] private Sprite emptySprite;
    [SerializeField] private string emptyText = "EMPTY";

    [SerializeField] private UnityEvent onClick; // 인스펙터에서 연결 가능한 클릭 이벤트

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.RemoveAllListeners();

            if (onClick != null)
            {
                button.onClick.AddListener(() => onClick.Invoke());
            }
        }
    }

    // 내용이 있는 슬롯으로 설정
    public void SetAssigned(Sprite icon, string text, UnityAction onClickAction = null)
    {
        if (iconImage != null)
        {
            iconImage.sprite = icon;
        }

        if (labelText != null)
        {
            labelText.text = text;
        }

        if (button != null)
        {
            button.interactable = true;
            button.onClick.RemoveAllListeners();

            if (onClick != null)
            {
                button.onClick.AddListener(() => onClick.Invoke());
            }

            if (onClickAction != null)
            {
                button.onClick.AddListener(onClickAction);
            }
        }
    }

    // 빈 슬롯으로 설정
    public void SetEmpty()
    {
        if (iconImage != null)
        {
            iconImage.sprite = emptySprite;
        }

        if (labelText != null)
        {
            labelText.text = emptyText;
        }

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.interactable = false;
        }
    }
}
