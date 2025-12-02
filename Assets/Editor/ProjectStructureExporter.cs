using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ProjectStructureExporter
{
#if UNITY_EDITOR
    [MenuItem("STALKER Tools/📁 Export Project Structure")]
    public static void ExportProjectStructure()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("🎯 STALKER UNITY PROJECT STRUCTURE");
        sb.AppendLine("Generated: " + System.DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
        sb.AppendLine("======================================");
        sb.AppendLine();
        
        ExportAssetsFolder(sb);
        
        // Добавляем анализ изменений
        AppendChangeAnalysis(sb);
        
        string filePath = Path.Combine(Application.dataPath, "../PROJECT_STRUCTURE.txt");
        File.WriteAllText(filePath, sb.ToString());
        
        Debug.Log("✅ Project structure exported to: " + filePath);
        EditorUtility.RevealInFinder(filePath);
        Debug.Log(sb.ToString());
    }
    
    static void ExportAssetsFolder(StringBuilder sb)
    {
        string assetsPath = Application.dataPath;
        DirectoryInfo assetsDir = new DirectoryInfo(assetsPath);
        
        sb.AppendLine("📁 Assets/");
        
        // Собираем статистику
        var stats = new FileStats();
        
        // Получаем все папки в Assets
        foreach (var dir in assetsDir.GetDirectories())
        {
            if (ShouldIgnoreFolder(dir.Name)) continue;
            ExportFolder(dir, sb, 1, stats);
        }
        
        // Получаем файлы в корне Assets
        foreach (var file in assetsDir.GetFiles())
        {
            if (IsRelevantFile(file.Extension))
            {
                sb.AppendLine("  " + GetFileIcon(file.Extension) + " " + file.Name + GetFileInfo(file));
                stats.CountFile(file);
            }
        }
        
        // Добавляем статистику
        sb.AppendLine();
        sb.AppendLine("📊 СТАТИСТИКА ПРОЕКТА:");
        sb.AppendLine($"• Скрипты C#: {stats.scriptCount} файлов");
        sb.AppendLine($"• Префабы: {stats.prefabCount} файлов");
        sb.AppendLine($"• Сцены: {stats.sceneCount} файлов");
        sb.AppendLine($"• Всего файлов: {stats.totalCount}");
        sb.AppendLine($"• Последнее изменение: {stats.lastModified:dd.MM.yyyy HH:mm}");
    }
    
    static void ExportFolder(DirectoryInfo dir, StringBuilder sb, int indent, FileStats stats)
    {
        string indentStr = new string(' ', indent * 2);
        
        sb.AppendLine(indentStr + "📁 " + dir.Name + "/");
        
        // Файлы в папке
        foreach (var file in dir.GetFiles())
        {
            if (IsRelevantFile(file.Extension))
            {
                sb.AppendLine(indentStr + "  " + GetFileIcon(file.Extension) + " " + file.Name + GetFileInfo(file));
                stats.CountFile(file);
            }
        }
        
        // Подпапки
        foreach (var subDir in dir.GetDirectories())
        {
            if (ShouldIgnoreFolder(subDir.Name)) continue;
            ExportFolder(subDir, sb, indent + 1, stats);
        }
    }
    
    static void AppendChangeAnalysis(StringBuilder sb)
    {
        sb.AppendLine();
        sb.AppendLine("🔍 АНАЛИЗ ИЗМЕНЕНИЙ:");
        
        // Анализ критических систем
        string[] criticalSystems = {
            "PlayerHealth", "StalkerAimSystem", "Inventory", 
            "ItemManager", "HUDManager", "PlayerInteraction"
        };
        
        List<string> missingSystems = new List<string>();
        List<string> foundSystems = new List<string>();
        
        foreach (var system in criticalSystems)
        {
            string[] files = Directory.GetFiles(Application.dataPath, $"{system}.cs", SearchOption.AllDirectories);
        if (files.Length > 0)
                foundSystems.Add(system);
            else
                missingSystems.Add(system);
        }
        
        sb.AppendLine("✅ Найдены системы: " + string.Join(", ", foundSystems));
        if (missingSystems.Count > 0)
            sb.AppendLine("❌ Отсутствуют: " + string.Join(", ", missingSystems));
        
        // Проверка архитектурных связей
        sb.AppendLine();
        sb.AppendLine("🏗️ АРХИТЕКТУРНЫЕ СВЯЗИ:");
        CheckArchitectureLinks(sb);
    }
    
    static void CheckArchitectureLinks(StringBuilder sb)
    {
        // Проверяем наличие ключевых компонентов на сцене
        if (GameObject.Find("PlayerSystems") != null)
            sb.AppendLine("✅ PlayerSystems объект на сцене");
        else
            sb.AppendLine("❌ PlayerSystems объект отсутствует на сцене");
            
        if (GameObject.FindObjectOfType<Canvas>() != null)
            sb.AppendLine("✅ UI Canvas присутствует");
        else
            sb.AppendLine("❌ UI Canvas отсутствует");
    }
    
    static string GetFileIcon(string extension)
    {
        switch (extension.ToLower())
        {
            case ".cs": return "📄";
            case ".prefab": return "🎭";
            case ".unity": return "🎬";
            case ".asset": return "💾";
            case ".mat": return "🎨";
            case ".png": case ".jpg": case ".psd": return "🖼️";
            case ".fbx": case ".obj": return "🔺";
            default: return "📎";
        }
    }
    
    static string GetFileInfo(FileInfo file)
    {
        List<string> info = new List<string>();
        
        // Размер файла
        if (file.Length > 1024)
            info.Add($"{file.Length / 1024}KB");
        
        // Время изменения
        if ((System.DateTime.Now - file.LastWriteTime).TotalDays < 1)
            info.Add("сегодня");
        else if ((System.DateTime.Now - file.LastWriteTime).TotalDays < 7)
            info.Add("недавно");
        
        return info.Count > 0 ? $" ({string.Join(", ", info)})" : "";
    }
    
    static bool IsRelevantFile(string extension)
    {
        string[] relevant = { ".cs", ".prefab", ".unity", ".asset", ".mat" };
        return relevant.Contains(extension.ToLower());
    }
    
    static bool ShouldIgnoreFolder(string folderName)
    {
        string[] ignored = { 
            "Library", "Logs", "Temp", "Obj", 
            "Build", "Builds", ".git", "MonoBleedingEdge" 
        };
        return ignored.Contains(folderName);
    }
    
    class FileStats
    {
        public int scriptCount = 0;
        public int prefabCount = 0;
        public int sceneCount = 0;
        public int totalCount = 0;
        public System.DateTime lastModified = System.DateTime.MinValue;
        
        public void CountFile(FileInfo file)
        {
            totalCount++;
            
            switch (file.Extension.ToLower())
            {
                case ".cs": scriptCount++; break;
                case ".prefab": prefabCount++; break;
                case ".unity": sceneCount++; break;
            }
            
            if (file.LastWriteTime > lastModified)
                lastModified = file.LastWriteTime;
        }
    }
#endif
}