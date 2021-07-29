using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleController : MonoBehaviour
{
    public GameObject SankeBody;
    //public GameObject SankeHead;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Åö×²
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Snake_Head")
        {
            SankeBody.GetComponent<SnakeController>().GetApple();
            
            int x = Random.Range(0, 19);
            int z = Random.Range(0, 19);
            transform.position = new Vector3(x+0.5f,0,z+0.5f);
        }
        else if(other.name == "Snake_Body")
        {
            int x = Random.Range(0, 19);
            int z = Random.Range(0, 19);
            transform.position = new Vector3(x + 0.5f, 0, z + 0.5f);
        }
    }
}
