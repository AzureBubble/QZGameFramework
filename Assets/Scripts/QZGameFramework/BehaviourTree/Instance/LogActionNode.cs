using UnityEngine;

public class LogActionNode : BTActionNode
{
    public string message;

    public LogActionNode()
    {
        AddOnEnterEvent(OnEnter);
        AddOnUpdateEvent(OnUpdate);
        AddOnExitEvent(OnExit);
    }

    private void OnEnter()
    {
        Debug.Log(message);
    }

    private E_BT_StateType OnUpdate()
    {
        return E_BT_StateType.Success;
    }

    private void OnExit(E_BT_StateType type)
    {
    }
}