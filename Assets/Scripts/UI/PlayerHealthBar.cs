using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI healthText;
    
    public void SetHealth(float normalizedHealth, int currentHealth, int maxHealth)
    {
        fillImage.fillAmount = Mathf.Clamp01(normalizedHealth);
        if (healthText != null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}";
        }
    }
}