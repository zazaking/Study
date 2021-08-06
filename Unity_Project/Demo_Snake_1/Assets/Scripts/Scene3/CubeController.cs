using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    Vector3 world;//����Ҫ�ƶ�����λ�� ����������ϵ��
    float moveSpeed = 0;//�����ƶ��ٶ�

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
        //����
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



        //���������������ת��Ϊ��Ļ����
        Vector3 targetposition = Camera.main.WorldToScreenPoint(this.transform.position);
        //�������Ļ�ϵ�λ������
        Vector3 mouseposition = Input.mousePosition;

        //����ת��
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

        //��������ƶ��������ָ����λ�� ���ƶ��ٶ���Ϊ0
        if (this.transform.position == world)
        {
            moveSpeed = 0;
        }

        this.transform.LookAt(world);//���峯������Ӧ��λ�� ����ʱ��λ��Ϊ��������ϵ��
        this.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        
        //��������
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
