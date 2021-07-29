using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SnakeController : MonoBehaviour
{
    //对应实体
    public GameObject BodyProfab;
    public GameObject SankeHead;
    public GameObject Canvas;

    //蛇长度
    private int SnakeLength;

    //蛇移动
    private Vector3 MoveUP = new Vector3(0,0,1);
    private Vector3 MoveDOWN = new Vector3(0, 0, -1);
    private Vector3 MoveLEFT = new Vector3(-1, 0, 0);
    private Vector3 MoveRIGHT = new Vector3(1, 0, 0);

    //记录当前位置
    private Vector3 SnakeDirection;

    //时间间隔
    private float Timer;
    public float Threshold;

    // Start is called before the first frame update
    void Start()
    {
        //初始化蛇
        SnakeLength = 3;
        SnakeDirection = MoveRIGHT;
        for(int i = 0; i < SnakeLength; ++i)
        {
            GameObject SankeBody = Instantiate(BodyProfab,transform);
            SankeBody.transform.position = new Vector3(
                SankeHead.transform.position.x - (i+1), 
                SankeHead.transform.position.y, 
                SankeHead.transform.position.z
                );
        }

        //初始化时间
        Timer = 0;

    }

    // Update is called once per frame
    void Update()
    {
        Canvas.transform.FindChild("Text_Score").GetComponent<Text>().text = "Score: " + (SnakeLength - 3);

        if ( Canvas.transform.FindChild("Button").gameObject.active == false )
        {
            //键盘事件判断
            if (Input.GetKeyDown(KeyCode.UpArrow) && SnakeDirection != MoveDOWN)
            {
                SnakeDirection = MoveUP;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && SnakeDirection != MoveUP)
            {
                SnakeDirection = MoveDOWN;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) && SnakeDirection != MoveRIGHT)
            {
                SnakeDirection = MoveLEFT;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) && SnakeDirection != MoveLEFT)
            {
                SnakeDirection = MoveRIGHT;
            }


            //蛇运动
            if (Timer > Threshold)
            {
                for (int i = SnakeLength - 1; i > 0; --i)
                {
                    transform.GetChild(i).transform.position = transform.GetChild(i - 1).transform.position;
                }
                transform.GetChild(0).transform.position = SankeHead.transform.position;

                SankeHead.transform.position += SnakeDirection;
                Timer = 0;
            }

            Timer += Time.deltaTime; //上一帧完成时间
        }
    }

    //吃苹果方法
    public void GetApple()
    {
        //创建身体
        GameObject SankeBody = Instantiate(BodyProfab, transform);
        SankeBody.transform.position = new Vector3(
                transform.GetChild(SnakeLength - 1).transform.position.x,
                transform.GetChild(SnakeLength - 1).transform.position.y,
                transform.GetChild(SnakeLength - 1).transform.position.z
                );

        SnakeLength++;

        //加速处理
        if (Threshold > 0.1f)
        {
            Threshold -= 0.05f;
        }
    }
}
