using TMPro;
using UnityEngine;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text healthText;

    public Transform content;

    public void Init(string playerName, int playerHealth)
    {
        nameText.text = playerName;
        healthText.text = playerHealth.ToString();
    }
}
