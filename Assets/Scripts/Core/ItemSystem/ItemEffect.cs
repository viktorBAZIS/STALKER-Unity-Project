using UnityEngine;

[System.Serializable]
public struct ItemEffect
{
    [Header("Effect Settings")]
    public EffectType type;
    public float value;
    public float duration; // 0 = мгновенный эффект
    public bool isPercentage; // true = значение в процентах
    
    [Header("Visual Feedback")]
    public ParticleSystem effectParticles;
    public AudioClip soundEffect;
    
    public ItemEffect(EffectType effectType, float effectValue, float effectDuration = 0f)
    {
        type = effectType;
        value = effectValue;
        duration = effectDuration;
        isPercentage = false;
        effectParticles = null;
        soundEffect = null;
    }
}