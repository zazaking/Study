using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ChangeCamera : MonoBehaviour
{
    public GameObject Camera_Main;//设置成public可以使unity中出现如下图所示的
    public GameObject Text_CameraMode;//设置成public可以使unity中出现如下图所示的


    //相机模式
    private int CAMERA_MODE_LEFT = 0;   //相机位置-左视图
    private int CAMERA_MODE_MAIN = 1;   //相机位置-主视图
    private int CAMERA_MODE_UP = 2;     //相机位置-俯视图
    
    private int CAMERA_MODE;

    // Start is called before the first frame update
    void Start()
    {
        CAMERA_MODE = CAMERA_MODE_MAIN;
        Camera_Main.transform.position = new Vector3(15, 18, -8);
        Camera_Main.transform.rotation = Quaternion.Euler(50, -20, 0);
        Text_CameraMode.transform.GetComponent<Text>().text = "相机位置：主视图";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Change()
    {
        ++CAMERA_MODE;

        if (CAMERA_MODE % 3 == CAMERA_MODE_MAIN)
        {
            Camera_Main.transform.position = new Vector3(15, 18, -8);
            Camera_Main.transform.rotation = Quaternion.Euler(50, -20, 0);
            Text_CameraMode.transform.GetComponent<Text>().text = "相机位置：主视图";
        }
        else if (CAMERA_MODE % 3 == CAMERA_MODE_UP)
        {
            Camera_Main.transform.position = new Vector3(10, 20, 10);
            Camera_Main.transform.rotation = Quaternion.Euler(90, 0, 0);
            Text_CameraMode.transform.GetComponent<Text>().text = "相机位置：俯视图";
        }
        else if (CAMERA_MODE % 3 == CAMERA_MODE_LEFT)
        {
            Camera_Main.transform.position = new Vector3(10, 20, 10);
            Camera_Main.transform.rotation = Quaternion.Euler(90, 90, 0);
            Text_CameraMode.transform.GetComponent<Text>().text = "相机位置：左视图";
        }
    }
}
