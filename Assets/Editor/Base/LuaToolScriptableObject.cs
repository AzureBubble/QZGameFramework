using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ScriptableObjectPath("ProjectSettings/LuaTool.asset")]
public class LuaToolScriptableObject : ScriptableObjectSingleton<LuaToolScriptableObject>
{
    [Header("Lua原文件夹名")]
    public string luaDirName = "Lua";

    [Header("Lua转存文件夹名")]
    public string luaNewDirName = "LuaTxt";

    [Header("Lua的AB包名")]
    public string abName = "lua";
}