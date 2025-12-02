using UnityEngine;

// Старые enum для совместимости
public enum ItemType
{
    Medical,
    Antirad,
    Food,
    Bandage,
    Weapon,
    Ammo,
    Artifact,
    Miscellaneous,
    Quest
}

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(fileName = "New Item", menuName = "STALKER/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Basic Info - Legacy Support")]
    public string itemID; // Для совместимости
    public string itemName;
    public ItemType itemType; // Для совместимости
    public ItemRarity rarity = ItemRarity.Common; // Для совместимости
    public float weight;
    public string description;
    public Sprite icon;

    [Header("World Representation - Legacy Support")]
    public GameObject worldPrefab; // Для совместимости

    [Header("New Scalable System")]
    public ItemTypeData itemTypeData; // Новая масштабируемая система

    [Header("Default Effects")]
    public float healthEffect = 0f;
    public float radiationEffect = 0f;
    public float staminaEffect = 0f;

    // Автогенерация ID если не задан
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(itemID))
        {
            itemID = $"item_{System.Guid.NewGuid().ToString().Substring(0, 8)}";
        }
    }

    // Совместимость: если новая система не задана, используем старую
    public string GetItemTypeName()
    {
        return itemTypeData != null ? itemTypeData.typeName : itemType.ToString();
    }

    public Color GetEditorColor()
    {
        return itemTypeData != null ? itemTypeData.editorColor : GetLegacyColor();
    }

    private Color GetLegacyColor()
    {
        switch (itemType)
        {
            case ItemType.Medical: return Color.green;
            case ItemType.Antirad: return Color.blue;
            case ItemType.Food: return Color.yellow;
            case ItemType.Bandage: return Color.red;
            default: return Color.white;
        }
    }
}