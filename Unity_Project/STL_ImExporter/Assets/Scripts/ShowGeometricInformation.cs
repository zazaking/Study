using System;
using UnityEngine;

//显示集合信息类
public class ShowGeometricInformation
{
    //显示生成包围盒（球型）
    public static GameObject GetBoundingBox_Sphere(GameObject _gameObject)
    {
        Vector3[] v = _gameObject.GetComponent<MeshFilter>().mesh.vertices;

        //获取球型包围盒的中心坐标-所有向量的均值
        Vector3 temp = new Vector3(0f, 0f, 0f);
        for (int i = 0; i < v.Length; ++i)
        {
            temp += v[i];
        }
        Vector3 center = new Vector3(temp.x / v.Length, temp.y / v.Length, temp.z / v.Length);

        //计算球型包围盒的半径-距离中心点的最大距离
        double radius = 0f;
        for (int i = 0; i < v.Length; ++i)
        {
            radius = (GetDistance(v[i], center) > radius) ? GetDistance(v[i], center) : radius;
        }

        GameObject boundingBoxGameObject = CreateSphere(center, (float)radius);
        boundingBoxGameObject.name = "BoundingBox_Sphere_" + _gameObject.name.ToString();

        boundingBoxGameObject.gameObject.GetComponent<MeshRenderer>().material.shader = Shader.Find("Skybox/Cubemap");

        return boundingBoxGameObject;
    }

    //创造正方形
    public static GameObject CreateSquare(Vector3 _center, float _radius)
    {
        GameObject gameObject = new GameObject();
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        //正方形-8个顶点
        gameObject.GetComponent<MeshFilter>().mesh.vertices = new Vector3[]
        {
            new Vector3(_center.x-_radius, _center.y-_radius, _center.z-_radius),
            new Vector3(_center.x+_radius, _center.y-_radius, _center.z-_radius),
            new Vector3(_center.x+_radius, _center.y-_radius, _center.z+_radius),
            new Vector3(_center.x-_radius, _center.y-_radius, _center.z+_radius),
            new Vector3(_center.x-_radius, _center.y+_radius, _center.z-_radius),
            new Vector3(_center.x+_radius, _center.y+_radius, _center.z-_radius),
            new Vector3(_center.x+_radius, _center.y+_radius, _center.z+_radius),
            new Vector3(_center.x-_radius, _center.y+_radius, _center.z+_radius),
        };

        //正方形6个面-每个面2个三角形-顺时针设置
        gameObject.GetComponent<MeshFilter>().mesh.triangles = new int[]
        {
            0,4,5,
            0,5,1,
            1,5,6,
            1,6,2,
            2,6,7,
            2,7,3,
            3,7,4,
            3,4,0,
            4,7,6,
            4,6,5,
            3,0,1,
            3,1,2
        };
        return gameObject;
    }

    //创造八面体
    public static GameObject CreateOctahedron(Vector3 _center, float _radius)
    {
        GameObject gameObject = new GameObject();
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        //八面体-6个顶点
        gameObject.GetComponent<MeshFilter>().mesh.vertices = new Vector3[] {
            Vector3.down,
            Vector3.forward,
            Vector3.left,
            Vector3.back,
            Vector3.right,
            Vector3.up
        };

        //正方形-8个顶点
        gameObject.GetComponent<MeshFilter>().mesh.vertices = new Vector3[]
        {
            _center + Vector3.down * _radius / (float)Math.Sin(45f),
            _center + Vector3.forward * _radius / (float)Math.Sin(45f),
            _center + Vector3.left * _radius / (float)Math.Sin(45f),
            _center + Vector3.back * _radius / (float)Math.Sin(45f),
            _center + Vector3.right * _radius / (float)Math.Sin(45f),
            _center + Vector3.up * _radius / (float)Math.Sin(45f)
        };

        //设置三角形顶点顺序，顺时针设置
        gameObject.GetComponent<MeshFilter>().mesh.triangles = new int[]
        {
            0, 2, 1,
            0, 3, 2,
            0, 4, 3,
            0, 1, 4,


            5, 1, 2,
            5, 2, 3,
            5, 3, 4,
            5, 4, 1

        };

        return gameObject;
    }

    //创造圆形
    public static GameObject CreateSphere(Vector3 _center, float _radius)
    {
        GameObject gameObject = new GameObject();
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        Mesh ball = new Mesh();

        //顶点数组
        Vector3[] ballVertices = new Vector3[182];
        //三角形数组
        int[] ballTriangles = new int[1080];
        /*水平每18度、垂直每18度确定一个顶点，
        顶部和底部各一个顶点，一共是9x20+2=182个顶点。
        每一环与相邻的下一环为一组，之间画出40个三角形，一共8组。
        顶部和底部各与相邻环画20个三角形，总三角形数量40x8+20x2=360,
        三角形索引数量360x3=1080*/

        int verticeCount = 0;
        for (int vD = 18; vD < 180; vD += 18)
        {
            float circleHeight =
            _radius * Mathf.Cos(vD * Mathf.Deg2Rad);
            float circleRadius =
            _radius * Mathf.Sin(vD * Mathf.Deg2Rad);
            for (int hD = 0; hD < 360; hD += 18)
            {
                ballVertices[verticeCount] =
                new Vector3(
                circleRadius * Mathf.Cos(hD * Mathf.Deg2Rad),
                circleHeight,
                circleRadius * Mathf.Sin(hD * Mathf.Deg2Rad));
                verticeCount++;
            }
        }
        ballVertices[180] = new Vector3(0, _radius, 0);
        ballVertices[181] = new Vector3(0, -_radius, 0);

        for (int i = 0; i < ballVertices.Length; ++i)
        {
            ballVertices[i] = ballVertices[i] + _center;
        }

        ball.vertices = ballVertices;

        int triangleCount = 0;
        for (int j = 0; j < 8; j++)
        {
            for (int i = 0; i < 20; i++)
            {
                ballTriangles[triangleCount++] =
                j * 20 + i;
                ballTriangles[triangleCount++] =
                (j + 1) * 20 + (i == 19 ? 0 : i + 1);
                ballTriangles[triangleCount++] =
                (j + 1) * 20 + i;
                ballTriangles[triangleCount++] =
                j * 20 + i;
                ballTriangles[triangleCount++] =
                j * 20 + (i == 19 ? 0 : i + 1);
                ballTriangles[triangleCount++] =
                (j + 1) * 20 + (i == 19 ? 0 : i + 1);
            }
        }
        for (int i = 0; i < 20; i++)
        {
            ballTriangles[triangleCount++] =
            180;
            ballTriangles[triangleCount++] =
            (i == 19 ? 0 : i + 1);
            ballTriangles[triangleCount++] =
            i;
            ballTriangles[triangleCount++] =
            181;
            ballTriangles[triangleCount++] =
            160 + i;
            ballTriangles[triangleCount++] =
            160 + (i == 19 ? 0 : i + 1);
        }
        ball.triangles = ballTriangles;
        ball.RecalculateNormals();

        gameObject.GetComponent<MeshFilter>().mesh = ball;

        return gameObject;
    }

    //计算距离
    public static double GetDistance(Vector3 _begin, Vector3 _end)
    {
        return Math.Sqrt(Math.Pow(_begin.x - _end.x, 2) + Math.Pow(_begin.y - _end.y, 2) + Math.Pow(_begin.z - _end.z, 2));
    }

    //计算体积
    public static float GetVolume(GameObject _gameObject)
    {
        Vector3[] vertices = _gameObject.GetComponent<MeshFilter>().mesh.vertices;
        int[] triangles = _gameObject.GetComponent<MeshFilter>().mesh.triangles;
        Vector3 lossyScale = _gameObject.transform.lossyScale; 
        Vector3 center = GetCenter(vertices);

        float volume = 0f;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            volume += CalculateVolumeOfTriangle(vertices[triangles[i]], vertices[triangles[i + 1]], vertices[triangles[i + 2]], center, lossyScale);
        }

        return Mathf.Abs(volume);
    }

    //计算表面积
    public static float GetArea(GameObject _gameObject)
    {
        Vector3[] vertices = _gameObject.GetComponent<MeshFilter>().mesh.vertices;
        int[] triangles = _gameObject.GetComponent<MeshFilter>().mesh.triangles;
        Vector3 lossyScale = _gameObject.transform.lossyScale;
        Vector3 center = GetCenter(vertices);

        float area = 0;
        for (int j = 0; j < triangles.Length; j += 3)
        {
            area += CalculateTriangleArea(vertices[triangles[j]], vertices[triangles[j + 1]], vertices[triangles[j + 2]], lossyScale);
        }

        return area;
    }

    //计算三角形面积
    private static float CalculateTriangleArea(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 lossyScale)
    {
        //计算缩放
        point1 = new Vector3(point1.x * lossyScale.x, point1.y * lossyScale.y, point1.z * lossyScale.z);
        point2 = new Vector3(point2.x * lossyScale.x, point2.y * lossyScale.y, point2.z * lossyScale.z);
        point3 = new Vector3(point3.x * lossyScale.x, point3.y * lossyScale.y, point3.z * lossyScale.z);

        //计算边长
        float l1 = (point2 - point1).magnitude;
        float l2 = (point3 - point2).magnitude;
        float l3 = (point1 - point3).magnitude;
        float p = (l1 + l2 + l3) * 0.5f;

        //计算面积  S=√[p(p-l1)(p-l2)(p-l3)]（p为半周长）
        return Mathf.Sqrt(p * (p - l1) * (p - l2) * (p - l3));
    }

    //计算三棱锥体积
    private static float CalculateVolumeOfTriangle(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 center, Vector3 lossyScale)
    {
        //计算缩放
        point1 = new Vector3(point1.x * lossyScale.x, point1.y * lossyScale.y, point1.z * lossyScale.z);
        point2 = new Vector3(point2.x * lossyScale.x, point2.y * lossyScale.y, point2.z * lossyScale.z);
        point3 = new Vector3(point3.x * lossyScale.x, point3.y * lossyScale.y, point3.z * lossyScale.z);

        //向量
        Vector3 v1 = point1 - center;
        Vector3 v2 = point2 - center;
        Vector3 v3 = point3 - center;

        //计算体积
        //首先我们求以这三个向量为邻棱的平行六面体的面积
        //那就是（a×b）·c的绝对值
        //然后四面体的体积是平行六面体的六分之一
        //因为四面体的底是平行六面体的一半,而且要多乘一个三分之一
        float v = Vector3.Dot(Vector3.Cross(v1, v2), v3) / 6f;
        return v;
    }

    private static Vector3 GetCenter(Vector3[] points)
    {
        Vector3 center = Vector3.zero;
        for (int i = 0; i < points.Length; i++)
        {
            center += points[i];
        }
        center = center / points.Length;
        return center;
    }
}
