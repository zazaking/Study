using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public GameObject Text_CameraScene;
    enum SceneName
    {
        Scene3,
        SampleScene
    }

    // Start is called before the first frame update
    void Start()
    {
        //SceneManager.LoadScene("SampleScene");
        SceneManager.UnloadSceneAsync("Scene3");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void f_ChangeScene()
    {
        if (SceneManager.GetActiveScene().name == SceneName.Scene3.ToString()) //枚举类型保存String
        {
            SceneManager.LoadSceneAsync("SampleScene", LoadSceneMode.Single);
            SceneManager.UnloadSceneAsync("Scene3");
        }
        else if (SceneManager.GetActiveScene().name == "SampleScene")
        {
            SceneManager.LoadSceneAsync("Scene3", LoadSceneMode.Single);
            SceneManager.UnloadSceneAsync("SampleScene");
        }
    }
}
