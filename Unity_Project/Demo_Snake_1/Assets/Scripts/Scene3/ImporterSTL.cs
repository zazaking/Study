using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Parabox.Stl;
using System.IO;

public class ImporterSTL : MonoBehaviour
{
    const string k_TempDir = "Assets/Scripts/Scene3";
    public Texture m_MainTexture;
    public GameObject parentGameObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadModelEvent()
    {
        //打开文件
        string strSTLPath = EditorUtility.OpenFilePanel("打开模型", "", "stl");
        string strGameName = strSTLPath.Substring(strSTLPath.LastIndexOf("/") + 1, strSTLPath.LastIndexOf(".") - (strSTLPath.LastIndexOf("/") + 1));
        string strGameURL = strSTLPath.Substring(0 , strSTLPath.LastIndexOf("/"));
       

        if (!Directory.Exists(strGameURL))
            Directory.CreateDirectory(strGameURL);

        Mesh[] meshes = Importer.Import(strSTLPath);

        for (int i = 0;i< meshes.Length;++i)
        {
            GameObject simpleMesh = new GameObject();

            simpleMesh.transform.SetParent(parentGameObject.transform);

            simpleMesh.name = strGameName;
            simpleMesh.transform.position = new Vector3(5, 5, -10);

            // 添加MeshFilter  
            simpleMesh.AddComponent<MeshFilter>();
            // 添加MeshRenderer  
            simpleMesh.AddComponent<MeshRenderer>();
            simpleMesh.GetComponent<MeshFilter>().mesh = meshes[i];
            // 添加CubeController脚本
            simpleMesh.gameObject.AddComponent<CubeController>();
            simpleMesh.gameObject.GetComponent<CubeController>().enabled = true;

            //Make sure to enable the Keywords
            simpleMesh.gameObject.GetComponent<Renderer>().material.EnableKeyword("_NORMALMAP");
            simpleMesh.gameObject.GetComponent<Renderer>().material.EnableKeyword("_METALLICGLOSSMAP");

            simpleMesh.gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", m_MainTexture);//Material.SetTexture设置贴图属性
        }
    }
}

