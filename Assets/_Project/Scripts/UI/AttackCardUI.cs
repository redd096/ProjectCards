using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttackCardUI : MonoBehaviour
{
    [SerializeField] Image iconImage;
    [SerializeField] Button button;
    [SerializeField] GameObject borderOnSelect;
    [SerializeField] GameObject numberContainer;
    [SerializeField] TMP_Text numberText;

    public void Init(Sprite icon, System.Action onClick)
    {
        iconImage.sprite = icon;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke());
        ToggleSelectCard(false);
        SetNumberText(false);
    }

    /// <summary>
    /// Show or hide border
    /// </summary>
    /// <param name="isSelected"></param>
    public void ToggleSelectCard(bool isSelected)
    {
        borderOnSelect.SetActive(isSelected);
    }

    /// <summary>
    /// Remove OnClick event setted in Init
    /// </summary>
    public void RemoveOnClickEvent()
    {
        button.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// Show or hide NumberContainer and set NumberText
    /// </summary>
    /// <param name="isEnabled"></param>
    /// <param name="number"></param>
    public void SetNumberText(bool isEnabled, int number = 0)
    {
        numberContainer.SetActive(isEnabled);
        numberText.text = number.ToString();
    }
}
