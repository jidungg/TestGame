using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieControl : Unit
{
    // Start is called before the first frame update
    void Start()
    {
        //targetPoint = GameObject.FindWithTag("TargetPointer").GetComponent<Transform>(); // targetPoint는 무조건 null이 아니라고 가정
    }

    // Update is called once per frame
    void Update()
    {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Reset();
        sw.Start();
        Debug.Log("cost time:" + sw.ElapsedTicks);
    }
}
