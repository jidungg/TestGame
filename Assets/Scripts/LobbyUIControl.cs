using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIControl : MonoBehaviour
{
    [SerializeField] GameObject mainTitle;
    [SerializeField] GameObject myDeckScreen;
    [SerializeField] Button matchingButton;
    [SerializeField] Text announceText;

    public void MyDeckButton()
    {
        mainTitle.SetActive(false);
        myDeckScreen.SetActive(true);
        Announce("");
    }
    public void DeckScreenQuitButton()
    {
        mainTitle.SetActive(true);
        myDeckScreen.SetActive(false);
        Announce("");
    }
    public void MatchingButton()
    {
        GameMain gameMain = GameObject.Find("GameMain").GetComponent<GameMain>();
        if (gameMain.MatchingRequest())
        {
            Announce("Now matching please wait");
            matchingButton.enabled = false;
        }
        else
        {
            Announce("Matching faild please check your network");
        }
    }
    public void Announce(string str)
    {
        announceText.text = str;
    }

}
