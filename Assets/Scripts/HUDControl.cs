using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDControl : MonoBehaviour
{
    public Camera camToLook;

    private void Start()
    {
        if(camToLook.Equals(null))
        {
            camToLook = Camera.main;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + camToLook.transform.rotation*Vector3.back,camToLook.transform.rotation*Vector3.down);
    }
}
