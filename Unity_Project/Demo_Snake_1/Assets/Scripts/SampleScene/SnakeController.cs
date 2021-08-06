using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SnakeController : MonoBehaviour
{
    //��Ӧʵ��
    public GameObject BodyProfab;
    public GameObject SankeHead;
    public GameObject Canvas;

    //�߳���
    private int SnakeLength;

    //���ƶ�
    private Vector3 MoveUP = new Vector3(0,0,1);
    private Vector3 MoveDOWN = new Vector3(0, 0, -1);
    private Vector3 MoveLEFT = new Vector3(-1, 0, 0);
    private Vector3 MoveRIGHT = new Vector3(1, 0, 0);

    //��¼��ǰλ��
    private Vector3 SnakeDirection;

    //ʱ����
    private float Timer;
    public float Threshold;

    // Start is called before the first frame update
    void Start()
    {
        //��ʼ����
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

        //��ʼ��ʱ��
        Timer = 0;

    }

    // Update is called once per frame
    void Update()
    {
        Canvas.transform.Find("Text_Score").GetComponent<Text>().text = "Score: " + (SnakeLength - 3);

        if ( Canvas.transform.Find("Button").gameObject.active == false )
        {
            //�����¼��ж�
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


            //���˶�
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

            Timer += Time.deltaTime; //��һ֡���ʱ��
        }
    }

    //��ƻ������
    public void GetApple()
    {
        //��������
        GameObject SankeBody = Instantiate(BodyProfab, transform);
        SankeBody.transform.position = new Vector3(
                transform.GetChild(SnakeLength - 1).transform.position.x,
                transform.GetChild(SnakeLength - 1).transform.position.y,
                transform.GetChild(SnakeLength - 1).transform.position.z
                );

        SnakeLength++;

        //���ٴ���
        if (Threshold > 0.1f)
        {
            Threshold -= 0.05f;
        }
    }
}
