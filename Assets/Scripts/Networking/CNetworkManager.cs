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

        // 네트워크 통신을 위해 CFreeNetUnityService객체를 추가합니다.  
        this.gameserver = gameObject.AddComponent<CFreeNetUnityService>();  
  
        // 상태 변화(접속, 끊김등)를 통보 받을 델리게이트 설정.  
        this.gameserver.appcallback_on_status_changed += on_status_changed;  
  
        // 패킷 수신 델리게이트 설정.  
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
    /// 네트워크 상태 변경시 호출될 콜백 매소드.  
    /// </summary>  
    /// <param name="server_token"></param>  
    void on_status_changed(NETWORK_EVENT status)  
    {  
        switch (status)  
        {  
                // 접속 성공.  
            case NETWORK_EVENT.connected:  
                {  
                    Debug.Log("on connected");
                    GameObject.Find("GameMain").GetComponent<GameMain>().on_connected();
                }  
                break;  
  
                // 연결 끊김.  
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
