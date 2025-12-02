using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [Header("Radiation Settings")]
    public float maxRadiation = 100f;
    public float currentRadiation = 0f;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float currentStamina = 100f;
    public float staminaDrainRate = 10f;
    public float staminaRegenRate = 5f;

    [Header("UI References")]
    public Slider healthBar;
    public Slider radiationBar;
    public Slider staminaBar;

    // События для других систем
    public System.Action<float> OnHealthChanged;
    public System.Action<float> OnRadiationChanged;

    void Start()
    {
        // Инициализация значений
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        UpdateUI();
    }

    void Update()
    {
        HandleStamina();
    }

    void HandleStamina()
    {
        // Восстановление выносливости
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            UpdateUI();
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth);
        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void AddRadiation(float radiation)
    {
        currentRadiation += radiation;
        currentRadiation = Mathf.Clamp(currentRadiation, 0, maxRadiation);

        OnRadiationChanged?.Invoke(currentRadiation);
        UpdateUI();
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth);
        UpdateUI();
    }

    public void ReduceRadiation(float amount)
    {
        currentRadiation = Mathf.Clamp(currentRadiation - amount, 0f, 100f);
        OnRadiationChanged?.Invoke(currentRadiation);
    }

    public bool UseStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    void UpdateUI()
    {
        if (healthBar != null)
            healthBar.value = currentHealth / maxHealth;

        if (radiationBar != null)
            radiationBar.value = currentRadiation / maxRadiation;

        if (staminaBar != null)
            staminaBar.value = currentStamina / maxStamina;
    }

    void Die()
    {
        Debug.Log("Игрок умер!");
        // Здесь будет перезагрузка уровня или меню смерти
    }
}