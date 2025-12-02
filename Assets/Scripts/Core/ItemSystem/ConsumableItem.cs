using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "STALKER/Items/Consumable")]
public class ConsumableItem : BaseItem
{
    [Header("Consumable Settings")]
    public bool removeOnUse = true;
    public int useCount = 1;
    
    public override bool CanUse(PlayerSystems systems)
    {
        return systems != null && useCount > 0;
    }
    
    public override void Use(PlayerSystems systems)
    {
        if (!CanUse(systems)) return;
        
        ApplyEffects(systems);
        
        if (removeOnUse)
        {
            useCount--;
        }
        
        Debug.Log($"🔄 Использован: {itemName}");
    }
}