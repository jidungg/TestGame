using UnityEngine;
using System;
using System.Collections;
using FreeNet;
using FreeNetUnity;
using SandWarGameServer;


public class CNetworkManager : MonoBehaviour {

    public static CNetworkManager instance;// singletone

    CFreeNetUnityService gameserver;

    public MonoBehaviour messageReceiver;

    void Awake()  
    {  
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != null)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        // ��Ʈ��ũ ����� ���� CFreeNetUnityService��ü�� �߰��մϴ�.  
        this.gameserver = gameObject.AddComponent<CFreeNetUnityService>();  
  
        // ���� ��ȭ(����, �����)�� �뺸 ���� ��������Ʈ ����.  
        this.gameserver.appcallback_on_status_changed += on_status_changed;  
  
        // ��Ŷ ���� ��������Ʈ ����.  
        this.gameserver.appcallback_on_message += on_message;  
    }  
  
    // Use this for initialization  
    void Start()  
    {  
        connect();  
    }  
  
    public void connect()  
    {  
        this.gameserver.connect("127.0.0.1", 7979);  
    }

    public bool is_connected()
    {
        return this.gameserver.is_connected();
    }
    /// <summary>  
    /// ��Ʈ��ũ ���� ����� ȣ��� �ݹ� �żҵ�.  
    /// </summary>  
    /// <param name="server_token"></param>  
    void on_status_changed(NETWORK_EVENT status)  
    {  
        switch (status)  
        {  
                // ���� ����.  
            case NETWORK_EVENT.connected:  
                {  
                    Debug.Log("on connected");
                    GameObject.Find("GameMain").GetComponent<GameMain>().on_connected();
                }  
                break;  
  
                // ���� ����.  
            case NETWORK_EVENT.disconnected:  
                Debug.Log("disconnected");  
                break;  
        }  
    }  
  
    void on_message(CPacket msg)  
    {
        this.messageReceiver.SendMessage("OnRecv", msg);
 
    }  
  
    public void send(CPacket msg)  
    {  
        this.gameserver.send(msg);  
    }  
}  
