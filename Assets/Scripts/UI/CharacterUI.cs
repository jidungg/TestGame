using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    [SerializeField]private Slider hpBar;
    [SerializeField] private Image fill;
    [SerializeField] private Transform headUpPosition;
    private Unit unitAttribute;

    private void Awake()
    {
        unitAttribute = GetComponentInParent<Unit>();
        hpBar.maxValue = unitAttribute.maxHp;


    }
    // Start is called before the first frame update
    void Start()
    {
        if (unitAttribute.generationBuilding.playerIndex.Equals(CBattleRoom.instance.playerMeIndex))
        {
            fill.color = Color.green;

        }
        else
        {
            fill.color = Color.red;

        }
    }

    // Update is called once per frame
    void Update()
    {
        hpBar.value = unitAttribute.HealthPoint;
    }
}
