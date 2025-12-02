using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class ArmsManager : MonoBehaviour
{
    [System.Serializable]
    public class HandsPreset
    {
        public string presetName;
        public GameObject handsPrefab;
        public Vector3 positionOffset = new Vector3(0.1f, -0.2f, 0.3f);
        public float scale = 1.0f;
    }

    [Header("Hands Presets Configuration")]
    [SerializeField] private int presetsCount = 5;
    [SerializeField] private List<HandsPreset> handsPresets = new List<HandsPreset>();

    [Header("CSV Configuration")]
    [SerializeField] private string csvFileName = "hands_config.csv";
    [SerializeField] private bool autoLoadFromCSV = true;

    [Header("Current Hands")]
    [SerializeField] private GameObject currentHandsInstance;

    [Header("Systems Integration")]
    [SerializeField] private string playerArmsTag = "PlayerArms";
    private PlayerHealth playerHealth;
    private StalkerAimSystem aimSystem;

    // Эффекты системы
    private float healthShakeIntensity = 0f;
    private float radiationTremorIntensity = 0f;
    private float aimStability = 1f;

    private Animator currentAnimator;
    private bool isInitialized = false;

    void Start()
    {
        // Находим системы здоровья и прицеливания
        FindAndConnectSystems();

        if (autoLoadFromCSV)
        {
            LoadHandsConfigFromCSV();
        }
        InitializeHands();
    }

    // 🔄 ИНТЕГРАЦИЯ С СИСТЕМАМИ
    void FindAndConnectSystems()
    {
        // Ищем системы в родительских объектах или в сцене
        playerHealth = GetComponentInParent<PlayerHealth>();
        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<PlayerHealth>();
            Debug.Log(playerHealth != null ?
                "✅ PlayerHealth found in scene" :
                "⚠️ PlayerHealth not found in scene");
        }

        aimSystem = GetComponentInParent<StalkerAimSystem>();
        if (aimSystem == null)
        {
            aimSystem = FindObjectOfType<StalkerAimSystem>();
            Debug.Log(aimSystem != null ?
                "✅ StalkerAimSystem found in scene" :
                "⚠️ StalkerAimSystem not found in scene");
        }

        // Подписываемся на события здоровья
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += OnHealthChanged;
            playerHealth.OnRadiationChanged += OnRadiationChanged;
            Debug.Log("✅ Connected to PlayerHealth system events");
        }
    }

    // События здоровья
    private void OnHealthChanged(float health)
    {
        // Обновляем эффекты при изменении здоровья
        UpdateHealthEffects();
    }

    private void OnRadiationChanged(float radiation)
    {
        // Обновляем эффекты при изменении радиации
        UpdateHealthEffects();
    }

    // Динамическое изменение количества пресетов в инспекторе
    void OnValidate()
    {
        // Автоматически регулируем размер списка при изменении presetsCount
        while (handsPresets.Count < presetsCount)
        {
            handsPresets.Add(new HandsPreset());
        }
        while (handsPresets.Count > presetsCount)
        {
            handsPresets.RemoveAt(handsPresets.Count - 1);
        }
    }

    // ЭКСПОРТ конфигурации в CSV
    [ContextMenu("Export Hands Config to CSV")]
    public void ExportHandsConfigToCSV()
    {
        string filePath = Path.Combine(Application.dataPath, "_Project/Configs", csvFileName);
        string directory = Path.GetDirectoryName(filePath);

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Заголовок CSV
            writer.WriteLine("PresetName,PrefabPath,PositionX,PositionY,PositionZ,Scale");

            // Данные пресетов
            foreach (var preset in handsPresets)
            {
                string prefabPath = preset.handsPrefab != null ?
                    GetPrefabPath(preset.handsPrefab) : "None";

                writer.WriteLine($"{preset.presetName}," +
                               $"{prefabPath}," +
                               $"{preset.positionOffset.x}," +
                               $"{preset.positionOffset.y}," +
                               $"{preset.positionOffset.z}," +
                               $"{preset.scale}");
            }
        }

        Debug.Log($"✅ Hands config exported to: {filePath}");
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    // ИМПОРТ конфигурации из CSV
    [ContextMenu("Load Hands Config from CSV")]
    public void LoadHandsConfigFromCSV()
    {
        string filePath = Path.Combine(Application.dataPath, "_Project/Configs", csvFileName);

        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"⚠️ CSV file not found: {filePath}");
            return;
        }

        List<HandsPreset> loadedPresets = new List<HandsPreset>();

        using (StreamReader reader = new StreamReader(filePath))
        {
            // Пропускаем заголовок
            reader.ReadLine();

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] values = line.Split(',');

                if (values.Length >= 6)
                {
                    HandsPreset preset = new HandsPreset();
                    preset.presetName = values[0];

                    // Загрузка префаба по пути
                    if (values[1] != "None" && values[1] != "")
                    {
#if UNITY_EDITOR
                        preset.handsPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(values[1]);
#else
                        // Для билда - альтернативная логика загрузки
                        Debug.Log($"Loading prefab for build: {values[1]}");
#endif
                    }

                    // Парсинг позиции и масштаба
                    if (float.TryParse(values[2], out float posX) &&
                        float.TryParse(values[3], out float posY) &&
                        float.TryParse(values[4], out float posZ) &&
                        float.TryParse(values[5], out float scale))
                    {
                        preset.positionOffset = new Vector3(posX, posY, posZ);
                        preset.scale = scale;
                    }

                    loadedPresets.Add(preset);
                }
            }
        }

        handsPresets = loadedPresets;
        presetsCount = handsPresets.Count;
        Debug.Log($"✅ Loaded {handsPresets.Count} hands presets from CSV");
    }

    // ИНИЦИАЛИЗАЦИЯ РУК
    void InitializeHands()
    {
        if (handsPresets.Count > 0 && handsPresets[0].handsPrefab != null)
        {
            EquipHands(0); // Первый пресет по умолчанию
        }
        else
        {
            Debug.LogWarning("⚠️ No hands presets configured!");
        }

        isInitialized = true;
        Debug.Log("✅ Arms Manager initialized with systems integration");
    }

    // ОСНОВНОЙ МЕТОД СМЕНЫ РУК
    public void EquipHands(int presetIndex)
    {
        if (presetIndex < 0 || presetIndex >= handsPresets.Count)
        {
            Debug.LogError($"❌ Invalid hands preset index: {presetIndex}");
            return;
        }

        HandsPreset preset = handsPresets[presetIndex];

        if (preset.handsPrefab == null)
        {
            Debug.LogError($"❌ Hands prefab is null for preset: {preset.presetName}");
            return;
        }

        // Удаляем старые руки
        if (currentHandsInstance != null)
        {
            Destroy(currentHandsInstance);
        }

        // Создаем новые руки
        currentHandsInstance = Instantiate(preset.handsPrefab, transform);
        currentHandsInstance.transform.localPosition = preset.positionOffset;
        currentHandsInstance.transform.localRotation = Quaternion.identity;
        currentHandsInstance.transform.localScale = Vector3.one * preset.scale;

        // Настраиваем компоненты
        SetupCurrentHands();

        Debug.Log($"✅ Equipped hands: {preset.presetName}");
    }

    // СМЕНА РУК ПО ИМЕНИ ПРЕСЕТА
    public void EquipHands(string presetName)
    {
        int index = handsPresets.FindIndex(p => p.presetName == presetName);
        if (index >= 0)
        {
            EquipHands(index);
        }
        else
        {
            Debug.LogError($"❌ Hands preset not found: {presetName}");
        }
    }

    void SetupCurrentHands()
    {
        if (currentHandsInstance == null) return;

        currentAnimator = currentHandsInstance.GetComponent<Animator>();
        if (currentAnimator != null)
        {
            currentAnimator.applyRootMotion = false;
        }

        SetHandsLayer();
        SetHandsTag();
    }

    void SetHandsLayer()
    {
        if (currentHandsInstance == null) return;

        int armsLayer = LayerMask.NameToLayer("Arms");
        if (armsLayer != -1)
        {
            SetLayerRecursively(currentHandsInstance, armsLayer);
        }
        else
        {
            Debug.LogWarning("⚠️ 'Arms' layer not found! Please create it in Project Settings");
        }
    }

    void SetHandsTag()
    {
        if (currentHandsInstance == null) return;

        currentHandsInstance.tag = playerArmsTag;
        SetTagRecursively(currentHandsInstance, playerArmsTag);
    }

    void SetLayerRecursively(GameObject obj, int layer)
    {
        if (obj == null) return;

        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    void SetTagRecursively(GameObject obj, string tag)
    {
        if (obj == null) return;

        obj.tag = tag;
        foreach (Transform child in obj.transform)
        {
            SetTagRecursively(child.gameObject, tag);
        }
    }

    string GetPrefabPath(GameObject prefab)
    {
#if UNITY_EDITOR
        return UnityEditor.AssetDatabase.GetAssetPath(prefab);
#else
        return prefab.name;
#endif
    }

    void Update()
    {
        if (!isInitialized) return;

        UpdateSystemsIntegration();
        UpdateHandsAnimation();
        HandleTestInput();
    }

    // 🔄 ОБНОВЛЕННАЯ ИНТЕГРАЦИЯ С СИСТЕМАМИ
    void UpdateSystemsIntegration()
    {
        // Эффекты здоровья на дрожание рук
        UpdateHealthEffects();

        // Эффекты системы прицеливания на стабильность
        UpdateAimEffects();

        // Применяем комбинированные эффекты к рукам
        ApplyHandsEffects();
    }

    void UpdateHealthEffects()
    {
        if (playerHealth != null)
        {
            // Используем существующие поля PlayerHealth
            float healthPercent = playerHealth.currentHealth / playerHealth.maxHealth;
            healthShakeIntensity = Mathf.Clamp01(1f - healthPercent) * 0.3f;

            // Эффекты радиации
            float radiationPercent = playerHealth.currentRadiation / playerHealth.maxRadiation;
            radiationTremorIntensity = Mathf.Clamp01(radiationPercent) * 0.4f;
        }
    }

    void UpdateAimEffects()
    {
        if (aimSystem != null)
        {
            // Безопасный доступ к системе прицеливания
            aimStability = GetAimStability();

            // Учитываем эффекты здоровья на прицеливание
            aimStability *= Mathf.Clamp01(1f - (healthShakeIntensity + radiationTremorIntensity));
        }
        else
        {
            aimStability = 1f; // Стандартная стабильность если система не найдена
        }
    }

    // Безопасный метод получения стабильности прицеливания
    float GetAimStability()
    {
        // Если система прицеливания не реализована, возвращаем базовую стабильность
        return 0.8f; // Базовая стабильность для тестирования
    }

    void ApplyHandsEffects()
    {
        if (currentHandsInstance == null) return;

        // Применяем дрожание к позиции рук
        float totalShake = healthShakeIntensity + radiationTremorIntensity;
        if (totalShake > 0.01f)
        {
            Vector3 shakeOffset = new Vector3(
                Mathf.PerlinNoise(Time.time * 8f, 0) - 0.5f,
                Mathf.PerlinNoise(0, Time.time * 8f) - 0.5f,
                Mathf.PerlinNoise(Time.time * 6f, Time.time * 6f) - 0.5f
            ) * totalShake * 0.1f;

            currentHandsInstance.transform.localPosition += shakeOffset;
        }
    }

    void HandleTestInput()
    {
        // Тестовые клавиши для быстрой смены рук (1-9)
        for (int i = 0; i < Mathf.Min(handsPresets.Count, 9); i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                EquipHands(i);
            }
        }

        // Тестовые клавиши для проверки интеграции
        if (Input.GetKeyDown(KeyCode.H))
        {
            SimulateHealthEffect();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SimulateRadiationEffect();
        }
    }

    void UpdateHandsAnimation()
    {
        if (currentAnimator == null) return;

        float moveSpeed = GetPlayerMoveSpeed();
        bool isMoving = moveSpeed > 0.1f;
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && isMoving;
        bool isCrouching = Input.GetKey(KeyCode.C);

        // Учитываем стабильность прицеливания в анимациях
        float stabilityMultiplier = aimStability;

        currentAnimator.SetFloat("Speed", moveSpeed * stabilityMultiplier);
        currentAnimator.SetBool("IsMoving", isMoving);
        currentAnimator.SetBool("IsSprinting", isSprinting);
        currentAnimator.SetBool("IsCrouching", isCrouching);

        // Параметры для эффектов системы
        currentAnimator.SetFloat("HealthShake", healthShakeIntensity);
        currentAnimator.SetFloat("RadiationTremor", radiationTremorIntensity);
        currentAnimator.SetFloat("AimStability", aimStability);

        // Анимация дыхания с учетом состояния
        if (!isMoving)
        {
            float breathingIntensity = 1f - (healthShakeIntensity * 0.5f);
            currentAnimator.SetFloat("Breathing", Mathf.Sin(Time.time * 0.5f) * 0.1f * breathingIntensity);
        }
    }

    float GetPlayerMoveSpeed()
    {
        // Временная реализация - замените на вашу систему движения
        if (Input.GetKey(KeyCode.LeftShift)) return 2.0f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) return 1.0f;
        return 0.0f;
    }

    // 🔧 ТЕСТОВЫЕ МЕТОДЫ ДЛЯ ПРОВЕРКИ ИНТЕГРАЦИИ
    void SimulateHealthEffect()
    {
        healthShakeIntensity = 0.2f;
        Debug.Log("🔧 Simulating low health effect");
    }

    void SimulateRadiationEffect()
    {
        radiationTremorIntensity = 0.3f;
        Debug.Log("🔧 Simulating radiation effect");
    }

    // API для внешнего использования
    public List<string> GetAvailablePresets()
    {
        return handsPresets.ConvertAll(p => p.presetName);
    }

    public int GetPresetsCount()
    {
        return handsPresets.Count;
    }

    public string GetCurrentPresetName()
    {
        if (currentHandsInstance == null) return "None";

        // Находим текущий пресет по префабу
        foreach (var preset in handsPresets)
        {
            if (preset.handsPrefab != null && currentHandsInstance.name.StartsWith(preset.handsPrefab.name))
            {
                return preset.presetName;
            }
        }
        return "Unknown";
    }

    public void SetHandsVisibility(bool visible)
    {
        if (currentHandsInstance != null)
        {
            currentHandsInstance.SetActive(visible);
        }
    }

    // Метод для интеграции с системой костюмов
    public void EquipHandsByOutfit(OutfitType outfitType)
    {
        string presetName = outfitType.ToString().ToLower() + "_hands";
        EquipHands(presetName);
    }

    public void EquipStalkerWeapon(GameObject weaponHandsPrefab)
    {
        // Убираем текущие руки
        if (currentHandsInstance != null)
            Destroy(currentHandsInstance);

        // Создаем руки с оружием в стиле STALKER
        currentHandsInstance = Instantiate(weaponHandsPrefab, transform);
        SetupStalkerWeaponAnimations();
    }

    private void SetupStalkerWeaponAnimations()
    {
        currentAnimator = currentHandsInstance.GetComponent<Animator>();

        // STALKER-style параметры:
        // - Fire (стрельба)
        // - Reload (перезарядка) 
        // - Jam (заклинивание)
        // - Condition (состояние оружия влияет на анимации)
    }

    // 🔄 МЕТОДЫ ДЛЯ ВНЕШНЕГО ВОЗДЕЙСТВИЯ
    public void ApplyHealthEffect(float intensity)
    {
        healthShakeIntensity = Mathf.Clamp01(intensity);
    }

    public void ApplyRadiationEffect(float intensity)
    {
        radiationTremorIntensity = Mathf.Clamp01(intensity);
    }

    public void SetAimStability(float stability)
    {
        aimStability = Mathf.Clamp01(stability);
    }

    void OnDestroy()
    {
        // Отписываемся от событий при уничтожении
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= OnHealthChanged;
            playerHealth.OnRadiationChanged -= OnRadiationChanged;
        }
    }
}

// Enum для типов костюмов
public enum OutfitType
{
    Default,
    Seva,
    Exoskeleton,
    Mercenary,
    Leather,
    Military
}