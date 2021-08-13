using System.Collections.Generic;
using UnityEngine;

public class DrawWireFrame : MonoBehaviour
{
    public GameObject GoParent; //该GameObject下存放所有导入生成的模型
    public Material lineMat;    //线框画笔颜色
    public Color lineColor = Color.black;   //设置线框颜色

    private List<Vector3> lines = new List<Vector3>();    //顶点数组
    private bool isWireFrame = false;   //是否为线框显示

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

        if (isWireFrame)    //线框模式
        {
            //提取坐标信息
            for (int i = 0; i < GoParent.transform.childCount; ++i)
            {
                GameObject gameObject = GoParent.transform.GetChild(i).gameObject;
                //线框显示模式 设置Shader为"Skybox/Cubemap"透明显示
                gameObject.GetComponent<MeshRenderer>().material.shader = Shader.Find("Skybox/Cubemap");

                var vertices = gameObject.GetComponent<MeshFilter>().mesh.vertices;
                var triangles = gameObject.GetComponent<MeshFilter>().mesh.triangles;

                Vector3 transform = gameObject.transform.position;

                for (int j = 0; j < triangles.Length / 3; j++)
                {
                    //从三角形的数据点添加点数据
                    Vector3 v1 = vertices[triangles[j * 3]];
                    Vector3 v2 = vertices[triangles[j * 3 + 1]];
                    Vector3 v3 = vertices[triangles[j * 3 + 2]];

                    Vector3 Axis = gameObject.GetComponent<ModelConroller>().GetCenterVector3();
                    //添加旋转X
                    float AngleX = gameObject.GetComponent<ModelConroller>().sumRotationX;
                    v1 = RotateRound(v1, Axis, AngleX);
                    v2 = RotateRound(v2, Axis, AngleX);
                    v3 = RotateRound(v3, Axis, AngleX);

                    //添加旋转Y
                    float AngleY = gameObject.GetComponent<ModelConroller>().sumRotationY;
                    v1 = RotateRound(v1, Axis, AngleY);
                    v2 = RotateRound(v2, Axis, AngleY);
                    v3 = RotateRound(v3, Axis, AngleY);

                    //添加平移偏移量
                    v1 += transform;
                    v2 += transform;
                    v3 += transform;

                    lines.Add(v1);
                    lines.Add(v2);
                    lines.Add(v3);
                }
            }

            //画线框
            lineMat.SetColor("_LineColor", lineColor);  //设置 GL 线的颜色
            lineMat.SetPass(0);

            GL.PushMatrix();
            //GL.MultMatrix(transform.localToWorldMatrix);  //转换到世界坐标-用了会失效
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

            //DebugDraw(); //Debug画线 用于测试顶点数组
        }
        else    //正常模式
        {
            for (int i = 0; i < GoParent.transform.childCount; ++i)
            {
                GameObject gameObject = GoParent.transform.GetChild(i).gameObject;
                //恢复默认Shader
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

