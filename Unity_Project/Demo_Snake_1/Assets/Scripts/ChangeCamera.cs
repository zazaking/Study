using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeCamera : MonoBehaviour
{
    public GameObject Camera_Main;//设置成public可以使unity中出现如下图所示的
    private int mode;

    // Start is called before the first frame update
    void Start()
    {
        mode = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Change()
    {
        ++mode;

        if( mode % 2 == 0)
        {
            Camera_Main.transform.position = new Vector3(15, 18, -8);
            Camera_Main.transform.rotation = Quaternion.Euler(50, -20, 0);
        }
        else if(mode % 2 == 1)
        {
            Camera_Main.transform.position = new Vector3(10, 20, 10);
            Camera_Main.transform.rotation = Quaternion.Euler(90, 0, 0);
        }

    }
}
