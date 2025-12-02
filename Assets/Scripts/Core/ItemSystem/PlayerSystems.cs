using UnityEngine;
using System.Collections.Generic;

public class PlayerSystems : MonoBehaviour
{
    [Header("Core Systems")]
    public PlayerHealth health;
    public StalkerAimSystem aim;
    // Добавить позже: hunger, thirst, radiation systems
    
    private Dictionary<EffectType, float> activeEffects = new Dictionary<EffectType, float>();
    
    void Update()
    {
        UpdateTimedEffects();
    }
    
    public void ApplyEffect(ItemEffect effect)
    {
        switch (effect.type)
        {
            case EffectType.HealthRestore:
                health.Heal(effect.value);
                break;
                
            case EffectType.RadiationReduce:
                health.ReduceRadiation(effect.value);
                break;
                
            case EffectType.StaminaRestore:
                health.currentStamina = Mathf.Clamp(
                    health.currentStamina + effect.value, 0, health.maxStamina);
                break;
                
            // Добавить другие эффекты позже
            default:
                Debug.Log($"Эффект {effect.type} применен: {effect.value}");
                break;
        }
        
        // Если эффект длительный - добавляем в активные
        if (effect.duration > 0)
        {
            activeEffects[effect.type] = effect.duration;
        }
        
        // Визуальные/звуковые эффекты
        PlayEffectFeedback(effect);
    }
    
    private void UpdateTimedEffects()
    {
        // Обновление длительных эффектов
        var keys = new List<EffectType>(activeEffects.Keys);
        foreach (var effectType in keys)
        {
            activeEffects[effectType] -= Time.deltaTime;
            if (activeEffects[effectType] <= 0)
            {
                activeEffects.Remove(effectType);
                // Снять эффект
            }
        }
    }
    
    private void PlayEffectFeedback(ItemEffect effect)
    {
        // Реализовать позже визуальные/звуковые эффекты
    }
}