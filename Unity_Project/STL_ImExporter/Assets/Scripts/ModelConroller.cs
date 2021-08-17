using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelConroller : MonoBehaviour
{
    private Vector3 MoveUP = new Vector3(0, 0, 1);
    private Vector3 MoveDOWN = new Vector3(0, 0, -1);
    private Vector3 MoveLEFT = new Vector3(-1, 0, 0);
    private Vector3 MoveRIGHT = new Vector3(1, 0, 0);

    //滑动旋转变量
    public float sumRotationX = 0;
    public float sumRotationY = 0;
    private Vector3 oldPosition;
    private Vector3 newPosition;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //键盘平移控制
        if (Input.GetKeyDown(KeyCode.W))
        {
            this.transform.position += MoveUP;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            this.transform.position += MoveDOWN;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            this.transform.position += MoveLEFT;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            this.transform.position += MoveRIGHT;
        }

        //滑动旋转
        if (Input.GetMouseButtonDown(0))
        {
            newPosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            oldPosition = newPosition;
            newPosition = Input.mousePosition;

            sumRotationX += (newPosition.x - oldPosition.x);
            sumRotationY += (newPosition.y - oldPosition.y);

            this.transform.Rotate(GetCenterVector3(), newPosition.x - oldPosition.x);
            //this.transform.Rotate(GetCenterVector3(), newPosition.y - oldPosition.y);
        }

        //滚轮缩放
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (Camera.main.fieldOfView <= 100)
                Camera.main.fieldOfView += 2;
            if (Camera.main.orthographicSize <= 20)
                Camera.main.orthographicSize += 0.5F;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (Camera.main.fieldOfView > 2)
                Camera.main.fieldOfView -= 2;
            if (Camera.main.orthographicSize >= 1)
                Camera.main.orthographicSize -= 0.5F;
        }
    }

    public Vector3 GetCenterVector3()
    {
        return this.GetComponent<Renderer>().bounds.center;
    }
}
