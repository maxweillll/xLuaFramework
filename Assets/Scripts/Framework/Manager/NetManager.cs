using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetManager : MonoBehaviour
{
    NetClient m_NetClient;
    Queue<KeyValuePair<int, string>> m_MessageQuene = new Queue<KeyValuePair<int, string>>();
    XLua.LuaFunction ReceiveMessage;
    public void Init()
    {
        m_NetClient = new NetClient();
        ReceiveMessage = Manager.Lua.LuaEnv.Global.Get<XLua.LuaFunction>("ReceiveMessage");
    }

    //发送消息
    public void SendMessage(int messageId, string message)
    {
        m_NetClient.SendMessage(messageId, message);
    }

    //连接到服务器
    public void ConnectedServer(string post,int port)
    {
        m_NetClient.OnConnectServer(post,port);
    }

    //网络连接
    public void OnNetConnected()
    {

    }

    //被服务器断开连接
    public void OnDisConnected()
    {

    }

    //接受到数据
    public void Receive(int msgId,string message)
    {
        m_MessageQuene.Enqueue(new KeyValuePair<int, string>(msgId, message));
    }

    private void Update()
    {
        if(m_MessageQuene.Count > 0)
        {
            KeyValuePair<int, string> msg = m_MessageQuene.Dequeue();
            ReceiveMessage?.Call(msg.Key, msg.Value);
        }
    }
}
