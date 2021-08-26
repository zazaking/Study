using UnityEngine;
using UnityEditor;
using VideoZAZA;

public class LoadRecord : MonoBehaviour
{
    public GameObject goParent;
    private int stepCount = 0;

    ZAZA_Controller zaza;

    void Start()
    {
        if(zaza == null)
        {
            zaza = new ZAZA_Controller();
        }
    }

    public void OnClick()
    {
        ImportRecord();
    }

    public void ImportRecord()
    {
        //获取文件路径
        string strRecordPath = EditorUtility.OpenFilePanel("打开模型", Application.dataPath, "zaza");
        Debug.Log("strRecordPath:" + strRecordPath);
        if (strRecordPath.Length <= 0)
            return;

        for (int i = goParent.transform.childCount - 1; i >= 0; --i)
        {
            GameObject go = goParent.transform.GetChild(i).gameObject;
            DestroyImmediate(go);
        }

        zaza.LoadVideoFile(strRecordPath);
        //zaza.DebugVideoInfo();

        stepCount = 0;
    }

    void FixedUpdate()
    {
        if (zaza != null && zaza.isPlay())
        {
            zaza.PlayOneStep(goParent, stepCount);
            ++stepCount;
        }
    }
}
