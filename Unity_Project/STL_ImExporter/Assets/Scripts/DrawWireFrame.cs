using System.Collections.Generic;
using UnityEngine;

public class DrawWireFrame : MonoBehaviour
{
    public GameObject GoParent; //��GameObject�´�����е������ɵ�ģ��
    public Material lineMat;    //�߿򻭱���ɫ
    public Color lineColor = Color.black;   //�����߿���ɫ

    private List<Vector3> lines = new List<Vector3>();    //��������
    private bool isWireFrame = false;   //�Ƿ�Ϊ�߿���ʾ

    public void OnClick()
    {

        if (isWireFrame)
        {
            isWireFrame = false;
        }
        else
        {
            isWireFrame = true;
        }

    }

    void OnRenderObject()
    {
        if (lines != null)
            lines.Clear();

        if (isWireFrame)    //�߿�ģʽ
        {
            //��ȡ������Ϣ
            for (int i = 0; i < GoParent.transform.childCount; ++i)
            {
                GameObject gameObject = GoParent.transform.GetChild(i).gameObject;

                //�߿���ʾģʽ ����ShaderΪ"Skybox/Cubemap"͸����ʾ
                gameObject.GetComponent<MeshRenderer>().material.shader = Shader.Find("Skybox/Cubemap");

                var vertices = gameObject.GetComponent<MeshFilter>().mesh.vertices;
                var triangles = gameObject.GetComponent<MeshFilter>().mesh.triangles;

                Vector3 offset = gameObject.transform.position;

                for (int j = 0; j < triangles.Length; j++)
                {
                    //��ȡVertice��Ϣ
                    Vector3 Vertice = vertices[triangles[j]];

                    //����ƫ��
                    Vertice = Vertice + offset;

                    //������ת
                    Vector3 Axis = gameObject.GetComponent<ModelConroller>().GetCenterVector3();    //��ת���ȡ
                    float AngleX = gameObject.GetComponent<ModelConroller>().sumRotationX;  //X����ת�Ƕ�
                    float AngleY = gameObject.GetComponent<ModelConroller>().sumRotationY;  //Y����ת�Ƕ�

                    Vertice = RotateRound(Vertice, Axis, AngleX); //X����ת


                    //���������Ϣ
                    lines.Add(Vertice);
                }

                DebugDrawVector3(gameObject.GetComponent<ModelConroller>().GetCenterVector3());
            }

            //���߿�
            lineMat.SetColor("_LineColor", lineColor);  //���� GL �ߵ���ɫ
            lineMat.SetPass(0);

            GL.PushMatrix();
            //GL.MultMatrix(transform.localToWorldMatrix);  //ת������������-���˻�ʧЧ
            GL.Begin(GL.LINES);
            for (int i = 0; i < lines.Count / 3; i++)
            {
                GL.Vertex(lines[i * 3]);
                GL.Vertex(lines[i * 3 + 1]);
                GL.Vertex(lines[i * 3 + 1]);
                GL.Vertex(lines[i * 3 + 2]);
                GL.Vertex(lines[i * 3 + 2]);
                GL.Vertex(lines[i * 3]);
            }

            GL.End();
            GL.PopMatrix();
            //DebugDraw(); //Debug���� ���ڲ��Զ�������
        }
        else    //����ģʽ
        {
            for (int i = 0; i < GoParent.transform.childCount; ++i)
            {
                GameObject gameObject = GoParent.transform.GetChild(i).gameObject;
                //�ָ�Ĭ��Shader
                gameObject.GetComponent<MeshRenderer>().material.shader = Shader.Find("Standard");
            }
        }
    }

    private void DebugDraw()
    {
        for (int i = 0; i < lines.Count; i++)
        {
            Debug.DrawLine(lines[i], lines[(i + 1) % lines.Count], Color.red, 10);
        }
    }

    private void DebugDrawVector3(Vector3 point)
    {
        Debug.DrawLine(point, point + Vector3.left, Color.black, 100);
        Debug.DrawLine(point, point + Vector3.right, Color.black, 100);
        Debug.DrawLine(point, point + Vector3.up, Color.black, 100);
        Debug.DrawLine(point, point + Vector3.down, Color.black, 100);
        Debug.DrawLine(point, point + Vector3.forward, Color.black, 100);
        Debug.DrawLine(point, point + Vector3.back, Color.black, 100);
    }

    private Vector3 RotateRound(Vector3 position, Vector3 axis, float angle)
    {
        return Quaternion.AngleAxis(angle, axis) * position;
    }
}

