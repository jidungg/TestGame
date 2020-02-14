using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FreeNet;
using SandWarGameServer;
using UnityEngine.SceneManagement;
using System;

public class CBattleRoom : MonoBehaviour
{
    public enum GAME_STATE
    {
        //게임방에 입장후 로딩 완료되서 서버의 응답을 기다리는 상태.
        READY = 0,

        //초기건설단계
        INITIAL_BUILD = 1,

        //초기건설 끝.
        INITIAL_BUILD_END,
    }

    public static CBattleRoom instance;
    public CamMoving camMove;
    public GameUIControl gameUI;

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
    //현재 소환된 유닛의 갯수
    short numUnits;
    //모든 건물들 리스트
    Dictionary<short, Building> buildings;
    //모든 유닛 리스트
    Dictionary<short, Unit> units;


    private void Awake()
    {
        Debug.Log("CBattleRoom Awake");
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
        this.buildManager = GameObject.Find("BuildManager").GetComponent<BuildManager>();

    }
    private void Reset()
    {
        foreach(var obj in buildings)
        {
        }
    }

    public void LoadBattle(byte playerIndex,MAPS map,int initGold, Deck opponentDeck)
    {
        Debug.Log("CBattleRoom LoadBattle");
        buildManager.gameObject.SetActive(true);
        playerMeIndex = playerIndex;
        this.map = map;
        this.initGold = initGold;
        this.networkManager.messageReceiver = this;
        DeckManager.instance.opponentDeck = opponentDeck;
        CPacket msg = CPacket.create((short)PROTOCOL.LOADING_COMPLETED);
        this.networkManager.send(msg);

        this.gameState = GAME_STATE.READY;


    }

    void GameStart(CPacket msg)
    {
        Debug.Log("CBattleRoom GameStart");
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
        //초기골드 획득
        GainGold(initGold);
        //턴 시작
        StartCoroutine(CommandsManager.instance.TurnsCoroutine());

    
        //건설 시작.
        BuildStart();
        //초기 건설시간 카운트.
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
        }
    }



    public void BackToMain()
    {
        Debug.Log("BackToMain");
        this.gameMain.gameObject.SetActive(true);
        this.gameMain.Enter();
        buildManager.Reset();
        SceneManager.LoadScene("Lobby",LoadSceneMode.Single);
        gameObject.SetActive(false);
    }
    public void BuildFail()
    {
        gameUI.Announce("Build Failed");
    }
    public void BattleStart()
    {
        gameState = GAME_STATE.INITIAL_BUILD_END;
        for (int i = 0; i < players.Count; i++)
        {
            players[i].BattleStart();
        }
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
        ulong turn = CommandsManager.instance.nowTurn;
        CPacket msg = CPacket.create((short)PROTOCOL.BUILD_REQUEST);
        msg.push(CommandsManager.instance.nowTurn);
        msg.push((short)card.CardType);
        msg.push(locationIndex);

        this.networkManager.send(msg);
        Debug.Log("BuildRequested in turn " + turn+ " /type: "+ (short)card.CardType);
    }


    IEnumerator InitialBuildCountDown(int seconds)
    {
        gameState = GAME_STATE.INITIAL_BUILD;
        for (int i = seconds; i > 0; i--)
        {
            gameUI.Announce("Battle starts in " + i + " seconds");
            yield return new WaitForSeconds(1);
        }

        BattleStart();
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
}
