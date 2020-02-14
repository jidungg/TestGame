using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour
{
    BuildManager buildManager;

    //버튼에 연결된카드정보
    public Card card;
    //버튼에 부여된 인덱스
    public int index;

    private void Awake()
    {
        buildManager = GameObject.Find("BuildManager").GetComponentInParent<BuildManager>();
    }

    private void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(OnClick);
    }


    void OnClick()
    {
        buildManager.BuildingButtonClick(card);

    }
}
