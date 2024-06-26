using UnityEditor;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

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

                    TMP_Text tmp = obj.GetComponent<TextMeshProUGUI>();
                    if (tmp != null)
                    {
                        tmp.raycastTarget = false;
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
                    //if (button != null)
                    //{
                    //    Navigation navigation = button.navigation;
                    //    navigation.mode = Navigation.Mode.None;
                    //    button.navigation = navigation;
                    //}

                    Text text = obj.GetComponentInChildren<Text>();
                    if (text != null)
                    {
                        text.raycastTarget = false;
                    }

                    TMP_Text tmp = obj.GetComponentInChildren<TextMeshProUGUI>();
                    if (tmp != null)
                    {
                        tmp.raycastTarget = false;
                    }
                }
                else if (obj.name.Contains("Panel"))
                {
                    Image image = obj.GetComponent<Image>();
                    if (image != null)
                    {
                        image.raycastTarget = false;
                    }
                }
                else if (obj.name.Contains("Toggle"))
                {
                    Toggle toggle = obj.GetComponent<Toggle>();
                    if (toggle != null)
                    {
                        Navigation navigation = toggle.navigation;
                        navigation.mode = Navigation.Mode.None;
                        toggle.navigation = navigation;
                    }

                    Image[] images = obj.GetComponentsInChildren<Image>();
                    if (images != null && images.Length >= 2)
                    {
                        images[1].raycastTarget = false; ;
                    }
                }
                else if (obj.name.Contains("Dropdown"))
                {
                    TMP_Text tmp = obj.GetComponentInChildren<TextMeshProUGUI>();
                    if (tmp != null)
                    {
                        tmp.raycastTarget = false;
                    }
                }
                else if (obj.name.Contains("InputField"))
                {
                    Text[] texts = obj.GetComponentsInChildren<Text>();
                    if (texts != null && texts.Length > 0)
                    {
                        foreach (var item in texts)
                        {
                            item.raycastTarget = false;
                        }
                    }

                    TMP_Text[] tmp = obj.GetComponentsInChildren<TextMeshProUGUI>();
                    if (tmp != null && tmp.Length > 0)
                    {
                        foreach (var item in tmp)
                        {
                            item.raycastTarget = false;
                        }
                    }
                }
                else if (obj.name.Contains("Scroll View"))
                {
                    GameObject viewPort = obj.transform.Find("Viewport").gameObject;
                    if (viewPort.TryGetComponent<Mask>(out Mask mask))
                    {
                        DestroyImmediate(mask);
                        viewPort.AddComponent<RectMask2D>();
                    }
                    if (viewPort.TryGetComponent<Image>(out Image image))
                    {
                        DestroyImmediate(image);
                    }
                }
            }
        }

        private static void LoadWindowCamera()
        {
            GameObject window = Selection.activeGameObject;
            if (window != null)
            {
                if (window.name.Contains("Window") && window.TryGetComponent<Canvas>(out Canvas canvas))
                {
                    canvas.renderMode = RenderMode.ScreenSpaceCamera;
                    GameObject uiCameraObj = GameObject.Find("UIRoot/UICamera");
                    if (uiCameraObj != null)
                    {
                        canvas.worldCamera = uiCameraObj.GetComponent<Camera>();
                    }

                    if (window.TryGetComponent<CanvasScaler>(out CanvasScaler canvasScaler))
                    {
                        canvasScaler.referenceResolution = new Vector2(1920, 1080);
                    }
                }
            }
        }
    }
}