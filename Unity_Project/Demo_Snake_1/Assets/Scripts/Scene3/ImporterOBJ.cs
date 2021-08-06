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

    public GameObject _gameObj;    //模型父节点

    private bool isDone = true;    //true：材质球全部创建完成后 给模型贴材质

    //按钮事件处理
    public void LoadModelEvent()
    {
        //打开文件
        string strObjPath = EditorUtility.OpenFilePanel("打开模型", "", "obj");

        Settings.ObjPath = strObjPath;

        ObjMesh objInstace = new ObjMesh();
        objInstace = objInstace.LoadFromObj(strObjPath);
        if (objInstace == null)
            return;

        listObjModel.Add(objInstace);
        listObjParts = objInstace.listObjParts;//模型
        Debug.Log("Parts:" + listObjParts.Count);

        listObjMats = objInstace.listObjMats;  //材质
        if (listObjMats != null)
            Debug.Log("Mats:" + listObjMats.Count);

        string strGameName = strObjPath;
        strGameName = strGameName.Substring(strGameName.LastIndexOf("/") + 1, strGameName.LastIndexOf(".") - (strGameName.LastIndexOf("/") + 1));
        //string[] names = strGameName.Split('\\');
        //strGameName = names[names.Length - 1];

        goParent = new GameObject(strGameName); //模型资源名称

        StartCoroutine(WaitLoadMaterialTexture());

        //移动
        goParent.transform.position = new Vector3(5, 5, -10);
        goParent.transform.transform.rotation = Quaternion.Euler(0, 0, 90);

        //添加控制脚本
        AddControlScript();
    }

    //给所有的子元素的物体添加一个脚本 该物体是所有元素的父元素
    private void AddControlScript()
    {
        int count;
        count = goParent.transform.childCount;    //获取所有子物体的个数
        for (int i = 0; i < count; i++)
        {
            if (goParent.transform.GetChild(i).GetComponent<CubeController>() == false)
            {
                //给所有的子物体添加上interavtive脚本
                goParent.transform.GetChild(i).gameObject.AddComponent<CubeController>();
                goParent.transform.GetChild(i).gameObject.GetComponent<CubeController>().enabled = true;
            }
        }
    }

    IEnumerator WaitLoadMaterialTexture()
    {
        while (!isDone && listObjMats != null) //等待材质创建完成
        {
            yield return null;
        }

        //计算网格
        int i = 0;
        foreach (ObjPart part in listObjParts)
        {
            ++i;
            Mesh mesh = new Mesh();
            mesh.vertices = part.listVertex.ToArray();//顶点
            //mesh.triangles = part.listTriangle.ToArray();
            mesh.triangles = ResetTriangles(part.listTriangle.ToArray());//修改三角形面 翻转三角形面

            if (part.listUV.Count > 0)
            {
                mesh.uv = part.listUV.ToArray();
            }

            if (part.listNormal.Count > 0)
            {
                mesh.normals = part.listNormal.ToArray();
            }

            mesh.tangents = part.listTangent.ToArray(); //切线

            mesh.RecalculateBounds();
            // mesh.RecalculateNormals(); //法线
            //生成物体
            GameObject go = new GameObject(part.strMatName + i.ToString());
            // ==go.AddComponent<ObjDestroy>();

            MeshFilter meshFilter = go.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            //parts里面存储的有材质名称 根据材质名称生成材质球
            MeshRenderer render = go.AddComponent<MeshRenderer>();
            RenderAddMaterials(go, part);

            go.transform.SetParent(goParent.transform);
        }

        goParent.transform.SetParent(_gameObj.transform);
    }

    //翻转法线
    private Vector3[] FlipNormals(Vector3[] normals)
    {
        Vector3[] res = new Vector3[normals.Length];
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = -normals[i];

        }
        return normals;
    }

    //翻转三角形面片
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
        //给模型添加材质球
        if (Settings.ModelMaterialList.ContainsKey(part.strMatName))
        {
            go.GetComponent<MeshRenderer>().material = Settings.ModelMaterialList[part.strMatName];
        }
    }

    //创建所有需要的材质球(mtl文件与obj文件读取完成后，调用)
    public void CreatMaterial(List<ObjMatItem> matList)
    {
        int nowIndex = 0;
        foreach (ObjMatItem item in matList)
        {
            ++nowIndex;
            string textureName = item.map_Kd;//贴图名称
            string texturePath = Directory.GetParent(Settings.ObjPath).FullName + "\\" + textureName;

            StartCoroutine(LoadTexture(item, nowIndex, matList.Count));
        }
    }

    /// <summary>
    /// 加载贴图   
    /// </summary>
    IEnumerator LoadTexture(ObjMatItem item, int nowIndex, int maxIndex)
    {
        string texturePath = Directory.GetParent(Settings.ObjPath).FullName + "\\" + item.map_Kd;  //图片路径
        string matName = item.strMatName;//材质名称

        if (!File.Exists(texturePath)) { yield break; } //终止协成
        UnityWebRequest request = new UnityWebRequest(texturePath);
        DownloadHandlerTexture tex = new DownloadHandlerTexture(true);
        request.downloadHandler = tex;
        yield return request.SendWebRequest();
        if (!request.isNetworkError)
        {
            Texture2D texture = tex.texture;       //下载的东西不用时要清空 否则内存占用会越来越多TODO
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

//obj文件数据结构
public class ObjPart
{

    public string strMatName;   //材质名称
    public List<Vector2> listUV; //vt UV坐标数组
    public List<Vector3> listNormal;    //vn 法线数组
    public List<Vector4> listTangent;   //切线数组
    public List<Vector3> listVertex;    //v 顶点数组
    public List<int> listTriangle;      //面数组 面索引

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

//mtl数据结构（记录的是材质对应的贴图）
public class ObjMatItem
{
    public string strMatName;   //材质名称
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
    public List<ObjPart> objParts;   //网格
    public List<ObjMatItem> ObjMats; //材质
}

//负责从.obj文件中加载数据
public class ObjMesh
{
    private List<Vector3> uvArrayList;  //UV坐标列表
    private List<Vector3> normalArrayList;  //法线列表
    private List<Vector3> vertexArrayList;  //顶点列表

    public List<ObjPart> listObjParts;
    public List<ObjMatItem> listObjMats;

    public string _strMatPath { get; set; }
    public string _strObjPath { get; set; }
    public string _strObjName { get; set; }

    //构造函数    
    public ObjMesh()
    {
        //初始化列表
        uvArrayList = new List<Vector3>();
        normalArrayList = new List<Vector3>();
        vertexArrayList = new List<Vector3>();

        listObjParts = new List<ObjPart>();
        listObjMats = new List<ObjMatItem>();
        _strMatPath = _strObjName = "";
    }

    //从一个文本化后的.obj文件中加载模型
    public ObjMesh LoadFromObj(string strObjPath)
    {

        _strObjPath = strObjPath;
        //_strObjName = strObjPath;
        //_strObjName = _strObjName.Replace("/", "");
        _strObjName = _strObjPath.Substring(_strObjPath.LastIndexOf("/"), _strObjPath.LastIndexOf(".") - _strObjPath.LastIndexOf("/"));

        //读取内容
        if (!File.Exists(strObjPath))
            return null;

        //读取数据
        StreamReader reader = new StreamReader(strObjPath, System.Text.Encoding.Default);
        string objText = reader.ReadToEnd();
        reader.Close();

        if (objText.Length <= 0)
            return null;

        //v这一行在3dsMax中导出的.obj文件
        //前面是两个空格后面是一个空格
        objText = objText.Replace("  ", " ");

        //将文本化后的obj文件内容按行分割
        string[] allLines = objText.Split('\n');
        foreach (string line in allLines)
        {
            //将每一行按空格分割
            char[] charsToTrim = { ' ' };
            string[] chars = line.TrimEnd('\r').TrimStart(' ').Split(charsToTrim, StringSplitOptions.RemoveEmptyEntries);
            if (chars.Length <= 0)
            {
                continue;
            }
            //根据第一个字符来判断数据的类型
            switch (chars[0])
            {
                case "mtllib":
                    _strMatPath = _strObjPath.Substring(0, _strObjPath.LastIndexOf('/') + 1) + chars[1];
                    break;
                case "v":
                    //处理顶点
                    this.vertexArrayList.Add(new Vector3(
                        -(ConvertToFloat(chars[1])),
                        ConvertToFloat(chars[2]),
                        ConvertToFloat(chars[3]))
                    );
                    break;
                case "vn":
                    //处理法线
                    this.normalArrayList.Add(new Vector3(
                        -ConvertToFloat(chars[1]),
                        ConvertToFloat(chars[2]),
                        ConvertToFloat(chars[3]))
                    );
                    break;
                case "vt":
                    //处理UV
                    this.uvArrayList.Add(new Vector3(
                        ConvertToFloat(chars[1]),
                        ConvertToFloat(chars[2]))
                    );
                    break;
                case "usemtl":
                    ObjPart objPart = new ObjPart();
                    objPart.strMatName = chars[1];//材质名称
                    listObjParts.Add(objPart);
                    break;
                case "f":
                    //处理面
                    GetTriangleList(chars);
                    break;
            }
        }

        //获取mtl文件路径
        //string mtlFilePath = strObjPath.Replace(".obj", ".mtl");
        if (_strMatPath != "")
        {
            LoadMat(_strMatPath);
        }

        return this;
    }

    private void LoadMat(string strmtlPath)
    {
        //从mtl文件中加载材质
        listObjMats = ObjMaterial.Instance.LoadFormMtl(strmtlPath);
    }

    //获取面列表.
    private List<Vector3> indexVectorList = new List<Vector3>();
    private Vector3 indexVector = new Vector3(0, 0);
    private void GetTriangleList(string[] chars)
    {
        indexVectorList.Clear();
        for (int i = 1; i < chars.Length; ++i)
        {
            //将每一行按照空格分割后从第一个元素开始
            //按照/继续分割可依次获得顶点索引、法线索引和UV索引
            string[] indexs = chars[i].Split('/');
            Vector3 vertex = (Vector3)vertexArrayList[ConvertToInt(indexs[0]) - 1];
            listObjParts[listObjParts.Count - 1].listVertex.Add(vertex);

            indexVector = new Vector3(0, 0);
            //UV索引
            if (indexs.Length > 1)
            {
                if (indexs[1] != "")
                    indexVector.y = ConvertToInt(indexs[1]);
            }

            //法线索引
            if (indexs.Length > 2)
            {
                if (indexs[2] != "")
                    indexVector.z = ConvertToInt(indexs[2]);
            }

            //给UV数组赋值
            if (uvArrayList.Count > 0 && indexVector.y > 0.01)
            {
                Vector3 tVec = (Vector3)uvArrayList[(int)indexVector.y - 1];
                listObjParts[listObjParts.Count - 1].listUV.Add(new Vector2(tVec.x, tVec.y));
            }

            //给法线数组赋值
            if (normalArrayList.Count > 0 && indexVector.z > 0.01)
            {
                Vector3 nVec = (Vector3)normalArrayList[(int)indexVector.z - 1];
                listObjParts[listObjParts.Count - 1].listNormal.Add(nVec);
            }

            //将索引向量加入列表中
            indexVectorList.Add(indexVector);
        }

        //面索引
        int nCount = listObjParts[listObjParts.Count - 1].listVertex.Count - indexVectorList.Count; //nCount==0 3 6 9 indexVectorList.Count=3 三角形面
        //Debug.Log(indexVectorList.Count);
        for (int j = 1; j < indexVectorList.Count - 1; ++j)
        {
            //按照0,1,2这样的方式来组成面          
            listObjParts[listObjParts.Count - 1].listTriangle.Add(nCount);
            listObjParts[listObjParts.Count - 1].listTriangle.Add(nCount + j);
            listObjParts[listObjParts.Count - 1].listTriangle.Add(nCount + j + 1);

        }
    }

    //将一个字符串转换为浮点类型
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
            Debug.LogError("数据[" + s + "]转换失败! " + ex.Message);
        }

        return fValue;
    }

    // 将一个字符串转化为整型 /// </summary>
    private int ConvertToInt(string s)
    {
        int nValue = 0;
        try
        {
            nValue = Convert.ToInt32(s);
        }
        catch (Exception ex)
        {
            Debug.LogError("数据[" + s + "]转换失败! " + ex.Message);
        }
        return nValue;
    }
}


//负责从.mtl中加载数据
public class ObjMaterial
{
    //当前实例
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
    /// 从一个文本化后的mtl文件加载一组材质
    /// </summary>
    /// <param name="mtlText">文本化的mtl文件</param>
    /// <param name="texturePath">贴图文件夹路径</param>
    public List<ObjMatItem> LoadFormMtl(string strMtlText)
    {
        List<ObjMatItem> listObjMats = new List<ObjMatItem>();
        DirectoryInfo mtlParent = Directory.GetParent(Settings.ObjPath);

        if (!File.Exists(mtlParent + "\\" + strMtlText)) return null;

        Stream mtlStream = new FileStream(mtlParent + "\\" + strMtlText, FileMode.Open);//mtl文件与obj文件处于同一级目录下
        StreamReader reader = new StreamReader(mtlStream);
        string mtlText = reader.ReadToEnd();
        reader.Close();
        if (mtlText == "")
            return listObjMats;

        //将文本化后的内容按行分割
        string[] allLines = mtlText.Split('\n');
        foreach (string line in allLines)
        {
            //按照空格分割每一行的内容
            string[] chars = line.TrimEnd('\r').TrimStart(' ').Split(' ');
            switch (chars[0])
            {
                case "newmtl":
                    //处理材质名
                    ObjMatItem matItem = new ObjMatItem();
                    matItem.strMatName = chars[1];
                    listObjMats.Add(matItem);
                    //根据贴图创建材质球
                    break;
                case "Ka":
                    listObjMats[listObjMats.Count - 1].Ka = new Vector3(
                        ConvertToFloat(chars[1]),
                        ConvertToFloat(chars[2]),
                        ConvertToFloat(chars[3])
                        );
                    break;
                case "Kd":
                    //处理漫反射
                    listObjMats[listObjMats.Count - 1].Kd = new Vector3(
                        ConvertToFloat(chars[1]),
                        ConvertToFloat(chars[2]),
                        ConvertToFloat(chars[3])
                        );
                    break;
                case "Ks":
                    //暂时仅考虑漫反射
                    break;
                case "Ke":
                    //Todo
                    break;
                case "Tf":
                    //处理漫反射
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
                    //暂时仅考虑漫反射
                    break;
                case "map_Kd":
                    //处理漫反射贴图
                    string textureName = chars[1].Substring(chars[1].LastIndexOf("\\") + 1, chars[1].Length - chars[1].LastIndexOf("\\") - 1);
                    listObjMats[listObjMats.Count - 1].map_Kd = textureName;

                    break;
                case "map_Ks":
                    //暂时仅考虑漫反射
                    break;
            }
        }
        GameObject.Find("Canvas").GetComponent<ImporterOBJ>().CreatMaterial(listObjMats);
        return listObjMats;
    }

    /// <summary>
    /// 将一个字符串转换为浮点类型
    /// </summary>
    /// <param name="s">待转换的字符串</param>
    /// <returns></returns>
    private float ConvertToFloat(string s)
    {
        return System.Convert.ToSingle(s);
    }
}

public class Settings
{
    public static string ObjPath = null;           //模型路径
    public static Dictionary<string, Material> ModelMaterialList = new Dictionary<string, Material>();//材质球存储 key:材质名称 value：材质球
}

/// <summary>
/// 完整模型=网格+材质
/// </summary>
