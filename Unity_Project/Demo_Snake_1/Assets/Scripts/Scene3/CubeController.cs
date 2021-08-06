using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    Vector3 world;//物体要移动到的位置 （世界坐标系）
    float moveSpeed = 0;//物体移动速度

    private Vector3 MoveUP = new Vector3(0, 0, 1);
    private Vector3 MoveDOWN = new Vector3(0, 0, -1);
    private Vector3 MoveLEFT = new Vector3(-1, 0, 0);
    private Vector3 MoveRIGHT = new Vector3(1, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //键盘
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



        //将物体的世界坐标转换为屏幕坐标
        Vector3 targetposition = Camera.main.WorldToScreenPoint(this.transform.position);
        //鼠标在屏幕上的位置坐标
        Vector3 mouseposition = Input.mousePosition;

        //坐标转换
        mouseposition.z = targetposition.z;
        world.x = Camera.main.ScreenToWorldPoint(mouseposition).x;
        world.z = Camera.main.ScreenToWorldPoint(mouseposition).z;
        world.y = this.transform.position.y;

        if (Input.GetMouseButton(0))
        {
            moveSpeed = 3;
        }
        else
        {
            moveSpeed = 0;
        }

        //如果物体移动到了鼠标指定的位置 将移动速度设为0
        if (this.transform.position == world)
        {
            moveSpeed = 0;
        }

        this.transform.LookAt(world);//物体朝向鼠标对应的位置 （此时的位置为世界坐标系）
        this.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        
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
}
