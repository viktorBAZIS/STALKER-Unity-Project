using UnityEngine;

public class StalkerAimSystem : MonoBehaviour
{
    [Header("Aim Settings")]
    public float aimSpeed = 1.5f; // Медленнее чем в Tarkov
    public Vector3 hipPosition;
    public Vector3 aimPosition;
    public float fovZoom = 50f; // Меньше зумирование

    [Header("Player References")]
    public PlayerHealth playerHealth;

    [Header("Hand Shake Settings")]
    public float handShakeIntensity = 0f;
    public float maxHandShake = 0.5f;

    [Header("Health-Based Shake")]
    public float healthShakeMultiplier = 2.0f;
    public float radiationShakeMultiplier = 1.5f;

    void Start()
    {
        // АВТОМАТИЧЕСКИ находим PlayerHealth если не назначен вручную
        if (playerHealth == null)
        {
            playerHealth = GetComponentInParent<PlayerHealth>();
            if (playerHealth == null)
            {
                Debug.LogError("PlayerHealth not found in StalkerAimSystem!");
            }
        }
    }

    void Update()
    {
        // НЕПРЕРЫВНО обновляем дрожание на основе здоровья/радиации
        UpdateHealthBasedShake();
    }

    // НОВЫЙ МЕТОД: автоматическое дрожание от здоровья/радиации
    private void UpdateHealthBasedShake()
    {
        if (playerHealth == null) return;

        float healthShake = 0f;
        float radiationShake = 0f;

        // Дрожание от низкого здоровья (менее 30%)
        if (playerHealth.currentHealth < 30f)
        {
            healthShake = (30f - playerHealth.currentHealth) / 30f * healthShakeMultiplier;
        }

        // Дрожание от радиации (более 25%)
        if (playerHealth.currentRadiation > 25f)
        {
            radiationShake = (playerHealth.currentRadiation - 25f) / 75f * radiationShakeMultiplier;
        }

        // Комбинируем оба эффекта
        float totalShake = Mathf.Clamp(healthShake + radiationShake, 0f, maxHandShake);
        SetHandShake(totalShake);
    }

    // STALKER: дрожание рук от радиации/здоровья
    public float GetAimStability()
    {
        float stability = 1.0f;
        if (playerHealth != null)
        {
            // ИСПРАВЛЕНО: используем currentRadiation и currentHealth
            if (playerHealth.currentRadiation > 50) stability *= 0.7f;
            if (playerHealth.currentHealth < 30) stability *= 0.5f;

            // Учитываем текущее дрожание рук
            stability *= (1f - handShakeIntensity);
        }
        return Mathf.Clamp(stability, 0.1f, 1f);
    }

    // Метод для установки дрожания рук из внешних систем
    public void SetHandShake(float intensity)
    {
        handShakeIntensity = Mathf.Clamp(intensity, 0f, maxHandShake);
    }

    // Метод для получения текущей стабильности прицеливания
    public float GetCurrentStability()
    {
        return GetAimStability();
    }

    // Визуализация в редакторе (опционально)
    private void OnDrawGizmosSelected()
    {
        // Визуализация позиций прицеливания
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + hipPosition, 0.1f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + aimPosition, 0.1f);
    }
}