using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Camera nowCamera;
    public float camPanningSpeed;
    public int camPanningDetect;
    public int maxCameraHeight;
    public int minCameraHeight;

    public UnitContorl unitControler;
    public Collider groundCol; //targetpoint 이동용

    // Start is called before the first frame update
    private void Awake()
    {
        camPanningSpeed = 1f;
        camPanningDetect = 30;
        maxCameraHeight = 50;
        minCameraHeight = 10;
        nowCamera = Camera.main;
        if (groundCol.Equals(null))
        {
            groundCol = GameObject.FindGameObjectWithTag("MainGround").GetComponent<Collider>();
        }
    }
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        cameraPanning();
        if (Input.GetMouseButtonUp(0))//target point 이동
        {
            Ray ray = nowCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitinfo;
            if (Physics.Raycast(ray, out hitinfo, int.MaxValue))
            {
                unitControler.MoveMasterTargetPoint(hitinfo.point);
            }

        }
    }

    void cameraPanning()
    {
        float moveX=nowCamera.transform.position.x;
        float moveZ=nowCamera.transform.position.z;
        float moveY = nowCamera.transform.position.y;

        float mouseXPos = Input.mousePosition.x;
        float mouseYPos = Input.mousePosition.y;

        if (mouseXPos < camPanningDetect )
        {
            moveX -= camPanningSpeed;
        }
        else if(mouseXPos> Screen.width- camPanningDetect)
        {
            moveX += camPanningSpeed;
        }

        if (mouseYPos < camPanningDetect )
        {
            moveZ -= camPanningSpeed;
        }
        else if (mouseYPos > Screen.height - camPanningDetect)
        {
            moveZ += camPanningSpeed;
            
        }

        moveY -= Input.GetAxis("Mouse ScrollWheel") * camPanningSpeed*20;
        moveY = Mathf.Clamp(moveY, minCameraHeight, maxCameraHeight);

        nowCamera.transform.position = new Vector3(moveX, moveY, moveZ);
    }


}
