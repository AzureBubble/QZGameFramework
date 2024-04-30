using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace QZGameFramework.UIManager
{
    public class OptimizeDrawcall : Editor
    {
        [MenuItem("GameObject/AutoOptimizeDrawcall(Shift+G) #G", false, 1)]
        private static void AutoOptimizeDrawcall()
        {
            GameObject obj = Selection.objects.First() as GameObject;
            if (obj == null || !obj.GetComponent<RectTransform>())
            {
                Debug.LogError("Need select a UIElement GameObject");
                return;
            }

            List<GameObject> childs = new List<GameObject>();
            foreach (Transform child in obj.transform)
            {
                childs.Add(child.gameObject);
            }

            List<GameObject> sortedChilds = childs.OrderBy(gameObject =>
             {
                 if (gameObject.GetComponent<RawImage>())
                 {
                     return 0;
                 }
                 else if (gameObject.GetComponent<Image>())
                 {
                     return 1;
                 }
                 else if (gameObject.GetComponent<Text>())
                 {
                     return 2;
                 }
                 else if (gameObject.GetComponent<InputField>())
                 {
                     return 3;
                 }
                 else if (gameObject.GetComponent<Button>())
                 {
                     return 4;
                 }
                 else if (gameObject.GetComponent<Toggle>())
                 {
                     return 5;
                 }
                 else if (gameObject.GetComponent<Slider>())
                 {
                     return 6;
                 }
                 else if (gameObject.GetComponent<Scrollbar>())
                 {
                     return 7;
                 }
                 else return 10;
             }).ToList();

            // 设置子物体在Unity中的顺序和列表排序一致
            for (int i = 0; i < sortedChilds.Count; i++)
            {
                sortedChilds[i].transform.SetSiblingIndex(i);
            }
        }
    }
}