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

                Vector3 transform = gameObject.transform.position;

                for (int j = 0; j < triangles.Length / 3; j++)
                {
                    //�������ε����ݵ���ӵ�����
                    Vector3 v1 = vertices[triangles[j * 3]];
                    Vector3 v2 = vertices[triangles[j * 3 + 1]];
                    Vector3 v3 = vertices[triangles[j * 3 + 2]];

                    Vector3 Axis = gameObject.GetComponent<ModelConroller>().GetCenterVector3();
                    //�����תX
                    float AngleX = gameObject.GetComponent<ModelConroller>().sumRotationX;
                    v1 = RotateRound(v1, Axis, AngleX);
                    v2 = RotateRound(v2, Axis, AngleX);
                    v3 = RotateRound(v3, Axis, AngleX);

                    //�����תY
                    float AngleY = gameObject.GetComponent<ModelConroller>().sumRotationY;
                    v1 = RotateRound(v1, Axis, AngleY);
                    v2 = RotateRound(v2, Axis, AngleY);
                    v3 = RotateRound(v3, Axis, AngleY);

                    //���ƽ��ƫ����
                    v1 += transform;
                    v2 += transform;
                    v3 += transform;

                    lines.Add(v1);
                    lines.Add(v2);
                    lines.Add(v3);
                }
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

    public Vector3 RotateRound(Vector3 position, Vector3 axis, float angle)
    {
        return Quaternion.AngleAxis(angle, axis) * position;
    }
}

