using UnityEngine;

public class WorldItem : MonoBehaviour, IInteractable
{
    [Header("Item Settings")]
    public BaseItem item; // ← ИЗМЕНЕНО: ItemData → BaseItem
    public int quantity = 1;

    [Header("Visual Effects")]
    public ParticleSystem glowEffect;
    public float rotationSpeed = 30f;

    private bool canInteract = true;

    void Start()
    {
        // Автоматически находим префаб из Item если не назначен
        if (item != null && item.worldPrefab != null)
        {
            GameObject visual = Instantiate(item.worldPrefab, transform);
            visual.transform.localPosition = Vector3.zero;
        }

        SetupVisualEffects();
    }

    void Update()
    {
        // Медленное вращение для визуального эффекта
        if (item != null && item.rarity >= ItemRarity.Rare)
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
    }

    void SetupVisualEffects()
    {
        if (glowEffect != null)
        {
            var main = glowEffect.main;

            // Разный цвет свечения в зависимости от редкости
            switch (item.rarity) // ← ИЗМЕНЕНО: itemData → item
            {
                case ItemRarity.Common:
                    glowEffect.Stop();
                    break;
                case ItemRarity.Uncommon:
                    main.startColor = Color.green;
                    break;
                case ItemRarity.Rare:
                    main.startColor = Color.blue;
                    break;
                case ItemRarity.Epic:
                    main.startColor = new Color(0.5f, 0f, 0.5f); // Фиолетовый
                    break;
                case ItemRarity.Legendary:
                    main.startColor = new Color(1f, 0.5f, 0f); // Оранжевый
                    break;
            }
        }
    }

    // Реализация IInteractable
    public void Interact(GameObject interactor)
    {
        if (!canInteract) return;

        // ТОЛЬКО ОДНА ЭТА СТРОКА:
        Inventory inventory = interactor.GetComponent<Inventory>();

        if (inventory != null)
        {
            if (inventory.AddItem(item, quantity)) // ← ИЗМЕНЕНО: itemData → item
            {
                Debug.Log($"Подобран: {item.itemName} x{quantity}"); // ← ИЗМЕНЕНО: itemData → item
                canInteract = false;
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Не удалось подобрать предмет - инвентарь полон!");
            }
        }
    }

    public string GetInteractionText()
    {
        if (item == null) return "Предмет"; // ← ИЗМЕНЕНО: itemData → item

        string text = $"Подобрать {item.itemName}"; // ← ИЗМЕНЕНО: itemData → item
        if (quantity > 1) text += $" x{quantity}";

        return text;
    }

    public bool CanInteract()
    {
        return canInteract;
    }

    // Для создания предметов в мире через код
    public static WorldItem SpawnItem(BaseItem data, Vector3 position, int count = 1) // ← ИЗМЕНЕНО: ItemData → BaseItem
    {
        if (data == null) return null;

        GameObject itemObj = new GameObject($"WorldItem_{data.itemName}");
        itemObj.transform.position = position;

        WorldItem worldItem = itemObj.AddComponent<WorldItem>();
        worldItem.item = data; // ← ИЗМЕНЕНО: itemData → item
        worldItem.quantity = count;

        // Добавляем коллайдер для взаимодействия
        SphereCollider collider = itemObj.AddComponent<SphereCollider>();
        collider.isTrigger = true;
        collider.radius = 0.5f;

        return worldItem;
    }
}