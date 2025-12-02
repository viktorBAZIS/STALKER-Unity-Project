using UnityEngine;

public class SystemIntegrationTest : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public ArmsManager armsManager;
    public PlayerHealth playerHealth;
    public Inventory inventory;
    public StalkerAimSystem aimSystem;
    
    void Start()
    {
        Debug.Log("🎯 STALKER SYSTEMS INTEGRATION TEST");
        Debug.Log("=====================================");
        
        // Автопоиск если ссылки не установлены
        if (player == null) player = GameObject.Find("Player");
        if (armsManager == null) armsManager = FindObjectOfType<ArmsManager>();
        if (playerHealth == null) playerHealth = FindObjectOfType<PlayerHealth>();
        if (inventory == null) inventory = FindObjectOfType<Inventory>();
        if (aimSystem == null) aimSystem = FindObjectOfType<StalkerAimSystem>();
        
        // Проверка систем
        CheckSystem("Player", player);
        CheckSystem("Arms Manager", armsManager);
        CheckSystem("Player Health", playerHealth);
        CheckSystem("Inventory", inventory);
        CheckSystem("Aim System", aimSystem);
        
        // Проверка связей между системами
        CheckSystemConnections();
        
        Debug.Log("✅ TEST COMPLETE");
    }
    
    void CheckSystem(string systemName, Object system)
    {
        if (system != null)
            Debug.Log($"✅ {systemName}: FOUND");
        else
            Debug.Log($"❌ {systemName}: MISSING");
    }
    
    void CheckSystemConnections()
    {
        Debug.Log("--- SYSTEM CONNECTIONS ---");
        
        // Проверяем связи ArmsManager с другими системами
        if (armsManager != null)
        {
            // Проверяем ссылку на PlayerHealth для дрожания рук
            // Это нужно настроить если еще нет
        }
        
        // Проверяем связи PlayerHealth с HUD
        if (playerHealth != null)
        {
            var hudManager = FindObjectOfType<HUDManager>();
            if (hudManager != null && hudManager.playerHealth != null)
                Debug.Log("✅ Health → HUD: CONNECTED");
            else
                Debug.Log("❌ Health → HUD: NOT CONNECTED");
        }
    }
}