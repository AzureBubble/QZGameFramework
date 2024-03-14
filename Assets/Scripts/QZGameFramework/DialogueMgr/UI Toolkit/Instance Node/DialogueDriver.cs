using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomEditorDialogue
{
    public class DialogueDriver : MonoBehaviour
    {
        public DialogueNodeTree tree;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                tree.OnTreeStart();
            }
            tree?.OnUpdate();
            if (Input.GetKeyDown(KeyCode.P))
            {
                tree.OnTreeStop();
            }
        }
    }
}