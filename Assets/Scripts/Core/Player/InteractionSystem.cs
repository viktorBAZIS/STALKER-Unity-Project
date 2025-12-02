using UnityEngine;

public class SimpleInteraction : MonoBehaviour
{
    [Header("Настройки взаимодействия")]
    public float interactionDistance = 3f;
    public LayerMask interactionLayer;

    [Header("Ссылки")]
    public Camera playerCamera;
    public Inventory inventorySystem;

    void Start()
    {
        // Автопоиск камеры
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
            if (playerCamera == null) playerCamera = Camera.main;
        }

        // Автопоиск инвентаря
        if (inventorySystem == null)
        {
            inventorySystem = GetComponent<Inventory>();
        }

        Debug.Log("✅ Interaction System готов");
    }

    void Update()
    {
        // Показываем подсказку при наведении
        ShowInteractionHint();

        // Обрабатываем взаимодействие
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }

        // Показываем инвентарь по нажатию I
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventorySystem != null)
            {
                inventorySystem.PrintInventory();
            }
        }

        // Дополнительные тестовые команды
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("=== ТЕКУЩИЙ ИНВЕНТАРЬ ===");
            if (inventorySystem != null)
            {
                Debug.Log($"Вес: {inventorySystem.GetCurrentWeight()}/{inventorySystem.maxWeight}кг");
                Debug.Log($"Слотов: {inventorySystem.GetUsedSlots()}/{inventorySystem.maxSlots}");
            }
        }
    }

    void ShowInteractionHint()
    {
        if (playerCamera == null) return;

        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            // Ищем ItemInteraction (новая система) или IInteractable
            ItemInteraction itemInteraction = hit.collider.GetComponent<ItemInteraction>();
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (itemInteraction != null && itemInteraction.item != null)
            {
                Debug.Log($"📦 [E] {itemInteraction.GetInteractionText()}");
            }
            else if (interactable != null)
            {
                Debug.Log($"📦 [E] {interactable.GetInteractionText()}");
            }
        }
    }

    void TryInteract()
    {
        if (playerCamera == null) return;

        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        Debug.Log("🔍 Поиск предметов для взаимодействия...");

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            Debug.Log($"🎯 Попал в: {hit.collider.gameObject.name}");

            // Ищем любой объект с IInteractable
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            ItemInteraction item = hit.collider.GetComponent<ItemInteraction>();

            Debug.Log($"📦 IInteractable найден: {interactable != null}");
            Debug.Log($"📦 ItemInteraction найден: {item != null}");

            if (interactable != null)
            {
                Debug.Log($"🔄 CanInteract: {interactable.CanInteract()}");
                if (interactable.CanInteract())
                {
                    interactable.Interact(gameObject);
                    Debug.Log("✅ Взаимодействие выполнено!");
                }
            }
        }
        else
        {
            Debug.Log("❌ Ничего не найдено в радиусе взаимодействия");
        }
    }
}