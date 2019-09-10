using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SandWarGameServer;

public class DeckManager : MonoBehaviour
{

    public static DeckManager instance;
    //모든 종류의 건물 프리팹을 담은 리스트. 인덱스와 BuildingType 값이 일치.
    public List<GameObject> buildingPrefapList;

    LobbyUIControl lobbyUIControl;
    //현재 저장된 덱들
    List<Deck> deckList=new List<Deck>();
    //현재 선택되어서 게임에서 이용할 덱
    Deck nowDeck;


    public Deck NowDeck
    {
        get
        {
            return nowDeck;
        }

        set
        {
            if (value.GetCount() == Constants.BUILDING_PER_DECK) //덱은 반드시 꽉 채워져 있어야함.
            {
                nowDeck = value;
            }
            else
            {
                lobbyUIControl.Announce("You can only use completed deck");
            }
        }
    }

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
        lobbyUIControl = GameObject.Find("UI").GetComponent<LobbyUIControl>();
    }
    private void Start()
    {
        //임시로 아무 덱 하나 만듦
        Deck deck = new Deck();
        Card card = new Card(BuildingType.TOMB_STONE);
        deck.AddToDeck(card);
        NowDeck = deck;
    }

}
