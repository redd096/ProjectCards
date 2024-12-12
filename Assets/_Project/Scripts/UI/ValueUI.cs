using UnityEngine;
using UnityEngine.UI;

public class ValueUI : MonoBehaviour
{
    [SerializeField] Image iconImage;

    public void Init(Sprite icon)
    {
        iconImage.sprite = icon;
    }
}
