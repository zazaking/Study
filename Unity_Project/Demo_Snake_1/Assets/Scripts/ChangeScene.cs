using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    private string sceneName;
    public GameObject Text_CameraScene;

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

        if(SceneManager.GetActiveScene().name == "Scene3")
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
