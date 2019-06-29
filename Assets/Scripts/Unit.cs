using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] [Range(1, 10)] private float moveSpeed;
    [SerializeField] int attackPoint;
    [SerializeField] public Transform targetPoint;
    public int maxHp;
    public int HealthPoint { get; set; }

    public Transform genPoint;
    public bool isMoving;

    private void Awake()
    {
        isMoving = false;
        HealthPoint = maxHp;
    }
    void Start()
    {

        Debug.Log("UnitStart");
        StartCoroutine(Moving(targetPoint));
    }

    public void GetDamage(int dam)
    {
        HealthPoint -= dam;
        if (HealthPoint <= 0)
        {
            Die();
        }
    }

    void Attack(Unit unit)
    {
        unit.GetDamage(attackPoint);
    }
    void Die()
    {
        this.gameObject.SetActive(false);
    }
    public IEnumerator Moving(Transform targetPoint)
    {
        Debug.Log("Moving(Unit)"+ targetPoint.position);
        //GetDamage(1);
        isMoving = true;
        Vector3 distFromTarget = this.transform.position - targetPoint.position;
        while (Vector3.SqrMagnitude(distFromTarget) > 1)
        {
            distFromTarget = this.transform.position - targetPoint.position;
            transform.position = Vector3.MoveTowards(this.transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
            transform.LookAt(targetPoint);
            yield return null;
        }
        isMoving = false;
        yield break;
    }
    private void OnDisable()
    {
        Debug.Log("disabled genposition: "+ genPoint.position);
        this.transform.position = genPoint.position;
    }
}
