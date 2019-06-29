using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    [SerializeField]private Slider hpBar;
    [SerializeField] private Transform headUpPosition;
    private Unit unitAttribute;

    // Start is called before the first frame update
    void Start()
    {
        unitAttribute = GetComponentInChildren<Unit>();
        hpBar.maxValue = unitAttribute.maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        hpBar.value = unitAttribute.HealthPoint;
    }
}
