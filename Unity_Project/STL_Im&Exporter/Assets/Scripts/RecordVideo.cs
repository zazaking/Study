using System.Collections;
using System.IO;
using UnityEngine.UI;
using UnityEngine;

using System;
using System.Diagnostics;
using System.Web;



public class RecordVideo : MonoBehaviour
{
    public float ScreenShotHeight = 500;
    public float ScreenShotWith = 500;
    public string PicSavePath = "MyPhoto";


    [SerializeField]
    private Texture2D picture;
    private RenderTexture renderBuffer;
    private string Path = "";
    private bool IsShot = false;
    private static int count = 0;

    public string RecordPath = "";
    private bool IsRecord = false;
    private static int RecordCount = 0;

    // Use this for initialization
    void Start()
    {
        //≥ı ºªØ
        picture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false, true);
        Path = Application.dataPath + "/" + PicSavePath;
        
        SetPath(Path);
        RecordPath = "I:/FFOutput";
        SetPath(RecordPath);

        this.gameObject.GetComponentInChildren<Text>().text = "Start Record";
        IsRecord = false;
    }

    public void SetPath(string _s)
    {
        //path
        if (Directory.Exists(_s))
        {
            FileAttributes attr = File.GetAttributes(_s);
            if (attr == FileAttributes.Directory)
                Directory.Delete(_s, true);
            else
                File.Delete(_s);
        }
        if (!Directory.Exists(_s))
            Directory.CreateDirectory(_s);
    }

    // Update is called once per frame
    public void OnClick()
    {
        if (IsRecord)
        {
            StopRecord();
            this.gameObject.GetComponentInChildren<Text>().text = "Start Record";
        }
        else
        {
            Record();
            this.gameObject.GetComponentInChildren<Text>().text = "Stop Recording";
        }
        //if (Input.GetKeyDown(KeyCode.Space) && !IsShot)
        //    ShotScreen();
        //if (Input.GetKeyDown(KeyCode.R))
        //    Record();
        //if (Input.GetKeyDown(KeyCode.S))
        //    StopRecord();
    }

    public bool StartRecord()
    {
        IsRecord = true;
        return IsRecord;
    }
    public bool StopRecord()
    {
        IsRecord = false;
        return IsRecord;
    }

    public void Record()
    {
        StartRecord();
        StartCoroutine(CaptureVideo());
    }

    public void ShotScreen()
    {
        IsShot = true;
        StartCoroutine(CaptureScreen());
    }


    public IEnumerator CaptureScreen()
    {
        yield return new WaitForEndOfFrame();
        picture.ReadPixels(
            new Rect(0, 0, Screen.width, Screen.height), 0, 0
            );

        picture.Apply();
        var bytes = picture.EncodeToPNG();
        File.WriteAllBytes(Path + "/" + count.ToString() + ".png", bytes);
        count++;
        UnityEngine.Debug.Log("Fin the work gender texture");

        yield return new WaitForSeconds(0.1f);
        IsShot = false;
    }

    public IEnumerator CaptureVideo()
    {
        while (IsRecord)
        {
            picture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            picture.Apply();
            var bytes = picture.EncodeToPNG();
            File.WriteAllBytes(Path + "/" + RecordCount.ToString() + ".png", bytes);
            RecordCount++;
            UnityEngine.Debug.Log("Fin the work CaptureVideo texture " + RecordCount);
            yield return new WaitForEndOfFrame();
        }

    }
}
