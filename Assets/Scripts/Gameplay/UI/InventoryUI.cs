using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject inventoryPanel;
    public Transform itemsContainer;
    public TextMeshProUGUI weightText;
    public TextMeshProUGUI slotsText;

    [Header("Prefabs")]
    public GameObject itemSlotPrefab;

    private Inventory playerInventory;
    private bool isInventoryOpen = false;

    void Start()
    {
        playerInventory = FindObjectOfType<Inventory>();
        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged += UpdateInventoryUI;
        }

        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;

        if (inventoryPanel != null)
            inventoryPanel.SetActive(isInventoryOpen);

        if (isInventoryOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // ОБНОВЛЯЕМ UI ПРИ ОТКРЫТИИ
            if (playerInventory != null)
            {
                UpdateInventoryUI(playerInventory);
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // ОЧИЩАЕМ СЛОТЫ ПРИ ЗАКРЫТИИ
            if (itemsContainer != null)
            {
                foreach (Transform child in itemsContainer)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }

    void UpdateInventoryUI(Inventory inventory)
    {
        if (inventory == null || itemsContainer == null) return;

        // Очищаем контейнер только если инвентарь открыт
        if (isInventoryOpen)
        {
            foreach (Transform child in itemsContainer)
            {
                Destroy(child.gameObject);
            }

            // Создаем слоты только для открытого инвентаря
            List<InventorySlot> items = inventory.GetAllItems();
            foreach (InventorySlot slot in items)
            {
                if (slot == null || slot.item == null) continue; // ← ИЗМЕНЕНО: itemData → item

                if (itemSlotPrefab != null)
                {
                    GameObject slotUI = Instantiate(itemSlotPrefab, itemsContainer.transform);
                    InventorySlotUI slotScript = slotUI.GetComponent<InventorySlotUI>();
                    if (slotScript != null)
                    {
                        slotScript.Setup(slot);
                    }
                }
            }
        }

        // Обновляем статы ВСЕГДА (даже при закрытом инвентаре)
        if (weightText != null)
            weightText.text = $"Вес: {inventory.GetCurrentWeight():F1}/{inventory.maxWeight}кг";

        if (slotsText != null)
            slotsText.text = $"Слоты: {inventory.GetUsedSlots()}/{inventory.maxSlots}";
    }

    void OnDestroy()
    {
        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged -= UpdateInventoryUI;
        }
    }
}