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

                Vector3 offset = gameObject.transform.position;

                for (int j = 0; j < triangles.Length; j++)
                {
                    //获取Vertice信息
                    Vector3 Vertice = vertices[triangles[j]];

                    //向量偏移
                    Vertice = Vertice + offset;

                    //向量旋转
                    Vector3 Axis = gameObject.GetComponent<ModelConroller>().GetCenterVector3();    //旋转轴获取
                    float AngleX = gameObject.GetComponent<ModelConroller>().sumRotationX;  //X轴旋转角度
                    float AngleY = gameObject.GetComponent<ModelConroller>().sumRotationY;  //Y轴旋转角度

                    Vertice = RotateRound(Vertice, Axis, AngleX); //X轴旋转


                    //添加坐标信息
                    lines.Add(Vertice);
                }

                DebugDrawVector3(gameObject.GetComponent<ModelConroller>().GetCenterVector3());
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

