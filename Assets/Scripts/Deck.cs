using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class Deck 
{

    List<Card> cardList= new List<Card>();

    public Card this[int index]
    {
        get
        {
           return cardList[index] ;
        }

    }
    public void AddToDeck(Card card)
    {
        if (!cardList.Contains(card))
        {
            cardList.Add(card);
        }
        OrderByCost();
    }
    public void ExceptFromDeck(Card card)
    {
        if (cardList.Contains(card))
        {
            cardList.Remove(card);
        }
    }
    public int GetCount()
    {
        return cardList.Count;
    }
    private void OrderByCost()
    {
        var ordered = from card in cardList
                             orderby card.cost
                             select card;
        List<Card> list= ordered.ToList<Card>();
        for (int i=0;i< list.Count;i++)
        {
            list[i].index = i;
        }

    }
}
