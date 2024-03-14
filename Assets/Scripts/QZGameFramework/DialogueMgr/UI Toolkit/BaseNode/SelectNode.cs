using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = ("DialogueTree/SelectNode"), fileName = ("New SelectNode"))]
public class SelectNode : BaseNode
{
    [TextArea] public string dialogueContent;
    public List<BaseNode> childs = new List<BaseNode>();
    private int nextNodeIndex = 0;

    public override void OnEnter()
    {
        Debug.Log(dialogueContent);
    }

    public override BaseNode Excute()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            nextNodeIndex = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            nextNodeIndex = 1;
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            state = E_NodeState.Waiting;
            if (nextNodeIndex < childs.Count)
            {
                childs[nextNodeIndex].state = E_NodeState.Running;
                return childs[nextNodeIndex];
            }
        }
        return this;
    }

    public override void OnExit()
    {
        Debug.Log("OnExit()");
    }
}