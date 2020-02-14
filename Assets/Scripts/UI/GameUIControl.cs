using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SandWarGameServer;

public class GameUIControl : MonoBehaviour
{
    DeckManager deckManager;
    //건설 할때 필요한 건물 버튼들
    List<BuildingButton> buildingButtons= new List<BuildingButton>();
    [SerializeField]GameObject buildUI;
    [SerializeField]GameObject gameMainUI;
    [SerializeField]GameObject buildingButtonPrefap;
    [SerializeField]Text announceText;
    [SerializeField]Text goldIndicater;

    private void Awake()
    {

        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        CBattleRoom.instance.gameUI = this;
    }
    void Start()
    {
        CreateBuildingButtons(deckManager.NowDeck);
    }
    //덱의 건물 버튼들 모두 생성
    void CreateBuildingButtons(Deck deck)
    {
        for (int i = 0; i < Constants. BUILDING_PER_DECK; i++)
        {
            GameObject obj = Instantiate(buildingButtonPrefap, buildUI.transform);
            BuildingButton button = obj.GetComponent<BuildingButton>();
            button.card = deck[i];
            button.index = i;
            AddBuildingButtonToList(button);
        }
    }
    // 건물 버튼 리스트에 추가하는 함수
    public void AddBuildingButtonToList(BuildingButton button)
    {
        if (buildingButtons.Contains(button))
        {
            return;
        }
        buildingButtons.Add(button);
    }


    public void BuildDone()
    {
        buildUI.SetActive(false);
        gameMainUI.SetActive(true);
        CBattleRoom.instance.BuildEnd();
    }
    public void BuildStart()
    {
        buildUI.SetActive(true);
        gameMainUI.SetActive(false);
        CBattleRoom.instance.BuildStart();
    }
    public IEnumerator AnnounceCoroutine(string str)
    {
        announceText.text = str;
        yield return new WaitForSeconds(7);
        announceText.text = "";
    }
    public void Announce(string str)
    {
        announceText.text = str;
    }

    public void ModifyGold(int gold)
    {
        goldIndicater.text = gold.ToString()+" Gold";
    }
}
