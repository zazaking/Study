using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using UnityEditor;

public class ImporterOBJ : MonoBehaviour
{

    private List<ObjMesh> listObjModel = new List<ObjMesh>();
    private List<ObjMatItem> listObjMats = new List<ObjMatItem>();
    private List<ObjPart> listObjParts = new List<ObjPart>();
    private GameObject goParent;

    public GameObject _gameObj;    //ģ�͸��ڵ�

    private bool isDone = true;    //true��������ȫ��������ɺ� ��ģ��������

    //��ť�¼�����
    public void LoadModelEvent()
    {
        //���ļ�
        string strObjPath = EditorUtility.OpenFilePanel("��ģ��", "", "obj");

        Settings.ObjPath = strObjPath;

        ObjMesh objInstace = new ObjMesh();
        objInstace = objInstace.LoadFromObj(strObjPath);
        if (objInstace == null)
            return;

        listObjModel.Add(objInstace);
        listObjParts = objInstace.listObjParts;//ģ��
        Debug.Log("Parts:" + listObjParts.Count);

        listObjMats = objInstace.listObjMats;  //����
        if (listObjMats != null)
            Debug.Log("Mats:" + listObjMats.Count);

        string strGameName = strObjPath;
        strGameName = strGameName.Substring(strGameName.LastIndexOf("/") + 1, strGameName.LastIndexOf(".") - (strGameName.LastIndexOf("/") + 1));
        //string[] names = strGameName.Split('\\');
        //strGameName = names[names.Length - 1];

        goParent = new GameObject(strGameName); //ģ����Դ����

        StartCoroutine(WaitLoadMaterialTexture());

        //�ƶ�
        goParent.transform.position = new Vector3(5, 5, -10);
        goParent.transform.transform.rotation = Quaternion.Euler(0, 0, 90);

        //��ӿ��ƽű�
        AddControlScript();
    }

    //�����е���Ԫ�ص��������һ���ű� ������������Ԫ�صĸ�Ԫ��
    private void AddControlScript()
    {
        int count;
        count = goParent.transform.childCount;    //��ȡ����������ĸ���
        for (int i = 0; i < count; i++)
        {
            if (goParent.transform.GetChild(i).GetComponent<CubeController>() == false)
            {
                //�����е������������interavtive�ű�
                goParent.transform.GetChild(i).gameObject.AddComponent<CubeController>();
                goParent.transform.GetChild(i).gameObject.GetComponent<CubeController>().enabled = true;
            }
        }
    }

    IEnumerator WaitLoadMaterialTexture()
    {
        while (!isDone && listObjMats != null) //�ȴ����ʴ������
        {
            yield return null;
        }

        //��������
        int i = 0;
        foreach (ObjPart part in listObjParts)
        {
            ++i;
            Mesh mesh = new Mesh();
            mesh.vertices = part.listVertex.ToArray();//����
            //mesh.triangles = part.listTriangle.ToArray();
            mesh.triangles = ResetTriangles(part.listTriangle.ToArray());//�޸��������� ��ת��������

            if (part.listUV.Count > 0)
            {
                mesh.uv = part.listUV.ToArray();
            }

            if (part.listNormal.Count > 0)
            {
                mesh.normals = part.listNormal.ToArray();
            }

            mesh.tangents = part.listTangent.ToArray(); //����

            mesh.RecalculateBounds();
            // mesh.RecalculateNormals(); //����
            //��������
            GameObject go = new GameObject(part.strMatName + i.ToString());
            // ==go.AddComponent<ObjDestroy>();

            MeshFilter meshFilter = go.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            //parts����洢���в������� ���ݲ����������ɲ�����
            MeshRenderer render = go.AddComponent<MeshRenderer>();
            RenderAddMaterials(go, part);

            go.transform.SetParent(goParent.transform);
        }

        goParent.transform.SetParent(_gameObj.transform);
    }

    //��ת����
    private Vector3[] FlipNormals(Vector3[] normals)
    {
        Vector3[] res = new Vector3[normals.Length];
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = -normals[i];

        }
        return normals;
    }

    //��ת��������Ƭ
    private int[] ResetTriangles(int[] triangles)
    {
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int t = triangles[i];
            triangles[i] = triangles[i + 2];
            triangles[i + 2] = t;
        }
        return triangles;
    }

    private void RenderAddMaterials(GameObject go, ObjPart part)
    {
        //��ģ����Ӳ�����
        if (Settings.ModelMaterialList.ContainsKey(part.strMatName))
        {
            go.GetComponent<MeshRenderer>().material = Settings.ModelMaterialList[part.strMatName];
        }
    }

    //����������Ҫ�Ĳ�����(mtl�ļ���obj�ļ���ȡ��ɺ󣬵���)
    public void CreatMaterial(List<ObjMatItem> matList)
    {
        int nowIndex = 0;
        foreach (ObjMatItem item in matList)
        {
            ++nowIndex;
            string textureName = item.map_Kd;//��ͼ����
            string texturePath = Directory.GetParent(Settings.ObjPath).FullName + "\\" + textureName;

            StartCoroutine(LoadTexture(item, nowIndex, matList.Count));
        }
    }

    /// <summary>
    /// ������ͼ   
    /// </summary>
    IEnumerator LoadTexture(ObjMatItem item, int nowIndex, int maxIndex)
    {
        string texturePath = Directory.GetParent(Settings.ObjPath).FullName + "\\" + item.map_Kd;  //ͼƬ·��
        string matName = item.strMatName;//��������

        if (!File.Exists(texturePath)) { yield break; } //��ֹЭ��
        UnityWebRequest request = new UnityWebRequest(texturePath);
        DownloadHandlerTexture tex = new DownloadHandlerTexture(true);
        request.downloadHandler = tex;
        yield return request.SendWebRequest();
        if (!request.isNetworkError)
        {
            Texture2D texture = tex.texture;       //���صĶ�������ʱҪ��� �����ڴ�ռ�û�Խ��Խ��TODO
            Material material = new Material(Shader.Find("Standard"));
            material.mainTexture = texture;
            Settings.ModelMaterialList.Add(matName, material);

            if (nowIndex == maxIndex)
            {
                isDone = true;
            }
        }
        else
        {
            Debug.LogError("load texture failed!");
        }
    }
}

//obj�ļ����ݽṹ
public class ObjPart
{

    public string strMatName;   //��������
    public List<Vector2> listUV; //vt UV��������
    public List<Vector3> listNormal;    //vn ��������
    public List<Vector4> listTangent;   //��������
    public List<Vector3> listVertex;    //v ��������
    public List<int> listTriangle;      //������ ������

    public ObjPart()
    {
        strMatName = "";
        listUV = new List<Vector2>();
        listNormal = new List<Vector3>();
        listVertex = new List<Vector3>();
        listTriangle = new List<int>();
        listTangent = new List<Vector4>();
    }
}

//mtl���ݽṹ����¼���ǲ��ʶ�Ӧ����ͼ��
public class ObjMatItem
{
    public string strMatName;   //��������
    public int illum;
    public Vector3 Kd;
    public Vector3 Ka;
    public Vector3 Tf;
    public Vector2 widthHeight;
    public string map_Kd;
    public float Ni;

    public ObjMatItem()
    {
        strMatName = "";
        illum = -1;
        Kd = new Vector3(0.0f, 0.0f, 0.0f);
        Ka = new Vector3(0.0f, 0.0f, 0.0f);
        Tf = new Vector3(0.0f, 0.0f, 0.0f);
        map_Kd = "";
        Ni = 1.0f;
        widthHeight.x = 0.0f;
        widthHeight.y = 0.0f;
    }
}

public class ObjModel
{
    public List<ObjPart> objParts;   //����
    public List<ObjMatItem> ObjMats; //����
}

//�����.obj�ļ��м�������
public class ObjMesh
{
    private List<Vector3> uvArrayList;  //UV�����б�
    private List<Vector3> normalArrayList;  //�����б�
    private List<Vector3> vertexArrayList;  //�����б�

    public List<ObjPart> listObjParts;
    public List<ObjMatItem> listObjMats;

    public string _strMatPath { get; set; }
    public string _strObjPath { get; set; }
    public string _strObjName { get; set; }

    //���캯��    
    public ObjMesh()
    {
        //��ʼ���б�
        uvArrayList = new List<Vector3>();
        normalArrayList = new List<Vector3>();
        vertexArrayList = new List<Vector3>();

        listObjParts = new List<ObjPart>();
        listObjMats = new List<ObjMatItem>();
        _strMatPath = _strObjName = "";
    }

    //��һ���ı������.obj�ļ��м���ģ��
    public ObjMesh LoadFromObj(string strObjPath)
    {

        _strObjPath = strObjPath;
        //_strObjName = strObjPath;
        //_strObjName = _strObjName.Replace("/", "");
        _strObjName = _strObjPath.Substring(_strObjPath.LastIndexOf("/"), _strObjPath.LastIndexOf(".") - _strObjPath.LastIndexOf("/"));

        //��ȡ����
        if (!File.Exists(strObjPath))
            return null;

        //��ȡ����
        StreamReader reader = new StreamReader(strObjPath, System.Text.Encoding.Default);
        string objText = reader.ReadToEnd();
        reader.Close();

        if (objText.Length <= 0)
            return null;

        //v��һ����3dsMax�е�����.obj�ļ�
        //ǰ���������ո������һ���ո�
        objText = objText.Replace("  ", " ");

        //���ı������obj�ļ����ݰ��зָ�
        string[] allLines = objText.Split('\n');
        foreach (string line in allLines)
        {
            //��ÿһ�а��ո�ָ�
            char[] charsToTrim = { ' ' };
            string[] chars = line.TrimEnd('\r').TrimStart(' ').Split(charsToTrim, StringSplitOptions.RemoveEmptyEntries);
            if (chars.Length <= 0)
            {
                continue;
            }
            //���ݵ�һ���ַ����ж����ݵ�����
            switch (chars[0])
            {
                case "mtllib":
                    _strMatPath = _strObjPath.Substring(0, _strObjPath.LastIndexOf('/') + 1) + chars[1];
                    break;
                case "v":
                    //������
                    this.vertexArrayList.Add(new Vector3(
                        -(ConvertToFloat(chars[1])),
                        ConvertToFloat(chars[2]),
                        ConvertToFloat(chars[3]))
                    );
                    break;
                case "vn":
                    //������
                    this.normalArrayList.Add(new Vector3(
                        -ConvertToFloat(chars[1]),
                        ConvertToFloat(chars[2]),
                        ConvertToFloat(chars[3]))
                    );
                    break;
                case "vt":
                    //����UV
                    this.uvArrayList.Add(new Vector3(
                        ConvertToFloat(chars[1]),
                        ConvertToFloat(chars[2]))
                    );
                    break;
                case "usemtl":
                    ObjPart objPart = new ObjPart();
                    objPart.strMatName = chars[1];//��������
                    listObjParts.Add(objPart);
                    break;
                case "f":
                    //������
                    GetTriangleList(chars);
                    break;
            }
        }

        //��ȡmtl�ļ�·��
        //string mtlFilePath = strObjPath.Replace(".obj", ".mtl");
        if (_strMatPath != "")
        {
            LoadMat(_strMatPath);
        }

        return this;
    }

    private void LoadMat(string strmtlPath)
    {
        //��mtl�ļ��м��ز���
        listObjMats = ObjMaterial.Instance.LoadFormMtl(strmtlPath);
    }

    //��ȡ���б�.
    private List<Vector3> indexVectorList = new List<Vector3>();
    private Vector3 indexVector = new Vector3(0, 0);
    private void GetTriangleList(string[] chars)
    {
        indexVectorList.Clear();
        for (int i = 1; i < chars.Length; ++i)
        {
            //��ÿһ�а��տո�ָ��ӵ�һ��Ԫ�ؿ�ʼ
            //����/�����ָ�����λ�ö�������������������UV����
            string[] indexs = chars[i].Split('/');
            Vector3 vertex = (Vector3)vertexArrayList[ConvertToInt(indexs[0]) - 1];
            listObjParts[listObjParts.Count - 1].listVertex.Add(vertex);

            indexVector = new Vector3(0, 0);
            //UV����
            if (indexs.Length > 1)
            {
                if (indexs[1] != "")
                    indexVector.y = ConvertToInt(indexs[1]);
            }

            //��������
            if (indexs.Length > 2)
            {
                if (indexs[2] != "")
                    indexVector.z = ConvertToInt(indexs[2]);
            }

            //��UV���鸳ֵ
            if (uvArrayList.Count > 0 && indexVector.y > 0.01)
            {
                Vector3 tVec = (Vector3)uvArrayList[(int)indexVector.y - 1];
                listObjParts[listObjParts.Count - 1].listUV.Add(new Vector2(tVec.x, tVec.y));
            }

            //���������鸳ֵ
            if (normalArrayList.Count > 0 && indexVector.z > 0.01)
            {
                Vector3 nVec = (Vector3)normalArrayList[(int)indexVector.z - 1];
                listObjParts[listObjParts.Count - 1].listNormal.Add(nVec);
            }

            //���������������б���
            indexVectorList.Add(indexVector);
        }

        //������
        int nCount = listObjParts[listObjParts.Count - 1].listVertex.Count - indexVectorList.Count; //nCount==0 3 6 9 indexVectorList.Count=3 ��������
        //Debug.Log(indexVectorList.Count);
        for (int j = 1; j < indexVectorList.Count - 1; ++j)
        {
            //����0,1,2�����ķ�ʽ�������          
            listObjParts[listObjParts.Count - 1].listTriangle.Add(nCount);
            listObjParts[listObjParts.Count - 1].listTriangle.Add(nCount + j);
            listObjParts[listObjParts.Count - 1].listTriangle.Add(nCount + j + 1);

        }
    }

    //��һ���ַ���ת��Ϊ��������
    private float ConvertToFloat(string s)
    {
        //return (float)System.Convert.ToDouble(s,CultureInfo.InvariantCulture);
        float fValue = 0.0f;
        try
        {
            fValue = (float)Convert.ToDouble(s);
        }
        catch (Exception ex)
        {
            Debug.LogError("����[" + s + "]ת��ʧ��! " + ex.Message);
        }

        return fValue;
    }

    // ��һ���ַ���ת��Ϊ���� /// </summary>
    private int ConvertToInt(string s)
    {
        int nValue = 0;
        try
        {
            nValue = Convert.ToInt32(s);
        }
        catch (Exception ex)
        {
            Debug.LogError("����[" + s + "]ת��ʧ��! " + ex.Message);
        }
        return nValue;
    }
}


//�����.mtl�м�������
public class ObjMaterial
{
    //��ǰʵ��
    private static ObjMaterial instance = new ObjMaterial();
    public static ObjMaterial Instance
    {
        get
        {
            if (instance == null)
                instance = new ObjMaterial();//GameObject.FindObjectOfType<ObjMaterial>(); 
            return instance;
        }
    }

    /// <summary>
    /// ��һ���ı������mtl�ļ�����һ�����
    /// </summary>
    /// <param name="mtlText">�ı�����mtl�ļ�</param>
    /// <param name="texturePath">��ͼ�ļ���·��</param>
    public List<ObjMatItem> LoadFormMtl(string strMtlText)
    {
        List<ObjMatItem> listObjMats = new List<ObjMatItem>();
        DirectoryInfo mtlParent = Directory.GetParent(Settings.ObjPath);

        if (!File.Exists(mtlParent + "\\" + strMtlText)) return null;

        Stream mtlStream = new FileStream(mtlParent + "\\" + strMtlText, FileMode.Open);//mtl�ļ���obj�ļ�����ͬһ��Ŀ¼��
        StreamReader reader = new StreamReader(mtlStream);
        string mtlText = reader.ReadToEnd();
        reader.Close();
        if (mtlText == "")
            return listObjMats;

        //���ı���������ݰ��зָ�
        string[] allLines = mtlText.Split('\n');
        foreach (string line in allLines)
        {
            //���տո�ָ�ÿһ�е�����
            string[] chars = line.TrimEnd('\r').TrimStart(' ').Split(' ');
            switch (chars[0])
            {
                case "newmtl":
                    //���������
                    ObjMatItem matItem = new ObjMatItem();
                    matItem.strMatName = chars[1];
                    listObjMats.Add(matItem);
                    //������ͼ����������
                    break;
                case "Ka":
                    listObjMats[listObjMats.Count - 1].Ka = new Vector3(
                        ConvertToFloat(chars[1]),
                        ConvertToFloat(chars[2]),
                        ConvertToFloat(chars[3])
                        );
                    break;
                case "Kd":
                    //����������
                    listObjMats[listObjMats.Count - 1].Kd = new Vector3(
                        ConvertToFloat(chars[1]),
                        ConvertToFloat(chars[2]),
                        ConvertToFloat(chars[3])
                        );
                    break;
                case "Ks":
                    //��ʱ������������
                    break;
                case "Ke":
                    //Todo
                    break;
                case "Tf":
                    //����������
                    listObjMats[listObjMats.Count - 1].Tf = new Vector3(
                        ConvertToFloat(chars[1]),
                        ConvertToFloat(chars[2]),
                        ConvertToFloat(chars[3])
                        );
                    break;
                case "Ni":
                    listObjMats[listObjMats.Count - 1].Ni = ConvertToFloat(chars[1]);
                    break;
                case "e":
                    //Todo
                    break;
                case "illum":
                    listObjMats[listObjMats.Count - 1].illum = Convert.ToInt32(chars[1]);
                    break;
                case "map_Ka":
                    //��ʱ������������
                    break;
                case "map_Kd":
                    //������������ͼ
                    string textureName = chars[1].Substring(chars[1].LastIndexOf("\\") + 1, chars[1].Length - chars[1].LastIndexOf("\\") - 1);
                    listObjMats[listObjMats.Count - 1].map_Kd = textureName;

                    break;
                case "map_Ks":
                    //��ʱ������������
                    break;
            }
        }
        GameObject.Find("Canvas").GetComponent<ImporterOBJ>().CreatMaterial(listObjMats);
        return listObjMats;
    }

    /// <summary>
    /// ��һ���ַ���ת��Ϊ��������
    /// </summary>
    /// <param name="s">��ת�����ַ���</param>
    /// <returns></returns>
    private float ConvertToFloat(string s)
    {
        return System.Convert.ToSingle(s);
    }
}

public class Settings
{
    public static string ObjPath = null;           //ģ��·��
    public static Dictionary<string, Material> ModelMaterialList = new Dictionary<string, Material>();//������洢 key:�������� value��������
}

/// <summary>
/// ����ģ��=����+����
/// </summary>
