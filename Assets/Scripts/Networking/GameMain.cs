using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FreeNet;
using FreeNetUnity;
using SandWarGameServer;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameMain : MonoBehaviour
{
    public static GameMain instance;
    enum USER_STATE
    {
        NOT_CONNECTED,
        CONNECTED,
        WAITING_MATCHING,
    }

    USER_STATE userState;
    public CNetworkManager network_manager;
    CBattleRoom battle_room;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        network_manager.messageReceiver = this;
        this.battle_room = GameObject.Find("BattleRoom").GetComponent<CBattleRoom>();
        this.battle_room.gameObject.SetActive(false);

    }
private void Start()
    {
        this.userState = USER_STATE.NOT_CONNECTED;
    }
    public void on_connected()
    {
        this.userState = USER_STATE.CONNECTED;

    }
    public void OnRecv(CPacket msg)
    {
        // 제일 먼저 프로토콜 아이디를 꺼내온다.  
        PROTOCOL protocol_id = (PROTOCOL)msg.pop_protocol_id();
        Debug.Log("on message " + protocol_id);
        // 프로토콜에 따른 분기 처리.  
        switch (protocol_id)
        {
            case PROTOCOL.START_LOADING:

                byte player_index = msg.pop_byte();
                MAPS map = (MAPS)msg.pop_byte();
                int initGold = msg.pop_int32();
                switch (map)
                {
                    case MAPS.DESERT:
                        SceneManager.LoadScene("Desert");
                        break;
                }
                this.battle_room.gameObject.SetActive(true);
                this.battle_room.LoadBattle(player_index,map,initGold);
                gameObject.SetActive(false);

                break;

        }
    }
    public bool MatchingRequest()
    {
        if (userState.Equals(USER_STATE.WAITING_MATCHING) || userState.Equals(USER_STATE.NOT_CONNECTED))
        {
            return false;
        }
        CPacket msg = CPacket.create((short)PROTOCOL.ENTER_GAME_ROOM_REQ);
        network_manager.send(msg);
        userState = USER_STATE.WAITING_MATCHING;
        return true;
    }


    public void Enter()
    {


        this.network_manager.messageReceiver = this;

        if (!this.network_manager.is_connected())
        {
            this.userState = USER_STATE.CONNECTED;
            this.network_manager.connect();
        }
        else
        {
            on_connected();
        }
    }


}
