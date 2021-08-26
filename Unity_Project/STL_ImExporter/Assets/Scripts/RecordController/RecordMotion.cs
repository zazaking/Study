using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VideoZAZA;


public class RecordMotion : MonoBehaviour
{
    public GameObject goParent;     //记录该Game Object下的子Game Object的运动轨迹

    public string RecordDirectory;
    private string RecordName;

    private bool isRecord = false;

    private ZAZA_Controller zaza;

    // Start is called before the first frame update
    void Start()
    {
        zaza = new ZAZA_Controller();
        //记录路径
        RecordDirectory = (RecordDirectory.Length <= 0) ? Application.dataPath + "/MyRecord/" : RecordDirectory;

        StopRecord();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isRecord)
        {
            List<GameObject> listGameObjects = new List<GameObject>();
            for (int i = 0; i < goParent.transform.childCount; ++i)
            {
                listGameObjects.Add(goParent.transform.GetChild(i).gameObject);
            }

            StartCoroutine(zaza.SaveVideoStep(listGameObjects));
        }
    }

    public void OnClick()
    {
        if (!isRecord)
        {
            //创建记录文件
            RecordName = DateTime.Now.ToString("yyyy-MM-dd") + DateTime.Now.ToString("-hh-mm-ss") + ".zaza";

            zaza.SaveVideoBegin(RecordDirectory, RecordName);

            BeginRecord();
        }
        else
        {
            StopRecord();

            StartCoroutine(zaza.SaveVideoEnd());
        }
    }

    private void BeginRecord()
    {
        this.gameObject.GetComponentInChildren<Text>().text = "Stop Recording";
        isRecord = true;
    }

    private void StopRecord()
    {
        this.gameObject.GetComponentInChildren<Text>().text = "Record Motion";
        isRecord = false;
    }
}
