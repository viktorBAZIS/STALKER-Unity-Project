using UnityEngine;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;
    
    [Header("Item Database")]
    public ItemData[] allItems;
    
    private Dictionary<string, ItemData> itemDictionary = new Dictionary<string, ItemData>();
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDatabase();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeDatabase()
    {
        foreach (ItemData item in allItems)
        {
            if (item != null && !string.IsNullOrEmpty(item.itemID))
            {
                if (!itemDictionary.ContainsKey(item.itemID))
                {
                    itemDictionary.Add(item.itemID, item);
                }
                else
                {
                    Debug.LogWarning($"Duplicate item ID: {item.itemID}");
                }
            }
        }
        Debug.Log($"Item database initialized with {itemDictionary.Count} items");
    }
    
    public ItemData GetItemByID(string itemID)
    {
        if (itemDictionary.ContainsKey(itemID))
            return itemDictionary[itemID];
        
        Debug.LogWarning($"Item with ID {itemID} not found!");
        return null;
    }
    
    public bool ItemExists(string itemID)
    {
        return itemDictionary.ContainsKey(itemID);
    }
    
    // Для создания предметов в редакторе
    #if UNITY_EDITOR
    [ContextMenu("Load All Items From Resources")]
    void LoadAllItemsFromResources()
    {
        allItems = Resources.LoadAll<ItemData>("Items");
        InitializeDatabase();
    }
    #endif
}