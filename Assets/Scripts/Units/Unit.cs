using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SandWarGameServer;

public abstract class Unit : MonoBehaviour
{
    public enum UnitState {IDLE,ATTACKING,MOVING,DEAD }

    public byte playerIndex;
    public byte unitIndex;
    public UnitState unitState;
    public Transform genPoint;
    public Transform targetPoint;
    public Unit targetUnit;
    public UnitType type;

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

    public void Initiallize(byte playerIndex,byte unitIndex,UnitType type,Transform genPoint,Transform targetPoint)
    {
        unitState = UnitState.IDLE;
        HealthPoint = maxHp;

        this.playerIndex = playerIndex;
        this.unitIndex = unitIndex;
        this.type = type;
        this.genPoint = genPoint;
        this.targetPoint = targetPoint;

        if (playerIndex.Equals(CBattleRoom.instance.playerMeIndex))
        {
            tag = "AllyUnit";
        }
        else
        {
            tag = "EnemyUnit";
        }

    }

    public void GetDamage(int dam,Unit opponent)
    {
        HealthPoint -= dam;
        OnGetDamage();
        if (HealthPoint <= 0)
        {
            Die(opponent);
        }
    }
    void Attack(Unit unit)
    {
        unit.GetDamage(attackPoint,this);

        OnAttack();
    }
    void Die(Unit opponent)
    {
        unitState = UnitState.DEAD;
        OnDie();
        ResetUnit();
        this.gameObject.SetActive(false);

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
        unitState = UnitState.MOVING;
        Vector3 distFromTarget = this.transform.position - targetPoint.position;
        while (Vector3.SqrMagnitude(distFromTarget) > 1)
        {
            distFromTarget = this.transform.position - targetPoint.position;
            transform.position = Vector3.MoveTowards(this.transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
            transform.LookAt(targetPoint);
            yield return null;
        }
        unitState = UnitState.IDLE;
        yield break;
    }
    IEnumerator AttackCoroutine()
    {
        Debug.Log("Attackcoroutine");
        unitState = UnitState.ATTACKING;
        while (true)
        {
            Attack(targetUnit);
            yield return new WaitForSeconds(attackSpeed);
        }
        yield break;

    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter"+other.tag);
        switch (other.tag)
        {
            case "EnemyUnit":
                targetUnit = other.GetComponent<Unit>();
                StartCoroutine(AttackCoroutine());
                break;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        
    }

    public void ResetUnit()
    {
        this.transform.position = genPoint.position;
    }
}
