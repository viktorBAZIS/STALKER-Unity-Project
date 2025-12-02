using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class InventorySlot
{
    public BaseItem item;
    public int quantity;
    public bool isEquipped;

    public InventorySlot(BaseItem itemData, int count)
    {
        item = itemData;
        quantity = count;
        isEquipped = false;
    }
}

public class Inventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    public int maxSlots = 20;
    public float maxWeight = 50f;

    [Header("Debug View")]
    [SerializeField] private List<InventorySlot> items = new List<InventorySlot>();
    [SerializeField] private float currentWeight = 0f;

    [Header("References")]
    public PlayerSystems playerSystems;

    // Защита от рекурсии
    private bool isProcessing = false;

    // События для UI
    public System.Action<Inventory> OnInventoryChanged;

    void Start()
    {
        // Автоматически находим PlayerSystems
        if (playerSystems == null)
        {
            playerSystems = GetComponent<PlayerSystems>();
        }

        Debug.Log($"Инвентарь инициализирован: {maxSlots} слотов, {maxWeight}кг емкость");
    }

    public bool AddItem(BaseItem item, int quantity = 1)
    {
        // Защита от рекурсии
        if (isProcessing)
        {
            Debug.LogWarning("Рекурсивный вызов AddItem предотвращен");
            return false;
        }

        if (item == null)
        {
            Debug.LogWarning("Попытка добавить null предмет");
            return false;
        }

        isProcessing = true;

        // Проверка веса
        float addedWeight = item.weight * quantity;
        if (currentWeight + addedWeight > maxWeight)
        {
            Debug.Log($"Перегруз! Нельзя добавить {item.itemName}: +{addedWeight}кг");
            isProcessing = false;
            return false;
        }

        // Поиск существующего стека (только для расходников)
        if (item.consumable && item.maxStackSize > 1)
        {
            InventorySlot existingSlot = items.Find(slot =>
                slot.item.itemID == item.itemID &&
                slot.quantity + quantity <= item.maxStackSize);

            if (existingSlot != null)
            {
                existingSlot.quantity += quantity;
                currentWeight += addedWeight;
                OnInventoryChanged?.Invoke(this);
                isProcessing = false;
                return true;
            }
        }

        // Проверка свободных слотов
        if (items.Count >= maxSlots)
        {
            Debug.Log("Инвентарь полон!");
            isProcessing = false;
            return false;
        }

        // Добавление в новый слот
        InventorySlot newSlot = new InventorySlot(item, quantity);
        items.Add(newSlot);
        currentWeight += addedWeight;

        OnInventoryChanged?.Invoke(this);
        Debug.Log($"Добавлен: {item.itemName} x{quantity}");

        isProcessing = false;
        return true;
    }

    public bool RemoveItem(string itemID, int quantity = 1)
    {
        if (isProcessing) return false;

        isProcessing = true;

        InventorySlot slot = items.Find(s => s.item.itemID == itemID);
        if (slot != null)
        {
            if (slot.quantity >= quantity)
            {
                currentWeight -= slot.item.weight * quantity;
                slot.quantity -= quantity;

                if (slot.quantity <= 0)
                {
                    items.Remove(slot);
                }

                OnInventoryChanged?.Invoke(this);
                isProcessing = false;
                return true;
            }
        }

        isProcessing = false;
        return false;
    }

    public bool UseItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= items.Count) return false;
        if (playerSystems == null) return false;

        InventorySlot slot = items[slotIndex];

        if (slot.item is ConsumableItem consumable)
        {
            if (consumable.CanUse(playerSystems))
            {
                consumable.Use(playerSystems);

                // Уменьшаем количество или удаляем
                if (consumable.removeOnUse)
                {
                    slot.quantity--;
                    currentWeight -= slot.item.weight;

                    if (slot.quantity <= 0)
                    {
                        items.RemoveAt(slotIndex);
                    }
                }

                OnInventoryChanged?.Invoke(this);
                return true;
            }
        }

        return false;
    }

    public bool UseItem(string itemID)
    {
        int slotIndex = items.FindIndex(s => s.item.itemID == itemID);
        return UseItem(slotIndex);
    }

    public bool HasItem(string itemID, int quantity = 1)
    {
        InventorySlot slot = items.Find(s => s.item.itemID == itemID);
        return slot != null && slot.quantity >= quantity;
    }

    public int GetItemCount(string itemID)
    {
        InventorySlot slot = items.Find(s => s.item.itemID == itemID);
        return slot?.quantity ?? 0;
    }

    public List<InventorySlot> GetAllItems()
    {
        return new List<InventorySlot>(items);
    }

    public InventorySlot GetSlot(int index)
    {
        if (index >= 0 && index < items.Count)
            return items[index];
        return null;
    }

    public float GetCurrentWeight()
    {
        return currentWeight;
    }

    public int GetUsedSlots()
    {
        return items.Count;
    }

    public int GetFreeSlots()
    {
        return maxSlots - items.Count;
    }

    // Для отладки
    [ContextMenu("Print Inventory Contents")]
    public void PrintInventory()
    {
        Debug.Log("=== СОДЕРЖИМОЕ ИНВЕНТАРЯ ===");
        foreach (var slot in items)
        {
            string typeInfo = slot.item.GetItemType();
            Debug.Log($"{slot.item.itemName} x{slot.quantity} ({slot.item.weight * slot.quantity}кг) [{typeInfo}]");
        }
        Debug.Log($"Вес: {currentWeight}/{maxWeight}кг, Слоты: {items.Count}/{maxSlots}");
    }

    [ContextMenu("Clear Inventory")]
    public void ClearInventory()
    {
        items.Clear();
        currentWeight = 0f;
        OnInventoryChanged?.Invoke(this);
        Debug.Log("Инвентарь очищен");
    }

    // Быстрое добавление для тестирования
    public void DebugAddItem(BaseItem item, int quantity = 1)
    {
        if (AddItem(item, quantity))
        {
            Debug.Log($"✅ Добавлен для теста: {item.itemName} x{quantity}");
        }
        else
        {
            Debug.Log($"❌ Не удалось добавить: {item.itemName}");
        }
    }
}