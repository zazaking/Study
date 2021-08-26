using System;
using System.IO;
using UnityEngine;

public class ExportHTML : MonoBehaviour
{
    public GameObject gameObjectParent;
    public string fileDirectory;
    public string shotDirectory;
    public int shotMagnification;   //放大倍数
    public int lineDrawAccuracy;    //画线精度

    public Color lineColor;

    // Start is called before the first frame update
    void Start()
    {
        fileDirectory = Application.dataPath + "/MyHTML/";
        if (!Directory.Exists(fileDirectory))
        {
            Directory.CreateDirectory(fileDirectory);
        }

        shotDirectory = Application.dataPath + "/MyPhoto/";
        if (!Directory.Exists(shotDirectory))
        {
            Directory.CreateDirectory(shotDirectory);
        }

        shotMagnification = 2;
        lineDrawAccuracy = 8;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ExportGameObjectInfoToHTML()
    {
        if (gameObjectParent == null)
        {
            return;
        }

        string fileName = DateTime.Now.ToString("yyyy-MM-dd") + DateTime.Now.ToString("-hh-mm-ss") + ".html";
        string filePath = fileDirectory + fileName;

        StreamWriter streamWriter= new StreamWriter(filePath, false);
        
        streamWriter.Write("<!DOCTYPE html>\n<html>\n");
        streamWriter.Write(GetHtmlHead(fileName));
        streamWriter.Flush();

        streamWriter.Write("<body>\n");
        streamWriter.Write(GetHtmlGameObjectTable());
        streamWriter.Write("</body>\n");

        streamWriter.Write("</html>\n");
        streamWriter.Flush();
        streamWriter.Close();
    }

    private string GetHtmlHead(string _fileName)
    {
        string str = "<head>\n";
        str += "<meta charset=\"utf - 8\">\n";
        str += "<title>" +  _fileName + "</title>";
        str += "</head>\n";
        return str;
    }

    private string GetHtmlGameObjectTable()
    {
        if (gameObjectParent == null)
        {
            return "";
        }

        string str = "<div align=\"center\">\n<table border=\"1\">\n";
        str += "<tr>\n" +
            "<th> Object Name: </th>\n" +
            "<th> Picture: </th>\n" +
            "<th> Volume: </th>\n" +
            "<th> Area: </th>\n" +
            "</tr>\n";
        for (int i = 0; i < gameObjectParent.transform.childCount; ++i)
        {
            GameObject gameObject = gameObjectParent.transform.GetChild(i).gameObject;
            ShotGameObject(gameObject);

            str += "<tr>\n";
            str += "<td>" + gameObject.name.ToString() + "</td>\n";
            str += string.Format("<td> <img src =\" {0} \" /> </td>\n", "../MyPhoto/" + gameObject.name.ToString() + ".png");
            str += "<td>" + ShowGeometricInformation.GetVolume(gameObject).ToString() + "</td>\n";
            str += "<td>" + ShowGeometricInformation.GetArea(gameObject).ToString() + "</td>\n";
            str += "</tr>\n";
        }

        str += "</table>\n</div>\n";
        return str;
    }

    private void ShotGameObject(GameObject _gameObject)
    {
        string shotName = _gameObject.name.ToString() + ".png";
        string shotPath = shotDirectory + shotName;

        if (File.Exists(shotPath))
        {
            File.Delete(shotPath);
        }

        Vector3[] v_3D = _gameObject.GetComponent<MeshFilter>().mesh.vertices;
        int[] t = _gameObject.GetComponent<MeshFilter>().mesh.triangles;

        Vector2[] v_2D = new Vector2[v_3D.Length];

        for (int i = 0; i < v_3D.Length; ++i)
        {
            v_2D[i] = Camera.main.WorldToScreenPoint(v_3D[i]);
            v_2D[i].x *= shotMagnification;
            v_2D[i].y *= shotMagnification;
        }

        //获取图片大小
        float min_X = 0f;
        float max_X = 0f;
        float min_Y = 0f;
        float max_Y = 0f;
        for (int i = 0; i < v_2D.Length; ++i)
        {
            if(i == 0)
            {
                min_X = v_2D[i].x;
                max_X = v_2D[i].x;
                min_Y = v_2D[i].y;
                max_Y = v_2D[i].y;
            }
            else
            {
                if (v_2D[i].x < min_X)
                {
                    min_X = v_2D[i].x;
                }
                if (v_2D[i].x > max_X)
                {
                    max_X = v_2D[i].x;
                }
                if (v_2D[i].y < min_Y)
                {
                    min_Y = v_2D[i].y;
                }
                if (v_2D[i].y > max_Y)
                {
                    max_Y = v_2D[i].y;
                }
            }
        }
        int screenWidth = (int)Mathf.Abs(max_X - min_X);
        int screenHeight = (int)Mathf.Abs(max_Y - min_Y);

        Debug.Log("Image W="+ screenWidth + " H=" + screenHeight);

        Texture2D picture = new Texture2D(screenWidth, screenHeight);

        for (int i = 0; i < v_2D.Length; ++i)
        {
            v_2D[i].x = (int)(v_2D[i].x - min_X);
            v_2D[i].y = (int)(v_2D[i].y - min_Y);
        }

        for (int i = 0; i < t.Length; i += 3)
        {
            TextureDrawLine(v_2D[t[i]],v_2D[t[i + 1]], picture);
            TextureDrawLine(v_2D[t[i]], v_2D[t[i + 2]], picture);
            TextureDrawLine(v_2D[t[i + 1]], v_2D[t[i + 2]], picture);
        }

        picture.Apply();

        var bytes = picture.EncodeToJPG();
        File.WriteAllBytes(shotPath, bytes);
    }

    private void TextureDrawLine(Vector2 _begin,Vector2 _end, Texture2D _texture)
    {
        int x1 = (int)_begin.x;
        int x2 = (int)_end.x;
        int y1 = (int)_begin.y;
        int y2 = (int)_end.y;

        if (x1 == x2)
        {
            for (int i = Math.Min(y1, y2); i < Math.Max(y1, y2); ++i)
            {
                _texture.SetPixel(x1 , i , Color.black);
            }
        }
        else if (y1 == y2)
        {
            for (int i = Math.Min(x1, x2); i < Math.Max(x1, x2); ++i)
            {
                _texture.SetPixel(i, y1, Color.black);
            }
        }
        else
        {
            int x_step = x1 < x2 ? 1 : -1;
            int y_step = y1 < y2 ? 1 : -1;

            for (int i = x1; i != x2; i += x_step)
            {
                for (int j = y1; j != y2; j += y_step)
                {
                    if (i - x1 == 0)
                    {
                        continue;
                    }

                    if ( (int)( (float)(y2-y1)/(float)(x2-x1)* lineDrawAccuracy) == (int)( (float)(j-y1)/(float)(i-x1)* lineDrawAccuracy) )
                    {
                        _texture.SetPixel(i, j, Color.black);
                    }
                }
            }
        }

        _texture.SetPixel((int)_begin.x, (int)_begin.y, Color.black);
        _texture.SetPixel((int)_end.x, (int)_end.y, Color.black);
    }
}

