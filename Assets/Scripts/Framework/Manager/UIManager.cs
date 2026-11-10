using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    //缓存UI
    Dictionary<string, GameObject> m_UI = new Dictionary<string, GameObject>();
    //ui分组
    Dictionary<string,Transform> m_UIGroups = new Dictionary<string,Transform>();

    private Transform m_UIparent;

    private void Awake()
    {
        m_UIparent = this.transform.parent.Find("UI");
    }
    public void SetUIGroup(List<string> group)
    {
        for (int i = 0; i < group.Count; i++)
        {
            //重复调用（例如 Main 被 require 多次）时复用已有分组，避免 Add 抛重复键
            if (m_UIGroups.ContainsKey(group[i]))
            {
                Debug.LogWarning("ui group already exist: " + group[i]);
                continue;
            }
            GameObject go = new GameObject("Group-" + group[i]);
            go.transform.SetParent(m_UIparent, false);
            m_UIGroups.Add(group[i], go.transform);
        }
    }
    Transform GetUIGroup(string group)
    {
        Transform trans;
        if (!m_UIGroups.TryGetValue(group, out trans))
        {
            Debug.LogError("group is not exist: " + group);
            return null;
        }
        return trans;
    }
    public void OpenUI(string uiName, string group,string luaName)
    {
        GameObject ui = null;
        if(m_UI.TryGetValue(uiName, out ui))
        {
            UILogic uiLogic = ui.GetComponent<UILogic>();
            uiLogic.OnOpen();
            return;
        }

        Manager.Resource.LoadUI(uiName, (UnityEngine.Object obj) =>
        {
            //异步加载期间同一个 UI 可能被再次打开，已缓存则复用，避免重复 Add 抛键冲突
            GameObject cached;
            if (m_UI.TryGetValue(uiName, out cached))
            {
                cached.GetComponent<UILogic>().OnOpen();
                return;
            }

            ui = Instantiate(obj) as GameObject;
            m_UI[uiName] = ui;

            Transform parent = GetUIGroup(group);   //这里要用分组名，不是 uiName
            if (parent == null) parent = m_UIparent; //分组名传错时兜底，避免 UI 被挂到场景根节点
            ui.transform.SetParent(parent, false);

            UILogic uiLogic = ui.AddComponent<UILogic>();
            uiLogic.Init(luaName);
            uiLogic.OnOpen();
        });
    }
}
