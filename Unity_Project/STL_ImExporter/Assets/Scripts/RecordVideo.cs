using System.Collections;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEngine;
using System.Diagnostics;

public class RecordVideo : MonoBehaviour
{
    //��ͼ���ʲ���
    [SerializeField]
    private Texture2D picture;
    private RenderTexture renderBuffer;

    //ͼƬ����
    public string ShotPath;
    public string RecordPath;

    private int ScreenShotHeight;
    private int ScreenShotWith;
    
    private bool IsRecord = false;
    private static int RecordCount = 0;

    //FFMPEG����
    private string ffmpegPath;

    void Start()
    {

        //��Ƶ�Ŀ�ȱ�����32�ı������߶ȱ�����2�ı��� ��ȻFFMPEGͼƬת��Ƶ�������
        ScreenShotWith = Screen.width;
        if (ScreenShotWith % 32 != 0)
        {
            ScreenShotWith = ScreenShotWith / 32 * 32;
        }

        ScreenShotHeight = Screen.height;
        if (ScreenShotHeight % 2 != 0)
        {
            ScreenShotHeight = ScreenShotHeight / 2 * 2;
        }
        picture = new Texture2D(ScreenShotWith, ScreenShotHeight, TextureFormat.RGBA32, false, true);

        ffmpegPath = Application.streamingAssetsPath + "/FFMPEG/ffmpeg.exe";

        ShotPath = (ShotPath.Length <= 0) ? Application.dataPath + "/MyPhoto/" : ShotPath;
        RecordPath = (RecordPath.Length <= 0) ? Application.dataPath + "/MyVideo/" : RecordPath;

        SetPath(ShotPath);
        SetPath(RecordPath);

        IsRecord = false;

        this.gameObject.GetComponentInChildren<Text>().text = "Start Record";
    }

    public void SetPath(string _s)
    {
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

    //
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
    }

    public void Record()
    {
        StartRecord();
        StartCoroutine(CaptureVideo());
    }

    public bool StartRecord()
    {
        IsRecord = true;
        return IsRecord;
    }

    public bool StopRecord()
    {
        StartCoroutine(PicToVideo());
        IsRecord = false;
        return IsRecord;
    }

    public IEnumerator PicToVideo()
    {
        yield return null;

        string VideoName = DateTime.Now.ToString("yyyy-MM-dd") + DateTime.Now.ToString("-hh-mm-ss") + ".mp4";

        //����FFMPEG��ͼƬתΪ��Ƶ
        Process ffp = new Process();
        ffp.StartInfo.FileName = @ffmpegPath;     // ���̿�ִ���ļ�λ��
        ffp.StartInfo.Arguments = "-f image2 -i " + ShotPath + "%d.jpg -vcodec libx264 -r 25 " + RecordPath + VideoName;  // ������ִ���ļ��������в���
        UnityEngine.Debug.Log(ffp.StartInfo.Arguments);
        ffp.StartInfo.CreateNoWindow = true;       // ����ʾ����̨���ڣ���Ҫ����һ������һ�������ȫ���ش���
        ffp.StartInfo.UseShellExecute = false;      // ��ʹ�ò���ϵͳShell������������
        ffp.StartInfo.RedirectStandardOutput = true;
        ffp.StartInfo.RedirectStandardError = true;
        ffp.StartInfo.RedirectStandardInput = true;//���һ����Ҫ������ģ��ý��̿���̨������
        ffp.Start();                                // ��ʼ����
        ffp.BeginErrorReadLine();   //��ʼ�첽��ȡ
        ffp.WaitForExit();  //�����ȴ����̽���
        ffp.Close();        //�رս���
        ffp.Dispose();      //�ͷ���Դ

        RecordCount = 0;
        SetPath(ShotPath);
        UnityEngine.Debug.Log("Finished JPG To MP4");
    }

    public IEnumerator CaptureVideo()
    {
        while (IsRecord)
        {
            yield return new WaitForEndOfFrame();

            //��Ƶ�Ŀ�ȱ�����32�ı������߶ȱ�����2�ı��� ��ȻFFMPEGͼƬת��Ƶ�������
            picture.ReadPixels(new Rect(0, 0, ScreenShotWith, ScreenShotHeight), 0, 0);
            picture.Apply();

            var bytes = picture.EncodeToJPG();
            File.WriteAllBytes(ShotPath + "/" + RecordCount.ToString() + ".jpg", bytes);

            RecordCount++;

            UnityEngine.Debug.Log("Fin the work CaptureVideo texture " + RecordCount);
        }
    }
}
