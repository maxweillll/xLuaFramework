using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class LuaBehaviour : MonoBehaviour
{
    private LuaEnv m_LuaEnv;
    protected LuaTable m_ScriptEnv;
    private Action m_LuaInit;
    private Action m_LuaUpdate;
    private Action m_LuaOnDestroy;

    public string luaName;
    private void Awake()
    {
        //原来写在字段初始化里（Manager.Lua.LuaEnv），构造期就会取，Manager.Lua 还没就绪时抛空引用；
        //挪到这里并判空，LuaManager 尚未 Init 或已 Dispose 的情况都能安全跳过
        if (m_LuaEnv == null && Manager.Lua != null)
            m_LuaEnv = Manager.Lua.LuaEnv;

        if (m_LuaEnv == null)
        {
            Debug.LogError("LuaEnv is null, LuaBehaviour skipped: " + name);
            return;
        }

        m_ScriptEnv = m_LuaEnv.NewTable();
        //为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量，函数冲突
        LuaTable meta = m_LuaEnv.NewTable();
        meta.Set("__index", m_LuaEnv.Global);
        m_ScriptEnv.SetMetaTable(meta);
        meta.Dispose();

        m_ScriptEnv.Set("self", this);
    }

    public virtual void Init(string luaName)
    {
        if (m_LuaEnv == null && Manager.Lua != null)
            m_LuaEnv = Manager.Lua.LuaEnv;

        if (m_LuaEnv == null || m_ScriptEnv == null)
        {
            Debug.LogError("LuaBehaviour can not init, LuaEnv or ScriptEnv is null: " + luaName);
            return;
        }

        m_LuaEnv.DoString(Manager.Lua.GetLuaScript(luaName), luaName, m_ScriptEnv);
        m_ScriptEnv.Get("Update", out m_LuaUpdate);
        m_ScriptEnv.Get("OnInit", out m_LuaInit);
        m_LuaInit?.Invoke();
    }
    private void Update()
    {
        m_LuaUpdate?.Invoke();
    }
    protected virtual void Clear()
    {
        m_LuaOnDestroy = null;
        m_ScriptEnv?.Dispose();
        m_ScriptEnv = null;
        m_LuaInit = null;
        m_LuaUpdate = null;
    }
    private void OnDestroy()
    {
        m_LuaOnDestroy?.Invoke();
        Clear();
    }
    private void OnApplicationQuit()
    {
        Clear();
    }
}
