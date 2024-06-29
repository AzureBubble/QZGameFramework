using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.U2D;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.U2D;

public class AutoSetSpriteTag : AssetPostprocessor
{
    private static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        // 导入资源
        foreach (var asset in importedAssets)
        {
            EditorSpriteAtlases.OnImportSprite(asset);
        }
        // 删除资源
        foreach (var asset in deletedAssets)
        {
            EditorSpriteAtlases.OnDeleteSprite(asset);
        }

        EditorSpriteAtlases.tempSprites = new Object[movedAssets.Length];
        EditorSpriteAtlases.index = 0;
        // 移动资源后
        foreach (var asset in movedAssets)
        {
            EditorSpriteAtlases.OnImportSprite(asset, true);
        }
        EditorSpriteAtlases.index = 0;
        // 移动资源前
        foreach (var asset in movedFromAssetPaths)
        {
            EditorSpriteAtlases.OnDeleteSprite(asset, true);
        }
        EditorSpriteAtlases.ForceRebuildSpriteAtlas();
        EditorSpriteAtlases.SaveSpriteMapInfo();
    }
}

[InitializeOnLoad]
public static class EditorSpriteAtlases
{
    private static bool isABLoad = false;
    private const string SPRITE_PATH = "Assets/ArtRes/"; // UI 资源导入根路径
    private const string SPRITE_ATLASES_PATH = "Assets/Resources/SpriteAtlases/"; // 图集存储根路径
    private const string SPRITE_ATLASES_ABPATH = "Assets/SpriteAtlases/"; // 图集存储根路径

    // 待删除的图片
    private static Dictionary<string, Object> delSprites = new Dictionary<string, Object>();

    public static Object[] tempSprites;
    public static int index;

    private static Dictionary<string, string> spriteMap = new Dictionary<string, string>(); // 图片映射图集
    private static bool isNeedSave = false;

    static EditorSpriteAtlases()
    {
    }

    private static bool IsIgnorePath(string path)
    {
        AssetDatabase.AssetPathToGUID(path);
        if (string.IsNullOrEmpty(path))
        {
            return true;
        }

        if (path.StartsWith(SPRITE_PATH))
        {
            if (path.EndsWith(".png"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return true;
        }
    }

    //[MenuItem("GameTool/强制重新生成所有图集资源")]
    public static void ForceReBuildSpriteAtlases(string path)
    {
    }

    /// <summary>
    /// Unity资源导入时
    /// </summary>
    /// <param name="path"></param>
    public static void OnImportSprite(string path, bool isMove = false)
    {
        if (IsIgnorePath(path))
        {
            return;
        }

        // 图片进行格式化设置
        TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
        if (ti != null && ti.textureType != TextureImporterType.Sprite)
        {
            TextureFormatTools.FormatSprite(ti, path);
            return;
        }

        string dirName = Path.GetDirectoryName(path).Replace("\\", "/").Replace(SPRITE_PATH, "");
        string spriteAtlasName = dirName;
        if (spriteAtlasName.Contains('/'))
        {
            spriteAtlasName = spriteAtlasName.Replace('/', '_');
        }
        string spriteName = Path.GetFileNameWithoutExtension(path);
        string spriteAtlasPath = Path.Combine(SPRITE_ATLASES_PATH, $"{spriteAtlasName}.spriteatlas");
        if (isABLoad)
        {
            // AB包存放路径
            spriteAtlasPath = Path.Combine(SPRITE_ATLASES_ABPATH, $"{spriteAtlasName}.spriteatlas");
            if (!Directory.Exists(SPRITE_ATLASES_ABPATH))
            {
                Directory.CreateDirectory(SPRITE_ATLASES_ABPATH);
            }
        }
        SpriteAtlas spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(spriteAtlasPath);
        if (spriteAtlas == null)
        {
            spriteAtlas = new SpriteAtlas();
            SpriteAtlasTextureSettings textureSettings = new SpriteAtlasTextureSettings()
            {
                filterMode = FilterMode.Bilinear,
                sRGB = true,
            };
            spriteAtlas.SetTextureSettings(textureSettings);
            SpriteAtlasPackingSettings packingSettings = new SpriteAtlasPackingSettings()
            {
                blockOffset = 1,
                enableRotation = true,
                enableTightPacking = false,
                padding = 2
            };
            spriteAtlas.SetPackingSettings(packingSettings);

            if (!Directory.Exists(SPRITE_ATLASES_PATH))
            {
                Directory.CreateDirectory(SPRITE_ATLASES_PATH);
            }
            AssetDatabase.CreateAsset(spriteAtlas, spriteAtlasPath);
            Debug.Log($"Create new sprite atlas success: {spriteAtlasPath}.");
        }

        Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);

        if (isMove)
        {
            tempSprites[index] = asset;
        }

        if (asset != null)
        {
            spriteMap[spriteName] = spriteAtlasName;
            SpriteAtlasExtensions.Add(spriteAtlas, new[] { asset });
            isNeedSave = true;
            Debug.Log($"Added {spriteName} to sprite altas: {spriteAtlasName}.");
        }

        EditorUtility.SetDirty(spriteAtlas);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static void SaveSpriteMapInfo()
    {
        if (!isNeedSave) return;

        string spriteMapSavePath = Application.dataPath + "/Resources/SpriteConfig/";
        string json = JsonConvert.SerializeObject(spriteMap);
        if (!Directory.Exists(spriteMapSavePath))
        {
            Directory.CreateDirectory(spriteMapSavePath);
        }
        File.WriteAllText(spriteMapSavePath + "SpriteConfig.json", json);
        isNeedSave = false;

        AssetDatabase.Refresh();
    }

    /// <summary>
    /// Unity 资源删除时
    /// </summary>
    /// <param name="path"></param>
    public static void OnDeleteSprite(string path, bool isMove = false)
    {
        if (IsIgnorePath(path))
        {
            return;
        }
        string dirName = Path.GetDirectoryName(path).Replace("\\", "/").Replace(SPRITE_PATH, "");
        string spriteAtlasName = dirName;
        if (spriteAtlasName.Contains('/'))
        {
            spriteAtlasName = spriteAtlasName.Replace('/', '_');
        }
        string spriteName = Path.GetFileNameWithoutExtension(path);
        string spriteAtlasPath = Path.Combine(SPRITE_ATLASES_PATH, $"{spriteAtlasName}.spriteatlas");
        if (isABLoad)
        {
            // AB包存放路径
            spriteAtlasPath = Path.Combine(SPRITE_ATLASES_ABPATH, $"{spriteAtlasName}.spriteatlas");
        }
        SpriteAtlas spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(spriteAtlasPath);

        if (spriteAtlas != null)
        {
            if (isMove)
            {
                delSprites.Add(spriteAtlasPath, tempSprites[index]);
            }
            SpriteAtlasUtility.PackAtlases(new[] { spriteAtlas }, EditorUserBuildSettings.activeBuildTarget);

            SerializedObject so = new SerializedObject(spriteAtlas);
            SerializedProperty sp = so.FindProperty("m_PackedSprites");
            if (sp.arraySize <= 0 && File.Exists(spriteAtlasPath))
            {
                AssetDatabase.DeleteAsset(spriteAtlasPath);
            }
        }

        if (spriteMap.ContainsKey(spriteName) && spriteMap[spriteName] == spriteAtlasName)
        {
            spriteMap.Remove(spriteName);
            isNeedSave = true;
        }

        if (spriteAtlas != null)
        {
            EditorUtility.SetDirty(spriteAtlas);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static void ForceRebuildSpriteAtlas()
    {
        if (tempSprites.Length <= 0)
        {
            return;
        }

        foreach (var delSprite in delSprites)
        {
            SpriteAtlas spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(delSprite.Key);
            if (spriteAtlas != null)
            {
                if (delSprite.Value != null)
                {
                    SpriteAtlasExtensions.Remove(spriteAtlas, new[] { delSprite.Value });
                }
            }
            SpriteAtlasUtility.PackAtlases(new[] { spriteAtlas }, EditorUserBuildSettings.activeBuildTarget);

            SerializedObject so = new SerializedObject(spriteAtlas);
            SerializedProperty sp = so.FindProperty("m_PackedSprites");
            if (sp.arraySize <= 0 && File.Exists(delSprite.Key))
            {
                AssetDatabase.DeleteAsset(delSprite.Key);
            }
            if (spriteAtlas != null)
            {
                EditorUtility.SetDirty(spriteAtlas);
            }
            Debug.Log("Force rebuild sprite atlas success: " + delSprite.Key);
        }

        delSprites.Clear();
        tempSprites = null;

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}