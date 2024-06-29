using Newtonsoft.Json;
using QZGameFramework.PackageMgr.ResourcesManager;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace QZGameFramework.Utilities
{
    public class UISpriteAtlasUtil : Singleton<UISpriteAtlasUtil>
    {
        private const string SPRITE_ATLASES_ABPATH = "";
        private const string SPRITE_ATLASES_PATH = "SpriteAtlases/";
        private const string SPRITE_ATLASES_MAP_PATH = "Assets/Resources/SpriteConfig/SpriteConfig.json";
        private Dictionary<string, string> spriteAtlasMap = new Dictionary<string, string>();

        public override void Initialize()
        {
            base.Initialize();
            if (!File.Exists(SPRITE_ATLASES_MAP_PATH))
            {
                Debug.LogError("请检查图片映射图集配置文件: " + SPRITE_ATLASES_MAP_PATH);
            }
            string json = File.ReadAllText(SPRITE_ATLASES_MAP_PATH);
            spriteAtlasMap = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }

        public void SetSprite(Image image, string spriteName, bool setNativeSize = false)
        {
            if (string.IsNullOrEmpty(spriteName) || image == null)
            {
                return;
            }
            string atlasPath;
            string atlasName;
            if (spriteAtlasMap.ContainsKey(spriteName))
            {
                atlasName = spriteAtlasMap[spriteName];
                atlasPath = atlasName.Replace("_", "/");
                atlasPath = SPRITE_ATLASES_PATH + atlasPath;

                SpriteAtlas atlas = ResourcesMgr.Instance.LoadRes<SpriteAtlas>(atlasPath);
                if (atlas != null)
                {
                    image.sprite = atlas.GetSprite(spriteName);
                }
            }

            if (setNativeSize)
            {
                image.SetNativeSize();
            }
        }

        public void SetSprite(SpriteRenderer image, string spriteName)
        {
            if (string.IsNullOrEmpty(spriteName) || image == null)
            {
                return;
            }
            string atlasPath;
            string atlasName;
            if (spriteAtlasMap.ContainsKey(spriteName))
            {
                atlasName = spriteAtlasMap[spriteName];
                atlasPath = atlasName.Replace("_", "/");
                atlasPath = SPRITE_ATLASES_PATH + atlasPath;

                SpriteAtlas atlas = ResourcesMgr.Instance.LoadRes<SpriteAtlas>(atlasPath);
                if (atlas != null)
                {
                    image.sprite = atlas.GetSprite(spriteName);
                }
            }
        }
    }
}