using UnityEngine;
using System.Collections;

public class SimpleLocationLoader : MonoBehaviour
{
    public GameObject testLocation; // Перетащи одну тестовую локацию
    private GameObject loadedLocation;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        if (testLocation != null)
        {
            loadedLocation = Instantiate(testLocation, Vector3.zero, Quaternion.identity);
            Debug.Log("✅ Тестовая локация загружена");
            
            // Телепортируем игрока на локацию
            if (player != null)
            {
                player.position = new Vector3(0, 5, 0);
                Debug.Log("🎮 Игрок телепортирован на тестовую локацию");
            }
        }
    }

    void Update()
    {
        // Простое переключение локаций для теста
        if (Input.GetKeyDown(KeyCode.F1) && loadedLocation != null)
        {
            Destroy(loadedLocation);
            Debug.Log("🗑️ Локация выгружена");
        }
        
        if (Input.GetKeyDown(KeyCode.F2) && testLocation != null)
        {
            loadedLocation = Instantiate(testLocation, Vector3.zero, Quaternion.identity);
            Debug.Log("✅ Локация загружена");
        }
    }
}