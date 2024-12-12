using UnityEngine;
using UnityEngine.UI;

public class AttackCardUI : MonoBehaviour
{
    [SerializeField] Image iconImage;
    [SerializeField] Button button;
    [SerializeField] GameObject borderOnSelect;

    public void Init(Sprite icon, System.Action onClick)
    {
        iconImage.sprite = icon;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke());
        ToggleSelectCard(false);
    }

    public void ToggleSelectCard(bool isSelected)
    {
        borderOnSelect.SetActive(isSelected);
    }
}
