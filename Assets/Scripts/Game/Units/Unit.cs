using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SandWarGameServer;
using System;

public abstract class Unit : MonoBehaviour
{
    public enum UnitState {IDLE,ATTACKING,MOVING,DEAD }

    public Building generationBuilding;
    public byte unitIndex;
    public UnitState unitState;
    public Transform genPoint;
    public Transform targetPoint;
    public Unit targetUnit;
    public UnitType type;
    public int level;
    public int maxHp;
    public int HealthPoint { get; set; }
    [SerializeField] [Range(1, 10)] float moveSpeed;
    [SerializeField] float attackSpeed;
    [SerializeField] int attackPoint;

    //유닛 소환후 할일
    public abstract void AfterBirth();
    //유닛 공격할때 발동
    protected abstract void OnAttack();
    //데미지 입을때 발동
    protected abstract void OnGetDamage();
    //죽을 때 발동
    protected abstract void OnDie();

    public void Initiallize(Building generationBuilding,byte unitIndex,UnitType type,Transform genPoint,Transform targetPoint)
    {
        unitState = UnitState.IDLE;
        HealthPoint = maxHp;

        this.generationBuilding = generationBuilding;
        this.unitIndex = unitIndex;
        this.type = type;
        this.genPoint = genPoint;
        this.targetPoint = targetPoint;
        this.level = generationBuilding.level;

        if (generationBuilding. playerIndex.Equals(CBattleRoom.instance.playerMeIndex))
        {
            tag = "Ally";
        }
        else
        {
            tag = "Enemy";
        }
    }



    public void StartMoving()
    {
        if (unitState.Equals(UnitState.IDLE))
        {
            StartCoroutine(MovingCoroutine());
        }
    }
    public void StopMoving()
    {
        StopCoroutine(MovingCoroutine());
        unitState = UnitState.IDLE;
        
    }

    IEnumerator MovingCoroutine()
    {
        float timer =0;
        unitState = UnitState.MOVING;
        Vector3 distFromTarget = this.transform.position - targetPoint.position;
        while (Vector3.SqrMagnitude(distFromTarget) > 1)
        {
            distFromTarget = this.transform.position - targetPoint.position;
           
            transform.position =  Vector3.MoveTowards(this.transform.position, targetPoint.position, moveSpeed * Time.deltaTime);        
            transform.LookAt(targetPoint);
            timer += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        unitState = UnitState.IDLE;
        yield break;
    }

    internal void ResetUnit()
    {
    }

    private void OnDestroy()
    {
        Debug.Log("destroy unit");
    }
}

