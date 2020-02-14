using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMoving : MonoBehaviour
{
    public float camPanningSpeed;
    public int camPanningDetect;
    public int maxCameraVert;
    public int minCameraVert;
    public int maxCameraHoriz;
    public int minCameraHoriz;
    public int maxCameraHeight;
    public int minCameraHeight;

    public Collider groundCol; //targetpoint 이동용
    public Transform[] baseCamPos = new Transform[2];

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        CBattleRoom.instance.camMove = this;

        //if (groundCol.Equals(null))
        //{
        //    groundCol = GameObject.FindGameObjectWithTag("MainGround").GetComponent<Collider>();
        //}

    }
    // Update is called once per frame
    void Update()
    {
        cameraPanning();
    }
    void cameraPanning()
    {
        float moveX = this.transform.position.x;
        float moveZ = this.transform.position.z;
        float moveY = this.transform.position.y;

        float mouseXPos = Input.mousePosition.x;
        float mouseYPos = Input.mousePosition.y;

        if (mouseXPos < camPanningDetect)
        {
            moveZ -= camPanningSpeed;
        }
        else if (mouseXPos > Screen.width - camPanningDetect)
        {
            moveZ += camPanningSpeed;
        }

        if (mouseYPos < camPanningDetect)
        {
            moveX += camPanningSpeed;
        }
        else if (mouseYPos > Screen.height - camPanningDetect)
        {
            moveX -= camPanningSpeed;

        }

        moveY -= Input.GetAxis("Mouse ScrollWheel") * camPanningSpeed * 20;
        moveY = Mathf.Clamp(moveY, minCameraHeight, maxCameraHeight);
        moveX = Mathf.Clamp(moveX, minCameraVert, maxCameraVert);
        moveZ = Mathf.Clamp(moveZ, minCameraHoriz, maxCameraHoriz);

        this.transform.position = new Vector3(moveX, moveY, moveZ);
    }
    public void MoveCamToBase(int playerIndex)
    {
        Camera.main.transform.position = baseCamPos[playerIndex].position;
    }
}
