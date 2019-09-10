using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FreeNet;
using SandWarGameServer;
using UnityEngine.SceneManagement;
using System;

public class CBattleRoom : MonoBehaviour
{
    public static CBattleRoom instance;
    public CamMoving camMove;
    public GameUIControl gameUI;
    public enum GAME_STATE
    {
        //게임방에 입장후 로딩 완료되서 서버의 응답을 기다리는 상태.
        READY = 0,

        //초기건설단계
        INITIAL_BUILD=1,

        //초기건설 끝.
        INITIAL_BUILD_END,
    }

    // 서버에서 지정해준 본인의 플레이어 인덱스.
    public byte playerMeIndex;
    // 게임 상태
    GAME_STATE gameState;
    public GAME_STATE GameState
    {
        get
        {
            return gameState;
        }
        set
        {
            gameState = value;
        }
    }

    //플레이어 리스트
    List<Player> players;
    // 게임 종료 후 메인으로 돌아갈 때 사용하기 위한 MainTitle객체의 레퍼런스.
    GameMain gameMain;
    BuildManager buildManager;
    // 네트워크 데이터 송,수신을 위한 네트워크 매니저 레퍼런스.
    CNetworkManager networkManager;
    //현재 맵
    MAPS map;
    //각 플레이어의 초기자금
    int initGold;



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

        this.gameMain = GameObject.Find("GameMain").GetComponent<GameMain>();
        this.networkManager = GameObject.Find("NetworkManager").GetComponent<CNetworkManager>();

    }
    public void LoadBattle(byte playerIndex,MAPS map,int initGold)
    {
        playerMeIndex = playerIndex;
        this.map = map;
        this.initGold = initGold;
        this.networkManager.messageReceiver = this;

        CPacket msg = CPacket.create((short)PROTOCOL.LOADING_COMPLETED);
        this.networkManager.send(msg);

        this.gameState = GAME_STATE.READY;

    }
    void GameStart(CPacket msg)
    {
        this.players = new List<Player>();
        byte playercount = msg.pop_byte();
        for (byte i = 0; i < playercount; ++i)
        {
            byte player_index = msg.pop_byte();
            GameObject obj = new GameObject(string.Format("player{0}", i));
            Player player = obj.AddComponent<Player>();
            player.initialize(player_index);
            this.players.Add(player);
        }
        GainGold(initGold);
        buildManager = GameObject.Find("BuildManager").GetComponent<BuildManager>();
        buildManager.Initialize();
        BuildStart();
        StartCoroutine(InitialBuildCountDown(10));

    }
    public void OnRecv(CPacket msg)
    {
        // 제일 먼저 프로토콜 아이디를 꺼내온다.  
        PROTOCOL protocol_id = (PROTOCOL)msg.pop_protocol_id();
        Debug.Log("on message " + protocol_id);
        // 프로토콜에 따른 분기 처리.  
        switch (protocol_id)
        {
            case PROTOCOL.GAME_START:
                GameStart(msg);
                break;

            case PROTOCOL.ROOM_REMOVED:
                BackToMain();
                break;

            case PROTOCOL.BUILD_SUCCESS:
                byte build_Playerindex = msg.pop_byte();
                BuildingType build_Type = (BuildingType)msg.pop_byte();
                short build_Index = msg.pop_int16();
                Card card = new Card(build_Type);
                buildManager.Build(build_Playerindex, card, build_Index);
                break;

            case PROTOCOL.BUILD_FAILED:
                gameUI.Announce("Build Failed");
                break;

            case PROTOCOL.BATTLE_START:
                gameState = GAME_STATE.INITIAL_BUILD_END;
                for (int i = 0; i < players.Count; i++)
                {
                    players[i].BattleStart();
                }
                break;

            case PROTOCOL.UNIT_SUMMON_SUCCESS:
                byte unit_Playerindex = msg.pop_byte();
                UnitType unit_Type = (UnitType)msg.pop_byte();
                short unit_Index = msg.pop_int16();
                players[unit_Playerindex].SummonUnit(unit_Type,unit_Index);
                break;
        }
    }


    IEnumerator InitialBuildCountDown(int seconds)
    {
        gameState = GAME_STATE.INITIAL_BUILD;
        for (int i = seconds; i > 0; i--)
        {
            gameUI.Announce("Battle starts in " + i + " seconds");
            yield return new WaitForSeconds(1);
        }
        CPacket msg = CPacket.create((short)PROTOCOL.INITIALBUILD_COMPLETED);
        this.networkManager.send(msg);

        StartCoroutine(gameUI.AnnounceCoroutine("BattleStart"));

    }

    public void BuildStart()
    {
        camMove.MoveCamToBase(playerMeIndex);
        StartCoroutine(buildManager.BuldingCoroutine());
    }
    public void BuildEnd()
    {
        StopCoroutine(buildManager.BuldingCoroutine());
    }
    void BackToMain()
    {
        this.gameMain.gameObject.SetActive(true);
        this.gameMain.Enter();
        SceneManager.LoadScene("Lobby");
        gameObject.SetActive(false);
    }
    public byte GetEnemyIndex()
    {
        if(playerMeIndex == 0)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    public bool CheckBuildAreaEmpty(byte playerIndex,short areaIndex)
    {
        if (players[playerIndex].buildings.ContainsKey(areaIndex))
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    public int GetNowGold()
    {
        return players[playerMeIndex].Gold;
    }
    public void SpendGold(int amount)
    {
        gameUI.ModifyGold(players[playerMeIndex].SpendGold(amount));
    }
    public void GainGold(int amount)
    {
        gameUI.ModifyGold(players[playerMeIndex].GainGold(amount));
    }
    public void AddBuilding(byte playerIndex, Building building,short index)
    {
        players[playerIndex].AddBuilding(building,index);
    }

 
    public void BuildRequest(Card card,short locationIndex)
    {
        CPacket msg = CPacket.create((short)PROTOCOL.BUILD_REQUEST);
        msg.push((short)card.CardType);
        msg.push(locationIndex);
        this.networkManager.send(msg);
    }

    public void UnitGenRequest(UnitType type,short summoningBuildingIndex)
    {
        Debug.Log("unitgenRequest " + type + summoningBuildingIndex);
        CPacket msg = CPacket.create((short)PROTOCOL.UNIT_SUMMON_REQEST);
        msg.push((short)type);
        msg.push(summoningBuildingIndex);
        this.networkManager.send(msg);
    }

}
