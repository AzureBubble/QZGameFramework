using UnityEngine;

/// <summary>
/// 顺序结点 只存在一个对话情况
/// </summary>
//[CreateAssetMenu(menuName = ("DialogueTree/SequenceNode"), fileName = ("New SequenceNode"))]
public class SequenceNode : BaseNode
{
    [TextArea] public string dialogueContent;
    public BaseNode child;

    public override void OnEnter()
    {
        Debug.Log(dialogueContent);
    }

    public override BaseNode Excute()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            state = E_NodeState.Waiting;

            if (child != null)
            {
                child.state = E_NodeState.Running;
                return child;
            }
        }
        return this;
    }

    public override void OnExit()
    {
        Debug.Log(this.GetType().Name + "执行结束退出");
        //state = E_NodeState.Waiting;
    }
}