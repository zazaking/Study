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
    public GameObject goParent;     //��¼��Game Object�µ���Game Object���˶��켣

    public string RecordDirectory;
    private string RecordName;

    private bool isRecord = false;

    private ZAZA_Controller zaza;

    // Start is called before the first frame update
    void Start()
    {
        zaza = new ZAZA_Controller();
        //��¼·��
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
            //������¼�ļ�
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
