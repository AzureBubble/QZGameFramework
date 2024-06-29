using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

public class TextureFormatTools
{
    /// <summary>
    /// 将导入的图片资源设置为 Sprite 关闭 Mipmap
    /// </summary>
    /// <param name="ti">导入的资源</param>
    /// <param name="path">图片路径</param>
    public static void FormatSprite(TextureImporter ti, string path)
    {
        if (ti == null) return;

        if (path.StartsWith("Assets/ArtRes") && path.EndsWith(".png"))
        {
            if (ti.textureType != TextureImporterType.Sprite)
            {
                ti.textureType = TextureImporterType.Sprite;
                ti.mipmapEnabled = false;
                ti.SaveAndReimport();
            }
        }
    }
}