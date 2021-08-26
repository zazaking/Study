using System;
using UnityEngine;

//��ʾ������Ϣ��
public class ShowGeometricInformation
{
    //��ʾ���ɰ�Χ�У����ͣ�
    public static GameObject GetBoundingBox_Sphere(GameObject _gameObject)
    {
        Vector3[] v = _gameObject.GetComponent<MeshFilter>().mesh.vertices;

        //��ȡ���Ͱ�Χ�е���������-���������ľ�ֵ
        Vector3 temp = new Vector3(0f, 0f, 0f);
        for (int i = 0; i < v.Length; ++i)
        {
            temp += v[i];
        }
        Vector3 center = new Vector3(temp.x / v.Length, temp.y / v.Length, temp.z / v.Length);

        //�������Ͱ�Χ�еİ뾶-�������ĵ��������
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

    //����������
    public static GameObject CreateSquare(Vector3 _center, float _radius)
    {
        GameObject gameObject = new GameObject();
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        //������-8������
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

        //������6����-ÿ����2��������-˳ʱ������
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

    //���������
    public static GameObject CreateOctahedron(Vector3 _center, float _radius)
    {
        GameObject gameObject = new GameObject();
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        //������-6������
        gameObject.GetComponent<MeshFilter>().mesh.vertices = new Vector3[] {
            Vector3.down,
            Vector3.forward,
            Vector3.left,
            Vector3.back,
            Vector3.right,
            Vector3.up
        };

        //������-8������
        gameObject.GetComponent<MeshFilter>().mesh.vertices = new Vector3[]
        {
            _center + Vector3.down * _radius / (float)Math.Sin(45f),
            _center + Vector3.forward * _radius / (float)Math.Sin(45f),
            _center + Vector3.left * _radius / (float)Math.Sin(45f),
            _center + Vector3.back * _radius / (float)Math.Sin(45f),
            _center + Vector3.right * _radius / (float)Math.Sin(45f),
            _center + Vector3.up * _radius / (float)Math.Sin(45f)
        };

        //���������ζ���˳��˳ʱ������
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

    //����Բ��
    public static GameObject CreateSphere(Vector3 _center, float _radius)
    {
        GameObject gameObject = new GameObject();
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        Mesh ball = new Mesh();

        //��������
        Vector3[] ballVertices = new Vector3[182];
        //����������
        int[] ballTriangles = new int[1080];
        /*ˮƽÿ18�ȡ���ֱÿ18��ȷ��һ�����㣬
        �����͵ײ���һ�����㣬һ����9x20+2=182�����㡣
        ÿһ�������ڵ���һ��Ϊһ�飬֮�仭��40�������Σ�һ��8�顣
        �����͵ײ��������ڻ���20�������Σ�������������40x8+20x2=360,
        ��������������360x3=1080*/

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

    //�������
    public static double GetDistance(Vector3 _begin, Vector3 _end)
    {
        return Math.Sqrt(Math.Pow(_begin.x - _end.x, 2) + Math.Pow(_begin.y - _end.y, 2) + Math.Pow(_begin.z - _end.z, 2));
    }

    //�������
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

    //��������
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

    //�������������
    private static float CalculateTriangleArea(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 lossyScale)
    {
        //��������
        point1 = new Vector3(point1.x * lossyScale.x, point1.y * lossyScale.y, point1.z * lossyScale.z);
        point2 = new Vector3(point2.x * lossyScale.x, point2.y * lossyScale.y, point2.z * lossyScale.z);
        point3 = new Vector3(point3.x * lossyScale.x, point3.y * lossyScale.y, point3.z * lossyScale.z);

        //����߳�
        float l1 = (point2 - point1).magnitude;
        float l2 = (point3 - point2).magnitude;
        float l3 = (point1 - point3).magnitude;
        float p = (l1 + l2 + l3) * 0.5f;

        //�������  S=��[p(p-l1)(p-l2)(p-l3)]��pΪ���ܳ���
        return Mathf.Sqrt(p * (p - l1) * (p - l2) * (p - l3));
    }

    //��������׶���
    private static float CalculateVolumeOfTriangle(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 center, Vector3 lossyScale)
    {
        //��������
        point1 = new Vector3(point1.x * lossyScale.x, point1.y * lossyScale.y, point1.z * lossyScale.z);
        point2 = new Vector3(point2.x * lossyScale.x, point2.y * lossyScale.y, point2.z * lossyScale.z);
        point3 = new Vector3(point3.x * lossyScale.x, point3.y * lossyScale.y, point3.z * lossyScale.z);

        //����
        Vector3 v1 = point1 - center;
        Vector3 v2 = point2 - center;
        Vector3 v3 = point3 - center;

        //�������
        //����������������������Ϊ�����ƽ������������
        //�Ǿ��ǣ�a��b����c�ľ���ֵ
        //Ȼ��������������ƽ�������������֮һ
        //��Ϊ������ĵ���ƽ���������һ��,����Ҫ���һ������֮һ
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
