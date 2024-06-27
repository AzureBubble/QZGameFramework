using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
            EditorSpriteSaveInfo.OnImportSprite(asset);
        }
        // 删除资源
        foreach (var asset in deletedAssets)
        {
        }
        // 移动资源前
        foreach (var asset in movedFromAssetPaths)
        {
        }
        // 移动资源后
        foreach (var asset in movedAssets)
        {
        }
    }
}

[InitializeOnLoad]
public static class EditorSpriteSaveInfo
{
    static EditorSpriteSaveInfo()
    {
    }

    private static bool IsIgnorePath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }
        return !path.StartsWith("Assets/ArtRes");
    }

    public static void OnImportSprite(string path)
    {
        if (string.IsNullOrEmpty(path) || IsIgnorePath(path))
        {
            return;
        }
        TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
        if (ti != null)
        {
            TextureFormatTools.FormatTexture(ti, path);
        }
    }
}