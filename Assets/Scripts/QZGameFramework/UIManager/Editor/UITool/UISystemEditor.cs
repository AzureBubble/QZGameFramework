using UnityEditor;
using UnityEngine.UI;
using UnityEngine;

namespace QZGameFramework.UIManager
{
    public class UISystemEditor : Editor
    {
        [InitializeOnLoadMethod]
        private static void InitEditor()
        {
            //监听hierarchy发生改变的委托
            EditorApplication.hierarchyChanged += HanderTextOrImageRaycast;
            EditorApplication.hierarchyChanged += LoadWindowCamera;
        }

        private static void HanderTextOrImageRaycast()
        {
            GameObject obj = Selection.activeGameObject;
            if (obj != null)
            {
                if (obj.name.Contains("Text"))
                {
                    Text text = obj.GetComponent<Text>();
                    if (text != null)
                    {
                        text.raycastTarget = false;
                    }
                }
                else if (obj.name.Contains("Image"))
                {
                    Image image = obj.GetComponent<Image>();
                    if (image != null)
                    {
                        image.raycastTarget = false;
                    }
                    else
                    {
                        RawImage rawImage = obj.GetComponent<RawImage>();
                        if (rawImage != null)
                        {
                            rawImage.raycastTarget = false;
                        }
                    }
                }
                else if (obj.name.Contains("Button"))
                {
                    Button button = obj.GetComponent<Button>();
                    if (button != null)
                    {
                        Navigation navigation = button.navigation;
                        navigation.mode = Navigation.Mode.None;
                        button.navigation = navigation;
                    }
                }
            }
        }

        private static void LoadWindowCamera()
        {
            if (Selection.activeGameObject != null)
            {
                GameObject uiCameraObj = GameObject.Find("UIRoot/UICamera");
                if (uiCameraObj != null)
                {
                    Camera camera = uiCameraObj.GetComponent<Camera>();
                    if (Selection.activeGameObject.name.Contains("Window"))
                    {
                        Canvas canvas = Selection.activeGameObject.GetComponent<Canvas>();
                        if (canvas != null)
                        {
                            canvas.worldCamera = camera;
                        }
                    }
                }
            }
        }
    }
}