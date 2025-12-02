using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("Health Bars")]
    public Slider healthBar;
    public Slider radiationBar;
    public Slider staminaBar;

    [Header("Text Displays (TMP)")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI radiationText;
    public TextMeshProUGUI staminaText;
    public TextMeshProUGUI weightText;
    public TextMeshProUGUI slotsText;

    [Header("References")]
    public PlayerHealth playerHealth;

    void Start()
    {
        // Находим PlayerHealth если не установлен вручную
        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<PlayerHealth>();
        }

        if (playerHealth != null)
        {
            // Подписываемся на события изменения здоровья
            playerHealth.OnHealthChanged += UpdateHealthUI;
            playerHealth.OnRadiationChanged += UpdateRadiationUI;

            // Инициализируем значения
            UpdateHealthUI(playerHealth.currentHealth);
            UpdateRadiationUI(playerHealth.currentRadiation);
        }
        else
        {
            Debug.LogError("HUDManager: PlayerHealth not found!");
        }

        // Инициализируем тексты по умолчанию
        UpdateInventoryText();
    }

    void Update()
    {
        // Обновляем стамину каждый кадр (она меняется часто)
        if (playerHealth != null)
        {
            UpdateStaminaUI();
        }
    }

    void UpdateHealthUI(float health)
    {
        // Обновляем полоску здоровья
        if (healthBar != null)
        {
            healthBar.value = health / playerHealth.maxHealth;
        }

        // Обновляем текстовое отображение
        if (healthText != null)
        {
            healthText.text = $"HP: {health:F0}/{playerHealth.maxHealth:F0}";
        }
    }

    void UpdateRadiationUI(float radiation)
    {
        // Обновляем полоску радиации
        if (radiationBar != null)
        {
            radiationBar.value = radiation / playerHealth.maxRadiation;
        }

        // Обновляем текстовое отображение
        if (radiationText != null)
        {
            radiationText.text = $"RAD: {radiation:F0}%";

            // Меняем цвет при высокой радиации
            if (radiation > 70f)
                radiationText.color = Color.red;
            else if (radiation > 30f)
                radiationText.color = Color.yellow;
            else
                radiationText.color = Color.white;
        }
    }

    void UpdateStaminaUI()
    {
        // Обновляем полоску стамины
        if (staminaBar != null)
        {
            staminaBar.value = playerHealth.currentStamina / playerHealth.maxStamina;
        }

        // Обновляем текстовое отображение
        if (staminaText != null)
        {
            staminaText.text = $"STM: {playerHealth.currentStamina:F0}%";

            // Меняем цвет при низкой стамине
            if (playerHealth.currentStamina < 20f)
                staminaText.color = Color.red;
            else if (playerHealth.currentStamina < 50f)
                staminaText.color = Color.yellow;
            else
                staminaText.color = Color.green;
        }
    }

    // Метод для обновления текстов инвентаря (из вашей структуры)
    public void UpdateInventoryText()
    {
        if (weightText != null)
        {
            weightText.text = "Вес: 0/50 кг"; // Заглушка
        }

        if (slotsText != null)
        {
            slotsText.text = "Слоты: 0/20"; // Заглушка
        }
    }

    // Метод для принудительного обновления всего HUD
    public void RefreshAllHUD()
    {
        if (playerHealth != null)
        {
            UpdateHealthUI(playerHealth.currentHealth);
            UpdateRadiationUI(playerHealth.currentRadiation);
            UpdateStaminaUI();
        }
        UpdateInventoryText();
    }

    // Важно: отписываемся от событий при уничтожении
    void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthUI;
            playerHealth.OnRadiationChanged -= UpdateRadiationUI;
        }
    }
}