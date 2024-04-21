using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace QZGameFramework.AutoUIManager
{
    [CreateAssetMenu(fileName = "UIConfig", menuName = "UISetting/UIConfig")]
    public class UIConfig : ScriptableObject
    {
        // 不开启则为叠遮模式
        [Tooltip("是否开启单遮罩模式")] public bool SINGMASK_SYSTEM;

        [SerializeField] private string UI_PREFABS_PATH = "/Resources/UI";
        [SerializeField] private List<string> windowRootArr = new List<string>();
        public List<WindowData> windowDataList = new List<WindowData>();

        private string[] scenePathSplit;

        public void GeneratorWindowConfig()
        {
            scenePathSplit = new string[] { UI_PREFABS_PATH, ".prefab" };
            //检测预制体有没有新增，如果没有就不需要生成配置
            int count = 0;
            foreach (var item in windowRootArr)
            {
                string[] filePathArr = Directory.GetFiles(Application.dataPath + UI_PREFABS_PATH + item, "*.prefab", SearchOption.AllDirectories);
                foreach (var path in filePathArr)
                {
                    if (path.EndsWith(".meta"))
                    {
                        continue;
                    }
                    count += 1;
                }
            }
            if (count == windowDataList.Count)
            {
                Debug.Log("预制体个数没有发生改变，不生成窗口配置");
                return;
            }

            windowDataList.Clear();
            foreach (var item in windowRootArr)
            {
                //获取预制体文件夹读取路径
                string floder = Application.dataPath + UI_PREFABS_PATH + item;
                //获取文件夹下的所有Prefab文件
                string[] filePathArr = Directory.GetFiles(floder, "*.prefab", SearchOption.AllDirectories);
                foreach (var path in filePathArr)
                {
                    if (path.EndsWith(".meta"))
                    {
                        continue;
                    }
                    string[] strs = path.Split(scenePathSplit, StringSplitOptions.RemoveEmptyEntries);

                    //获取预制体名字
                    string fileName = Path.GetFileNameWithoutExtension(path);
                    //计算文件读取路径
                    string filePath = UI_PREFABS_PATH.Split(new string[] { "/Resources/" }, StringSplitOptions.RemoveEmptyEntries)[0] + strs[1];
                    WindowData data = new WindowData { name = fileName, path = filePath };
                    windowDataList.Add(data);
                }
            }
        }

        public string GetWindowPath(string wndName)
        {
            foreach (var item in windowDataList)
            {
                if (string.Equals(item.name, wndName))
                {
                    return item.path;
                }
            }
            Debug.LogError(wndName + "不存在配置文件中，请检查窗口预制体存放位置，或配置文件UIConfig. Path: UI/UIConfig/UIConfig.");
            return "";
        }
    }

    [System.Serializable]
    public class WindowData
    {
        public string name;
        public string path;
    }
}