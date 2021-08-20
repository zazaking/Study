using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using System.IO;
using System;


namespace VideoZAZA
{
    //记录文件格式类
    public class typeZAZA
    {
        public int index;
        public int objCount;
        public List<string> listObjName;
        public List<Vector3> listObjPosition;
        public List<Vector3> listObjEulerAngles;

        public typeZAZA()
        {
            index = 0;
            objCount = 0;
            listObjName = new List<string>();
            listObjPosition = new List<Vector3>();
            listObjEulerAngles = new List<Vector3>();
        }

        public typeZAZA(typeZAZA other)
        {
            this.index = other.index;
            this.objCount = other.objCount;
            this.listObjName = new List<string>(other.listObjName);
            this.listObjPosition = new List<Vector3>(other.listObjPosition);
            this.listObjEulerAngles = new List<Vector3>(other.listObjEulerAngles);
        }

        public void clear()
        {
            index = 0;
            objCount = 0;
            listObjName.Clear();
            listObjPosition.Clear();
            listObjEulerAngles.Clear();
        }
    }

    //记录文件控制类
    public class ZAZA_Controller
    {
        public enum state{
            isIdle = 0,
            isLoad = 1,
            isPlay = 2,
            isSave = 3
        };  //状态信息

        private int mState; //状态
        private int mSaveStepIndex; //写文件序列号
        
        private string saveFileDiectory;    //保存文件文件夹Path
        private string loadFileDiectory;    //保存文件文件夹Path

        private List<typeZAZA> listStep;    //记录信息列表

        private StreamWriter streamWriter; 
        private StreamReader streamReader; 

        public ZAZA_Controller()
        {
            listStep = new List<typeZAZA>();
            mState = (int)state.isIdle;
            mSaveStepIndex = 0;
        }

        public int GetState()
        {
            return mState;
        }

        public bool isPlay()
        {
            return mState == (int)state.isPlay;
        }

        //加载视频
        public void LoadVideoFile(string _VideoFilePath)
        {
            if (mState != (int)state.isIdle)
                return;

            if (_VideoFilePath.Length <= 0)
                return;

            mState = (int)state.isLoad;
            loadFileDiectory = _VideoFilePath.Substring(0, _VideoFilePath.LastIndexOf("/")+1);

            listStep.Clear();
            
            //获取文件流
            typeZAZA tempStep = new typeZAZA();
            streamReader = new StreamReader(_VideoFilePath);
            
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                if (line.StartsWith("Record Begin\tCount:"))
                {
                    tempStep.index = Convert.ToInt32(line.Replace("Record Begin\tCount:", ""));
                }
                else if (line.StartsWith("\tGameObject:"))
                {
                    tempStep.listObjName.Add(line.Replace("\tGameObject:", ""));
                }
                else if (line.StartsWith("\t\tPosition:"))
                {
                    string[] split = line.Replace("\t\tPosition:", "").Split(',');
                    Vector3 v = new Vector3();
                    float.TryParse(split[0], out v.x);
                    float.TryParse(split[1], out v.y);
                    float.TryParse(split[2], out v.z);

                    tempStep.listObjPosition.Add(v);
                }
                else if (line.StartsWith("\t\tEulerAngles:"))
                {
                    string[] split = line.Replace("\t\tEulerAngles:", "").Split(',');
                    Vector3 v = new Vector3();
                    float.TryParse(split[0], out v.x);
                    float.TryParse(split[1], out v.y);
                    float.TryParse(split[2], out v.z);

                    tempStep.listObjEulerAngles.Add(v);
                }
                else if (line.StartsWith("Record End"))
                {
                    tempStep.objCount = tempStep.listObjName.Count;
                    listStep.Add(new typeZAZA(tempStep));
                    tempStep.clear();
                }
            }

            streamReader.Close();

            mState = (int)state.isPlay;
        }

        public void PlayOneStep(GameObject StlGoParent, int _stepIndex)
        {
            if (mState != (int)state.isPlay)
                return;

            if ( _stepIndex >= listStep.Count || _stepIndex < 0)
            {
                mState = (int)state.isIdle;
                return;
            }
                

            typeZAZA tempStep = new typeZAZA(listStep[_stepIndex]);

            //循环查找goParent下的对象是否都存在
            for (int i = 0; i < tempStep.objCount; ++i)
            {
                GameObject gameObject;

                //导入未导入的对象
                if (i >= StlGoParent.transform.childCount)
                {
                    string modelName = tempStep.listObjName[i];
                    string modelPath = loadFileDiectory + modelName + ".stl";
                    
                    gameObject = LoadSTLModel(modelPath);
                    gameObject.transform.SetParent(StlGoParent.transform);
                }
                else
                {
                    gameObject = StlGoParent.transform.GetChild(i).gameObject;
                }

                gameObject.transform.position = tempStep.listObjPosition[i];
                gameObject.transform.rotation = Quaternion.Euler(tempStep.listObjEulerAngles[i]);
            }
        }

        public void SaveVideoBegin(string _saveFileDiectory, string _saveFileName)
        {
            if (mState != (int)state.isIdle)
                return;

            mState = (int)state.isSave;
            mSaveStepIndex = 0;
            saveFileDiectory = _saveFileDiectory;

            if (!Directory.Exists(_saveFileDiectory))
                Directory.CreateDirectory(_saveFileDiectory);

            string saveFilePath = _saveFileDiectory + _saveFileName;

            streamWriter = new StreamWriter(saveFilePath, true);
        }

        public IEnumerator SaveVideoEnd()
        {
            yield return null;

            if (mState == (int)state.isSave)
            {
                streamWriter.Close();
                mState = (int)state.isIdle;
            }
        }

        public IEnumerator SaveVideoStep(List<GameObject> _listGameObject)
        {
            yield return null;

            if (mState == (int)state.isSave)
            {
                streamWriter.WriteLine(string.Format("Record Begin\tCount:{0}", mSaveStepIndex));

                for (int i = 0; i < _listGameObject.Count; ++i)
                {
                    GameObject go = _listGameObject[i];

                    //检测模型文件是否存在
                    string stlModelPath = saveFileDiectory + go.name.ToString() + ".stl";
                    if (!File.Exists(stlModelPath))
                    {
                        SaveSTLModel(stlModelPath, go);     //模型不存在则创建模型
                        Debug.Log("Create STL Model:" + stlModelPath);
                    }

                    streamWriter.WriteLine(string.Format("\tGameObject:{0}", go.name.ToString()));
                    streamWriter.WriteLine(
                        string.Format("\t\tPosition:{0},{1},{2}",
                            go.transform.position.x,
                            go.transform.position.y,
                            go.transform.position.z
                            )
                        );
                    streamWriter.WriteLine(
                        string.Format("\t\tEulerAngles:{0},{1},{2}",
                            go.transform.eulerAngles.x,
                            go.transform.eulerAngles.y,
                            go.transform.eulerAngles.z
                            )
                        );
                }
                streamWriter.WriteLine("Record End");
                streamWriter.Flush();

                ++mSaveStepIndex;
            }
        }

        private GameObject LoadSTLModel(string _filePath)
        {
            //获取文件名
            string fileName = _filePath.Substring(_filePath.LastIndexOf("/") + 1, _filePath.LastIndexOf(".") - (_filePath.LastIndexOf("/") + 1));
            fileName = fileName.Length == 0 ? "StlGameObject" : fileName;

            //获取文件流
            StreamReader sr = new StreamReader(_filePath);

            //mesh信息列标
            List<Vector3> listVertices = new List<Vector3>();   //网格顶点数组
            List<Vector3> listNormals = new List<Vector3>();    //网格的法线数组

            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (line.StartsWith("solid"))
                {

                }
                else if (line.StartsWith("facet"))
                {
                    Vector3 normal = StringToVec3(line.Replace("facet normal ", ""));
                    listNormals.Add(normal);
                    listNormals.Add(normal);
                    listNormals.Add(normal);
                }
                else if (line.StartsWith("outer loop"))
                {
                }
                else if (line.StartsWith("endloop"))
                {

                }
                else if (line.StartsWith("endfacet"))
                {

                }
                else if (line.StartsWith("	vertex"))
                {
                    Vector3 vertex = StringToVec3(line.Replace("vertex ", ""));
                    listVertices.Add(vertex);
                }
            }

            //STL根据stl创建GameObject
            GameObject gameObject = new GameObject();
            gameObject.name = fileName;
            gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<MeshRenderer>();

            int len = listNormals.Count;
            Vector3[] v = new Vector3[len];
            Vector3[] n = new Vector3[len];
            int[] t = new int[len];

            for (int i = 0; i < len; ++i)
            {
                v[i] = listVertices[i];
                n[i] = listNormals[i];
                t[i] = i;
            }

            Mesh mesh = new Mesh
            {
                vertices = v,
                normals = n,
                triangles = t
            };

            gameObject.GetComponent<MeshFilter>().mesh = mesh;

            //添加材质
            string[] KeyWords = {
            "_NORMALMAP" ,
            "_ALPHATEST_ON" ,
            "_ALPHABLEND_ON",
            "_ALPHAPREMULTIPLY_ON" ,
            "_EMISSION",
            "_PARALLAXMAP" ,
            "_DETAIL_MULX2" ,
            //"_METALLICGLOSSMAP",
            "_SPECGLOSSMAP"
            };
            foreach (string keyword in KeyWords)
            {
                gameObject.gameObject.GetComponent<Renderer>().material.EnableKeyword(keyword);
            }


            return gameObject;
        }

        private void SaveSTLModel(string filePath,GameObject gameObject)
        {
            //生成导出模型信息
            string strSaveName = gameObject.name;
            Vector3[] v = gameObject.GetComponent<MeshFilter>().mesh.vertices;
            Vector3[] n = gameObject.GetComponent<MeshFilter>().mesh.normals;
            int[] t = gameObject.GetComponent<MeshFilter>().mesh.triangles;

            //创建文件流
            StreamWriter stlSW = new StreamWriter(filePath,false);

            //写文件
            stlSW.WriteLine(string.Format("solid {0}", strSaveName));

            int triLen = t.Length;
            for (int i = 0; i < triLen; i += 3)
            {
                int a = t[i];
                int b = t[i + 1];
                int c = t[i + 2];

                Vector3 nrm = AvgNrm(n[a], n[b], n[c]);

                stlSW.WriteLine(string.Format("facet normal {0} {1} {2}", nrm.x, nrm.y, nrm.z));

                stlSW.WriteLine("outer loop");

                stlSW.WriteLine(string.Format("\tvertex {0} {1} {2}", v[a].x, v[a].y, v[a].z));
                stlSW.WriteLine(string.Format("\tvertex {0} {1} {2}", v[b].x, v[b].y, v[b].z));
                stlSW.WriteLine(string.Format("\tvertex {0} {1} {2}", v[c].x, v[c].y, v[c].z));

                stlSW.WriteLine("endloop");
                stlSW.WriteLine("endfacet");
                stlSW.Flush();
            }

            stlSW.WriteLine(string.Format("endsolid {0}", strSaveName));

            stlSW.Close();
        }

        public void DebugPrintLoadVideoInfo()
        {
            foreach (var tempStep in listStep)
            {
                Debug.Log("********************************************");
                Debug.Log(tempStep.index.ToString());
                for (int i = 0; i < tempStep.objCount; ++i)
                {
                    Debug.Log(
                        string.Format(
                            "\t{0}:[{1},{2},{3}]\t[{4},{5},{6}]",
                            tempStep.listObjName[i],
                            tempStep.listObjPosition[i].x,
                            tempStep.listObjPosition[i].y,
                            tempStep.listObjPosition[i].z,
                            tempStep.listObjEulerAngles[i].x,
                            tempStep.listObjEulerAngles[i].y,
                            tempStep.listObjEulerAngles[i].z
                            )
                        );
                }
            }
        }

        private static Vector3 AvgNrm(Vector3 a, Vector3 b, Vector3 c)
        {
            return new Vector3(
                (a.x + b.x + c.x) / 3f,
                (a.y + b.y + c.y) / 3f,
                (a.z + b.z + c.z) / 3f);
        }

        static Vector3 StringToVec3(string str)
        {
            string[] split = str.Trim().Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            Vector3 v = new Vector3();

            float.TryParse(split[0], out v.x);
            float.TryParse(split[1], out v.y);
            float.TryParse(split[2], out v.z);

            return v;
        }
    }
}
