using UnityEngine;

namespace QZGameFramework.UIManager
{
    public class GenerateConfig
    {
        public static string BindComponentGeneratePath = Application.dataPath + "/Scripts/UI/BindComponent";
        public static string WindowGeneratePath = Application.dataPath + "/Scripts/UI/Window";
        public static string OBJDATALIST_KEY = "objDataList";

        // TODO:配置当前自动化生成窗口的作者名
        public static string AUTHOR_NAME = "";
    }
}