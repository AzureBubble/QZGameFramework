using QZGameFramework.PackageMgr.AssetBundleMgr;
using System.IO;
using UnityEngine;
using XLua;

namespace QZGameFramework.LuaMgr
{
    /// <summary>
    /// Lua 脚本管理器
    /// 提供 Lua解析器 保证唯一性
    /// </summary>
    public class LuaManager : Singleton<LuaManager>
    {
        /// <summary>
        /// Lua 解析器
        /// </summary>
        private LuaEnv luaEnv;

        /// <summary>
        /// Lua 中的大_G表
        /// </summary>
        private LuaTable global;

        public LuaTable Global
        {
            get { return global; }
        }

        /// <summary>
        /// 初始化解析器
        /// </summary>
        public void Init()
        {
            if (luaEnv != null)
            {
                return;
            }
            // 初始化Lua解析器
            luaEnv = new LuaEnv();
            // 初始化Lua中的_G表
            global = luaEnv.Global;
            // 加载Lua脚本 重定向
            // 开发环境下的测试重定向 打包时 注释
            luaEnv.AddLoader(CustomLoader);
            // 打包版本发布后的重定向
            luaEnv.AddLoader(CustomABLoader);
        }

        #region 重定向

        /// <summary>
        /// 自动执行
        /// </summary>
        /// <param name="filepath">Lua脚本名</param>
        /// <returns></returns>
        private byte[] CustomLoader(ref string filepath)
        {
            // 传入的 filepath 参数是 require 执行的 Lua 脚本的文件名
            // 通过这个文件名 拼接一个 Lua 脚本路径
            string path = Application.dataPath + "/Lua/" + filepath + ".lua";

            if (File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }
            else
            {
                Debug.Log("CustomLoader 重定向Lua脚本路径失败，Lua文件名为:" + filepath);
            }

            return null;
        }

        /// <summary>
        /// 重定向AB包中的Lua脚本资源
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        private byte[] CustomABLoader(ref string filepath)
        {
            // AB包中的Lua文件名: 文件名.lua.txt
            TextAsset lua = ABManager.Instance.LoadABRes<TextAsset>("lua", filepath + ".lua");

            if (lua != null)
            {
                return lua.bytes;
            }
            else
            {
                Debug.Log("CustomABLoader 重定向Lua脚本路径失败，Lua文件名为:" + filepath);
            }

            return null;
        }

        #endregion

        #region 执行 Lua 脚本

        /// <summary>
        /// 执行 Lua 脚本文件
        /// </summary>
        /// <param name="scriptName">脚本名</param>
        public void DoLuaScript(string scriptName)
        {
            // 进行字符串拼接 再执行 Lua 脚本
            DoString($"require('{scriptName}')");
        }

        /// <summary>
        /// 执行 Lua 脚本
        /// </summary>
        /// <param name="scriptName"></param>
        private void DoString(string scriptName)
        {
            if (luaEnv == null)
            {
                Debug.Log("Lua 解析器未初始化,无法执行Lua脚本");
                return;
            }

            luaEnv.DoString(scriptName);
        }

        #endregion

        #region 释放垃圾

        /// <summary>
        /// 释放 Lua 垃圾
        /// </summary>
        public void Tick()
        {
            if (luaEnv == null)
            {
                Debug.Log("Lua 解析器未初始化");
                return;
            }
            luaEnv.Tick();
        }

        /// <summary>
        /// 销毁解析器
        /// </summary>
        public override void Dispose()
        {
            if (IsDisposed) return;

            if (luaEnv == null)
            {
                Debug.Log("Lua 解析器未初始化");
                return;
            }
            luaEnv.Dispose();
            luaEnv = null;

            base.Dispose();
        }

        #endregion
    }
}