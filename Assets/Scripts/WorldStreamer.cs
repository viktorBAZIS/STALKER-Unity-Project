using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SeamlessWorldStreamer : MonoBehaviour
{
    [System.Serializable]
    public class LocationData
    {
        public string locationName;
        public GameObject locationPrefab;
        public Vector3 worldPosition;
        [System.NonSerialized] public GameObject loadedInstance;
        [System.NonSerialized] public float distanceToPlayer;
        [System.NonSerialized] public bool isLoaded = false;
        [System.NonSerialized] public bool isProcessing = false;
    }

    [Header("Streaming Settings")]
    public float loadDistance = 200f;
    public float unloadDistance = 300f;
    public int maxLoadedLocations = 3;
    public float updateInterval = 2.0f; // ТОЛЬКО 2 СЕКУНДЫ!

    [Header("Debug")]
    public bool showDebug = true;

    public List<LocationData> locations = new List<LocationData>();

    private Transform player;
    private Coroutine streamingCoroutine;
    private bool isInitialized = false;
    private float lastUpdateTime;

    void Start()
    {
        Debug.Log("🌍 World Streamer: Инициализация...");
        StartCoroutine(InitializeWithDelay());
    }

    IEnumerator InitializeWithDelay()
    {
        // Ждем пока все системы загрузятся
        yield return new WaitForSeconds(1f);

        FindPlayer();
        if (player != null)
        {
            streamingCoroutine = StartCoroutine(StreamingUpdateLoop());
            isInitialized = true;
            Debug.Log("✅ World Streamer: Запущен успешно");
        }
    }

    void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            var playerObj = GameObject.Find("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        if (player == null)
            Debug.LogError("❌ World Streamer: Игрок не найден!");
    }

    IEnumerator StreamingUpdateLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateInterval); // ВАЖНО: большой интервал

            if (player == null)
            {
                FindPlayer();
                continue;
            }

            // ОБНОВЛЯЕМ ВСЕ РАССТОЯНИЯ
            UpdateAllDistances();

            // ПРИНИМАЕМ РЕШЕНИЯ О ЗАГРУЗКЕ/ВЫГРУЗКЕ
            ProcessStreamingDecisions();

            if (showDebug)
                LogCurrentState();
        }
    }

    void UpdateAllDistances()
    {
        Vector3 playerPos = player.position;
        foreach (var location in locations)
        {
            if (location != null)
            {
                location.distanceToPlayer = Vector3.Distance(playerPos, location.worldPosition);
            }
        }
    }

    void ProcessStreamingDecisions()
    {
        int loadedCount = locations.Count(l => l != null && l.isLoaded);

        // СОРТИРУЕМ ПО РАССТОЯНИЮ (ближайшие первые)
        var sortedLocations = locations
            .Where(l => l != null)
            .OrderBy(l => l.distanceToPlayer)
            .ToList();

        // ВЫГРУЖАЕМ ДАЛЕКИЕ ЛОКАЦИИ
        foreach (var location in sortedLocations)
        {
            if (location.isLoaded && location.distanceToPlayer > unloadDistance)
            {
                if (!location.isProcessing)
                {
                    StartCoroutine(UnloadLocationAsync(location));
                }
            }
        }

        // ЗАГРУЖАЕМ БЛИЗКИЕ ЛОКАЦИИ
        foreach (var location in sortedLocations)
        {
            if (!location.isLoaded && location.distanceToPlayer < loadDistance && loadedCount < maxLoadedLocations)
            {
                if (!location.isProcessing)
                {
                    StartCoroutine(LoadLocationAsync(location));
                    loadedCount++;

                    if (loadedCount >= maxLoadedLocations)
                        break;
                }
            }
        }
    }

    IEnumerator LoadLocationAsync(LocationData location)
    {
        if (location.isProcessing || location.isLoaded) yield break;

        location.isProcessing = true;

        if (showDebug)
            Debug.Log($"🔄 Загрузка: {location.locationName}");

        yield return new WaitForSeconds(0.5f); // ЗАДЕРЖКА ДЛЯ СТАБИЛЬНОСТИ

        try
        {
            if (location.locationPrefab != null)
            {
                location.loadedInstance = Instantiate(location.locationPrefab, location.worldPosition, Quaternion.identity);
                location.loadedInstance.name = $"[STREAMED] {location.locationName}";
                location.isLoaded = true;

                if (showDebug)
                    Debug.Log($"✅ Загружено: {location.locationName}");
            }
            else
            {
                Debug.LogError($"❌ Нет префаба для: {location.locationName}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Ошибка загрузки {location.locationName}: {e.Message}");
        }
        finally
        {
            location.isProcessing = false;
        }
    }

    IEnumerator UnloadLocationAsync(LocationData location)
    {
        if (location.isProcessing || !location.isLoaded) yield break;

        location.isProcessing = true;

        if (showDebug)
            Debug.Log($"🗑️ Выгрузка: {location.locationName}");

        yield return new WaitForSeconds(0.3f); // ЗАДЕРЖКА

        try
        {
            if (location.loadedInstance != null)
            {
                Destroy(location.loadedInstance);
                location.loadedInstance = null;
            }
            location.isLoaded = false;

            if (showDebug)
                Debug.Log($"✅ Выгружено: {location.locationName}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Ошибка выгрузки {location.locationName}: {e.Message}");
        }
        finally
        {
            location.isProcessing = false;
        }
    }

    void LogCurrentState()
    {
        int loaded = locations.Count(l => l != null && l.isLoaded);
        int loading = locations.Count(l => l != null && l.isProcessing);

        string loadedNames = string.Join(", ",
            locations.Where(l => l != null && l.isLoaded)
                     .Select(l => l.locationName));

        Debug.Log($"📊 WorldStreamer: {loaded} загружено, {loading} в процессе | {loadedNames}");
    }

    void OnDestroy()
    {
        // АККУРАТНАЯ ОСТАНОВКА
        if (streamingCoroutine != null)
        {
            StopCoroutine(streamingCoroutine);
            streamingCoroutine = null;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!showDebug || player == null) return;

        // Визуализация зон
        Gizmos.color = new Color(0, 1, 0, 0.1f);
        Gizmos.DrawSphere(player.position, loadDistance);

        Gizmos.color = new Color(1, 0, 0, 0.1f);
        Gizmos.DrawSphere(player.position, unloadDistance);

        // Визуализация локаций
        foreach (var location in locations)
        {
            if (location == null) continue;

            Gizmos.color = location.isLoaded ? Color.green :
                          location.isProcessing ? Color.yellow : Color.gray;

            Gizmos.DrawWireCube(location.worldPosition, Vector3.one * 25f);
        }
    }
}