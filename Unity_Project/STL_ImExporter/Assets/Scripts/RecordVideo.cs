using System.Collections;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEngine;
using System.Diagnostics;

public class RecordVideo : MonoBehaviour
{
    //截图材质参数
    [SerializeField]
    private Texture2D picture;
    private RenderTexture renderBuffer;

    //图片参数
    public string ShotPath;
    public string RecordPath;

    private int ScreenShotHeight;
    private int ScreenShotWith;
    
    private bool IsRecord = false;
    private static int RecordCount = 0;

    //FFMPEG参数
    private string ffmpegPath;

    void Start()
    {

        //视频的宽度必须是32的倍数，高度必须是2的倍数 不然FFMPEG图片转视频会出问题
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

        //利用FFMPEG将图片转为视频
        Process ffp = new Process();
        ffp.StartInfo.FileName = @ffmpegPath;     // 进程可执行文件位置
        ffp.StartInfo.Arguments = "-f image2 -i " + ShotPath + "%d.jpg -vcodec libx264 -r 25 " + RecordPath + VideoName;  // 传给可执行文件的命令行参数
        UnityEngine.Debug.Log(ffp.StartInfo.Arguments);
        ffp.StartInfo.CreateNoWindow = true;       // 不显示控制台窗口，需要和下一个参数一起才能完全隐藏窗口
        ffp.StartInfo.UseShellExecute = false;      // 不使用操作系统Shell程序启动进程
        ffp.StartInfo.RedirectStandardOutput = true;
        ffp.StartInfo.RedirectStandardError = true;
        ffp.StartInfo.RedirectStandardInput = true;//这句一定需要，用于模拟该进程控制台的输入
        ffp.Start();                                // 开始进程
        ffp.BeginErrorReadLine();   //开始异步读取
        ffp.WaitForExit();  //阻塞等待进程结束
        ffp.Close();        //关闭进程
        ffp.Dispose();      //释放资源

        RecordCount = 0;
        SetPath(ShotPath);
        UnityEngine.Debug.Log("Finished JPG To MP4");
    }

    public IEnumerator CaptureVideo()
    {
        while (IsRecord)
        {
            yield return new WaitForEndOfFrame();

            //视频的宽度必须是32的倍数，高度必须是2的倍数 不然FFMPEG图片转视频会出问题
            picture.ReadPixels(new Rect(0, 0, ScreenShotWith, ScreenShotHeight), 0, 0);
            picture.Apply();

            var bytes = picture.EncodeToJPG();
            File.WriteAllBytes(ShotPath + "/" + RecordCount.ToString() + ".jpg", bytes);

            RecordCount++;

            UnityEngine.Debug.Log("Fin the work CaptureVideo texture " + RecordCount);
        }
    }
}
