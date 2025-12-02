using UnityEngine;

public class ItemInteraction : MonoBehaviour, IInteractable
{
    [Header("Item Reference")]
    public BaseItem item;
    
    [Header("Visual Effects")]
    public ParticleSystem glowEffect;
    public float rotationSpeed = 30f;
    
    private bool canInteract = true;
    
    void Start()
    {
        if (item != null && item.worldPrefab != null)
        {
            Instantiate(item.worldPrefab, transform);
        }
        SetupVisualEffects();
    }
    
    void Update()
    {
        if (item != null && item.rarity >= ItemRarity.Rare)
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
    }
    
    // IInteractable implementation
    public void Interact(GameObject interactor)
    {
        if (!canInteract) return;
        
        Inventory inventory = interactor.GetComponent<Inventory>();
        PlayerSystems systems = interactor.GetComponent<PlayerSystems>();
        
        if (inventory != null && systems != null)
        {
            // Можно сразу использовать или добавить в инвентарь
            if (item is ConsumableItem consumable && consumable.CanUse(systems))
            {
                consumable.Use(systems);
                canInteract = false;
                Destroy(gameObject);
            }
            else if (inventory.AddItem(item))
            {
                Debug.Log($"🎒 Подобран: {item.itemName}");
                canInteract = false;
                Destroy(gameObject);
            }
        }
    }
    
    public string GetInteractionText()
    {
        return item != null ? $"Подобрать {item.itemName}" : "Предмет";
    }
    
    public bool CanInteract() => canInteract;
    
    private void SetupVisualEffects()
    {
        // Настройка визуальных эффектов на основе редкости
    }
}