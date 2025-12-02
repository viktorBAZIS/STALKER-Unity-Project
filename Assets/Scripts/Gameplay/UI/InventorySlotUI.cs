using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    public Image background;
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI quantityText;
    public CanvasGroup canvasGroup;

    private InventorySlot currentSlot;
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 originalPosition;
    private bool isDragging = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void Setup(InventorySlot slot)
    {
        currentSlot = slot;

        if (slot == null || slot.item == null) // ← ИЗМЕНЕНО: itemData → item
        {
            ClearSlot();
            return;
        }

        // ДОБАВЬТЕ ЭТИ ДЕБАГИ
        Debug.Log($"🔄 Setting up slot: {slot.item.itemName}, Quantity: {slot.quantity}");
        Debug.Log($"📝 NameText exists: {nameText != null}");
        Debug.Log($"🔢 QuantityText exists: {quantityText != null}");

        // Иконка
        if (icon != null)
        {
            icon.sprite = slot.item.icon; // ← ИЗМЕНЕНО: itemData → item
            icon.gameObject.SetActive(true);
            Debug.Log($"🖼️ Icon set: {slot.item.icon != null}");
        }

        // Название
        if (nameText != null)
        {
            nameText.text = slot.item.itemName; // ← ИЗМЕНЕНО: itemData → item
            Debug.Log($"📋 Name text set to: '{slot.item.itemName}'");
        }
        else
        {
            Debug.LogError("❌ NameText is NULL!");
        }

        // Количество (ВСЕГДА показывать)
        if (quantityText != null)
        {
            quantityText.text = slot.quantity.ToString();
            quantityText.gameObject.SetActive(true);
            Debug.Log($"🔢 Quantity text set to: '{slot.quantity}'");
        }

        // Цвет фона
        if (background != null)
        {
            background.color = GetColorByItemType(slot.item); // ← ИЗМЕНЕНО: itemData → item
        }
    }

    void ClearSlot()
    {
        if (icon != null) icon.gameObject.SetActive(false);
        if (nameText != null) nameText.text = "";
        if (quantityText != null) quantityText.text = "";
        if (background != null) background.color = Color.gray;
    }

    // Метод для определения цвета по типу предмета
    private Color GetColorByItemType(BaseItem item) // ← ИЗМЕНЕНО: ItemData → BaseItem
    {
        if (item == null) return new Color(0.5f, 0.5f, 0.5f, 0.3f);

        // Используем тип предмета из новой системы
        string itemType = item.GetItemType();

        switch (itemType)
        {
            case "ConsumableItem":
                if (item.itemName.Contains("Аптечка") || item.itemName.Contains("Медикаменты"))
                    return new Color(1f, 0.2f, 0.2f, 0.3f); // Красный для медицины
                if (item.itemName.Contains("Бинт") || item.itemName.Contains("Перевязоч"))
                    return new Color(1f, 1f, 0.2f, 0.3f); // Желтый для бинтов
                if (item.itemName.Contains("Хлеб") || item.itemName.Contains("Еда"))
                    return new Color(0.2f, 1f, 0.2f, 0.3f); // Зеленый для еды
                break;

            case "EquipableItem":
                return new Color(0.2f, 0.5f, 1f, 0.3f); // Синий для экипировки

            case "WeaponItem":
                return new Color(0.8f, 0.2f, 0.2f, 0.3f); // Темно-красный для оружия
        }

        return new Color(0.5f, 0.5f, 0.5f, 0.3f); // Серый по умолчанию
    }

    // Drag & Drop
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentSlot == null || currentSlot.item == null) return; // ← ИЗМЕНЕНО: itemData → item

        originalPosition = rectTransform.anchoredPosition;
        if (canvasGroup != null) canvasGroup.alpha = 0.6f;
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        if (canvasGroup != null) canvasGroup.alpha = 1f;
        rectTransform.anchoredPosition = originalPosition;
        isDragging = false;

        // Здесь будет логика перемещения между слотами
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Подсветка при наведении
        if (background != null)
            background.color = Color.yellow;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Возврат обычного цвета
        if (background != null && currentSlot?.item != null) // ← ИЗМЕНЕНО: itemData → item
            background.color = GetColorByItemType(currentSlot.item); // ← ИЗМЕНЕНО: itemData → item
    }

    // Новый метод для использования предмета
    public void UseItem()
    {
        if (currentSlot?.item != null)
        {
            Debug.Log($"🔄 Попытка использовать: {currentSlot.item.itemName}");
            // Здесь можно добавить логику использования предмета
        }
    }
}