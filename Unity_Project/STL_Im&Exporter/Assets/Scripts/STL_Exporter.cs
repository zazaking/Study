using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;

public class STL_Exporter : MonoBehaviour
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

    public void ExportSTLModel()
    {
        //��ȡģ�ͱ���λ��
        string strSaveURL = EditorUtility.OpenFolderPanel("����ģ��", "", "");

        if (StlGoParent == null || strSaveURL.Length <= 0)
        {
            return;
        }

        //����������µ�����GameObject���ֱ������ļ�
        for (int i = 0; i < StlGoParent.transform.childCount; ++i)
        {
            //���ɵ���ģ����Ϣ
            GameObject stlGameObject = StlGoParent.transform.GetChild(i).gameObject;
            string strSavePath = strSaveURL + "/" + stlGameObject.name + ".stl";

            //����STLģ��
            StlExporter(stlGameObject, strSavePath);
        }

        //��յ�����GameObject
        while (StlGoParent.transform.childCount != 0)
        {
            DestroyImmediate(StlGoParent.transform.GetChild(0).gameObject);
        }
    }

    private void StlExporter(GameObject stlGameObject, string strSavePath)
    {
        //�ļ��Ѵ�����ɾ��
        if (File.Exists(strSavePath))
            File.Delete(strSavePath);

        //���ɵ���ģ����Ϣ
        string strSaveName = stlGameObject.name;
        Vector3[] v = stlGameObject.GetComponent<MeshFilter>().mesh.vertices;
        Vector3[] n = stlGameObject.GetComponent<MeshFilter>().mesh.normals;
        int[] t = stlGameObject.GetComponent<MeshFilter>().mesh.triangles;

        //�����ļ���
        StringBuilder sb = new StringBuilder();

        //д�ļ�
        sb.AppendLine(string.Format("solid {0}", strSaveName));

        int triLen = t.Length;
        for (int i = 0; i < triLen; i += 3)
        {
            int a = t[i];
            int b = t[i + 1];
            int c = t[i + 2];

            Vector3 nrm = AvgNrm(n[a], n[b], n[c]);

            sb.AppendLine(string.Format("facet normal {0} {1} {2}", nrm.x, nrm.y, nrm.z));

            sb.AppendLine("outer loop");

            sb.AppendLine(string.Format("\tvertex {0} {1} {2}", v[a].x, v[a].y, v[a].z));
            sb.AppendLine(string.Format("\tvertex {0} {1} {2}", v[b].x, v[b].y, v[b].z));
            sb.AppendLine(string.Format("\tvertex {0} {1} {2}", v[c].x, v[c].y, v[c].z));

            sb.AppendLine("endloop");
            sb.AppendLine("endfacet");
        }

        sb.AppendLine(string.Format("endsolid {0}", strSaveName));

        File.WriteAllText(strSavePath, sb.ToString());
    }

    private static Vector3 AvgNrm(Vector3 a, Vector3 b, Vector3 c)
    {
        return new Vector3(
            (a.x + b.x + c.x) / 3f,
            (a.y + b.y + c.y) / 3f,
            (a.z + b.z + c.z) / 3f);
    }
}
