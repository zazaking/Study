using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;

public class STL_Importer : MonoBehaviour
{
    //��GameObject�´�����е������ɵ�ģ��
    public GameObject StlGoParent;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ImportSTLModel()
    {
        //��ȡSTL�ļ�·��
        string strStlPath = EditorUtility.OpenFilePanel("��ģ��", "", "stl");
        Debug.Log("Path:" + strStlPath);
        if (strStlPath.Length <= 0)
        {
            return;
        }

        //����STLģ��
        StlImporter(strStlPath);
    }

    private void StlImporter(string strStlPath)
    {
        //mesh��Ϣ�б�
        List<Vector3> listVertices = new List<Vector3>();   //���񶥵�����
        List<Vector3> listNormals = new List<Vector3>();    //����ķ�������

        //��ȡ�ļ���
        StreamReader sr = new StreamReader(strStlPath);
        string line;
        while ((line = sr.ReadLine()) != null)
        {
            if (line.StartsWith("solid"))
            {

            }
            else if (line.StartsWith("facet"))
            {
                Vector3 normal = StringToVec3(line.Replace("facet normal ", ""));
                listNormals.Add(normal);
                listNormals.Add(normal);
                listNormals.Add(normal);
            }
            else if (line.StartsWith("outer loop"))
            {
            }
            else if (line.StartsWith("endloop"))
            {

            }
            else if (line.StartsWith("endfacet"))
            {

            }
            else if (line.StartsWith("	vertex"))
            {
                Vector3 vertex = StringToVec3(line.Replace("vertex ", ""));
                listVertices.Add(vertex);
            }
        }

        //����STLģ�͵�GameObject�ļ�
        //��ȡ�������
        string strStlName = strStlPath.Substring(strStlPath.LastIndexOf("/") + 1, strStlPath.LastIndexOf(".") - (strStlPath.LastIndexOf("/") + 1));
        strStlName = strStlName.Length == 0 ? "StlGameObject" : strStlName;
        Debug.Log("Name:" + strStlName);

        GameObject StlGameObject = new GameObject();
        StlGameObject.transform.SetParent(StlGoParent.transform);

        StlGameObject.name = strStlName;
        StlGameObject.AddComponent<MeshFilter>();
        StlGameObject.AddComponent<MeshRenderer>();
        StlGameObject.AddComponent<BoxCollider>();

        int len = listNormals.Count;
        Vector3[] v = new Vector3[len];
        Vector3[] n = new Vector3[len];
        int[] t = new int[len];

        for (int i = 0; i < len; ++i)
        {
            v[i] = listVertices[i];
            n[i] = listNormals[i];
            t[i] = i;
        }

        Mesh mesh = new Mesh
        {
            vertices = v,
            normals = n,
            triangles = t
        };

        StlGameObject.GetComponent<MeshFilter>().mesh = mesh;

        //��Ӳ���
        string[] KeyWords = {
            "_NORMALMAP" ,
            "_ALPHATEST_ON" ,
            "_ALPHABLEND_ON",
            "_ALPHAPREMULTIPLY_ON" ,
            "_EMISSION",
            "_PARALLAXMAP" ,
            "_DETAIL_MULX2" ,
            //"_METALLICGLOSSMAP",
            "_SPECGLOSSMAP"
        };
        foreach (string keyword in KeyWords)
        {
            StlGameObject.gameObject.GetComponent<Renderer>().material.EnableKeyword(keyword);
        }

        //����˶����ƽű�
        StlGameObject.gameObject.AddComponent<ModelConroller>();
        StlGameObject.gameObject.GetComponent<ModelConroller>().enabled = true;

    }

    static Vector3 StringToVec3(string str)
    {
        string[] split = str.Trim().Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
        Vector3 v = new Vector3();

        float.TryParse(split[0], out v.x);
        float.TryParse(split[1], out v.y);
        float.TryParse(split[2], out v.z);

        return v;
    }
}
