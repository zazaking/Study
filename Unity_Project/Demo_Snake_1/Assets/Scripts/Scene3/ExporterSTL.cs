using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Parabox.Stl;

public class ExporterSTL : MonoBehaviour
{
    //go��ʾģ�����
    public GameObject parentGameObject;
    private GameObject[] GameObjectList;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExportModelEvent()
    {
        string strURL = EditorUtility.OpenFolderPanel("����ģ��", "", ""); //ģ�ͱ���λ��

        //��ȡ��������ĸ���
        int parentCount = parentGameObject.transform.childCount;
        GameObjectList = new GameObject[parentCount];

        for (int i = 0; i < parentCount; i++)
        {
            GameObjectList[i] = parentGameObject.transform.GetChild(i).gameObject;
        }

        Exporter.Export(strURL + "/" + parentGameObject.name + ".stl", GameObjectList, FileType.Ascii);

        for (int i = 0; i < parentCount; i++)
        {
            DestroyImmediate(GameObjectList[i],true);
        }
        GameObjectList = null; //�ڴ��ͷ�
    }
}