using QZGameFramework.GameTool;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace QZGameFramework.GameTool
{
    public class DialogueTool
    {
        private readonly string IMG_COPY_TO_PATH = "Assets/Resources/Image/";

        private static DialogueData_SO currentData; // 当前对话数据

        private ReorderableList piecesList = null;
        private Dictionary<string, ReorderableList> optionListDic = new Dictionary<string, ReorderableList>();
        private Dictionary<string, Sprite> spriteListDic = new Dictionary<string, Sprite>();

        private Vector2 scrollPos = Vector2.zero;

        private string newDataName = "You Dialogue Name";
        public bool isRefresh;

        public DialogueTool(DialogueData_SO data = null)
        {
            currentData = data;
        }

        public void SetDialogueData(DialogueData_SO data)
        {
            currentData = data;
        }

        public void OnDisable()
        {
            optionListDic.Clear();
        }

        public void OnGUI()
        {
            if (!Directory.Exists(IMG_COPY_TO_PATH))
            {
                Directory.CreateDirectory(IMG_COPY_TO_PATH);
            }

            EditorGUI.BeginChangeCheck();
            // 头部可拖拽物体输入框
            currentData = EditorGUILayout.ObjectField("DialogueData_SO", currentData, typeof(DialogueData_SO), false) as DialogueData_SO;
            if (EditorGUI.EndChangeCheck())
            {
                OnDisable();
                piecesList = null;
            }

            if (currentData != null)
            {
                EditorGUILayout.LabelField(currentData.name, EditorStyles.boldLabel);

                GUILayout.Space(10);

                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                {
                    if (piecesList == null)
                    {
                        SetUpReorderableList();
                    }
                    // 绘制 ReorderableList
                    piecesList.DoLayoutList();
                }
                GUILayout.EndScrollView();
            }
            else
            {
                newDataName = GUILayout.TextField(newDataName);
                if (GUILayout.Button("Create New Dialogue"))
                {
                    string dataPath = "Assets/Game Data/Dialogue Data/";
                    if (!Directory.Exists(dataPath))
                    {
                        Directory.CreateDirectory(dataPath);
                    }
                    // 创建一个 ScriptableObject 实例
                    DialogueData_SO newData = ScriptableObject.CreateInstance<DialogueData_SO>();
                    AssetDatabase.CreateAsset(newData, dataPath + "/" + newDataName + ".asset");
                    currentData = newData;
                }
                GUILayout.Label("NO DATA SELECTED", EditorStyles.boldLabel);
            }
        }

        /// <summary>
        /// 设置 ReorderableList 数据
        /// </summary>
        private void SetUpReorderableList()
        {
            piecesList = new ReorderableList(currentData.dialoguePieces, typeof(DialoguePiece), true, true, true, true);

            piecesList.drawHeaderCallback += OnDrawPieceHeader;
            piecesList.drawElementCallback += OnDrawPieceListElement;
            piecesList.elementHeightCallback += OnHeightChanged;
        }

        /// <summary>
        /// 修改 piecesList 每一行的高度
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private float OnHeightChanged(int index)
        {
            return GetPieceHeight(currentData.dialoguePieces[index]);
        }

        /// <summary>
        /// 获得每一行数据的高度
        /// </summary>
        /// <param name="dialoguePiece"></param>
        /// <returns></returns>
        private float GetPieceHeight(DialoguePiece dialoguePiece)
        {
            float height = EditorGUIUtility.singleLineHeight;

            bool isExpand = dialoguePiece.canExpand;

            if (isExpand)
            {
                height += EditorGUIUtility.singleLineHeight * 9;

                var options = dialoguePiece.options;

                if (options.Count > 1)
                {
                    height += EditorGUIUtility.singleLineHeight * options.Count;
                }
            }

            return height;
        }

        /// <summary>
        /// 初始化图片数据
        /// </summary>
        /// <param name="dialoguePiece"></param>
        private void Init(DialoguePiece dialoguePiece)
        {
            //if (!string.IsNullOrEmpty(dialoguePiece.imgRes) && imgSprite == null)
            //{
            //    imgSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/" + dialoguePiece.imgRes + ".png");
            //}
        }

        //private Sprite imgSprite = null;

        /// <summary>
        /// 绘制每一行元素
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="index">数据的个数</param>
        /// <param name="isActive"></param>
        /// <param name="isFocused"></param>
        private void OnDrawPieceListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            EditorUtility.SetDirty(currentData);

            GUIStyle textStyle = new GUIStyle("TextField");
            if (index < currentData.dialoguePieces.Count)
            {
                DialoguePiece currentPiece = currentData.dialoguePieces[index];
                Init(currentPiece);

                Rect tempRect = rect;
                // 高度保持默认的单行高度
                tempRect.height = EditorGUIUtility.singleLineHeight;

                currentPiece.canExpand = EditorGUI.Foldout(tempRect, currentPiece.canExpand, "Element " + index);

                if (currentPiece.canExpand)
                {
                    tempRect.width = 50f;
                    tempRect.y += tempRect.height;
                    EditorGUI.LabelField(tempRect, "Piece Id");

                    tempRect.x += tempRect.width;
                    tempRect.width = 100f;
                    currentPiece.pieceId = EditorGUI.TextField(tempRect, currentPiece.pieceId);

                    tempRect.x += tempRect.width + 10f;
                    tempRect.width = 50f;
                    EditorGUI.LabelField(tempRect, "Name");
                    tempRect.x += tempRect.width - 5f;
                    tempRect.width = 100f;
                    currentPiece.name = EditorGUI.TextField(tempRect, currentPiece.name);

                    // 任务选择输入
                    tempRect.x += tempRect.width + 10f;
                    EditorGUI.LabelField(tempRect, "Quest");
                    tempRect.x += 45f;
                    tempRect.width = 200f;
                    EditorGUI.ObjectField(tempRect, currentData, typeof(DialogueData_SO), false);

                    // 换行 x 复位
                    tempRect.y += EditorGUIUtility.singleLineHeight + 5f;
                    tempRect.x = rect.x;

                    tempRect.height = 60f;
                    tempRect.width = tempRect.height;

                    string spriteListKey = currentPiece.pieceId + currentPiece.text;
                    if (spriteListKey != string.Empty)
                    {
                        Sprite imgSprite = null;
                        if (!string.IsNullOrEmpty(currentPiece.imgRes) && imgSprite == null)
                        {
                            imgSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/" + currentPiece.imgRes + ".png");
                        }
                        EditorGUI.BeginChangeCheck();
                        imgSprite = EditorGUI.ObjectField(tempRect, imgSprite, typeof(Sprite), false) as Sprite;

                        if (EditorGUI.EndChangeCheck())
                        {
                            if (imgSprite != null)
                            {
                                string imgPath = AssetDatabase.GetAssetPath(imgSprite);
                                string copyToPath = IMG_COPY_TO_PATH + imgSprite.name + ".png";
                                if (copyToPath != imgPath)
                                {
                                    if (File.Exists(copyToPath))
                                    {
                                        AssetDatabase.DeleteAsset(copyToPath);

                                        AssetDatabase.Refresh();
                                    }

                                    AssetDatabase.CopyAsset(imgPath, copyToPath);
                                    // 获得png资源的名字 拼接路径
                                    currentPiece.imgRes = "Image/" + imgSprite.name;
                                    AssetDatabase.Refresh();
                                }
                            }
                            else
                            {
                                currentPiece.imgRes = "";
                            }
                            spriteListDic[spriteListKey] = imgSprite;
                        }

                        if (!spriteListDic.ContainsKey(spriteListKey))
                        {
                            spriteListDic[spriteListKey] = imgSprite;
                        }
                        else
                        {
                            imgSprite = spriteListDic[spriteListKey];
                        }
                    }

                    tempRect.x += tempRect.width + 5f;
                    tempRect.width = rect.width - tempRect.x;
                    textStyle.wordWrap = true;
                    currentPiece.text = EditorGUI.TextField(tempRect, currentPiece.text, textStyle) as string;

                    // 画对话选项 重置 tempRect
                    tempRect.y += tempRect.height + 5f;
                    tempRect.x = rect.x;
                    tempRect.width = rect.width;

                    string optionListKey = currentPiece.pieceId + currentPiece.text;
                    if (optionListKey != string.Empty)
                    {
                        if (!optionListDic.ContainsKey(optionListKey))
                        {
                            var optionList = new ReorderableList(currentPiece.options, typeof(DialogueOption), true, true, true, true);

                            optionList.drawHeaderCallback += OnDrawOptionHeader;

                            // 绘制 options 的每一行元素
                            optionList.drawElementCallback = (optionRect, optionIndex, optionActive, optionFocused) =>
                            {
                                OnDrawOptionElement(currentPiece, optionRect, optionIndex, optionActive, optionFocused);
                            };

                            optionListDic[optionListKey] = optionList;
                        }
                        optionListDic[optionListKey].DoList(tempRect);
                    }
                }
            }
        }

        /// <summary>
        /// 绘制 options 的头部便签显示
        /// </summary>
        /// <param name="rect"></param>
        private void OnDrawOptionHeader(Rect rect)
        {
            rect.x += 10f;
            GUI.Label(rect, "Option Text");
            rect.x += rect.width * 0.5f;
            GUI.Label(rect, "Target ID");
            rect.x += rect.width * 0.3f;
            GUI.Label(rect, "Apply");
        }

        /// <summary>
        /// 画单行 Option
        /// </summary>
        private void OnDrawOptionElement(DialoguePiece currentPiece, Rect optionRect, int optionIndex, bool optionActive, bool optionFocused)
        {
            // 取得当前 option
            DialogueOption currentOption = currentPiece.options[optionIndex];
            Rect tempRect = optionRect;

            tempRect.width = optionRect.width * 0.5f;
            currentOption.text = EditorGUI.TextField(tempRect, currentOption.text);

            tempRect.x += tempRect.width + 5f;
            tempRect.width = optionRect.width * 0.3f;
            currentOption.targetID = EditorGUI.TextField(tempRect, currentOption.targetID);

            tempRect.x += tempRect.width + 5f;
            tempRect.width = optionRect.width * 0.2f;
            currentOption.takeQuest = EditorGUI.Toggle(tempRect, currentOption.takeQuest);
        }

        /// <summary>
        /// piecesList 的头部标签栏
        /// </summary>
        /// <param name="rect">绘制的矩形范围</param>
        private void OnDrawPieceHeader(Rect rect)
        {
            GUI.Label(rect, "Dialogue Pieces");
        }
    }
}