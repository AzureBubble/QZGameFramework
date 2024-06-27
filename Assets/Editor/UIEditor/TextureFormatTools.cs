using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TextureFormatTools
{
    public static void FormatTexture(TextureImporter ti, string path)
    {
        if (ti == null) return;

        if (path.StartsWith("Assets/ArtRes") && path.EndsWith(".png"))
        {
            if (ti.textureType != TextureImporterType.Sprite)
            {
                ti.textureType = TextureImporterType.Sprite;
                ti.mipmapEnabled = false;
            }
        }
    }
}