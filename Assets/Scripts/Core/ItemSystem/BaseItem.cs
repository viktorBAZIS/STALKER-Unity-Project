using UnityEngine;
using System.Collections.Generic;

public abstract class BaseItem : ScriptableObject
{
    [Header("Basic Information")]
    public string itemID;
    public string itemName;
    public float weight;
    public string description;
    public Sprite icon;
    
    [Header("World Representation")]
    public GameObject worldPrefab;
    public ItemRarity rarity = ItemRarity.Common;
    
    [Header("Item Effects")]
    public List<ItemEffect> effects = new List<ItemEffect>();
    
    [Header("Usage Settings")]
    public bool consumable = true;
    public float cooldown = 0f;
    public int maxStackSize = 1;
    
    // Основные методы
    public abstract bool CanUse(PlayerSystems systems);
    public abstract void Use(PlayerSystems systems);
    
    // Вспомогательные методы
    public virtual string GetItemType() => GetType().Name;
    
    protected void ApplyEffects(PlayerSystems systems)
    {
        foreach (var effect in effects)
        {
            systems.ApplyEffect(effect);
        }
    }
    
    // Автогенерация ID
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(itemID))
        {
            itemID = $"item_{System.Guid.NewGuid().ToString().Substring(0, 8)}";
        }
    }
}