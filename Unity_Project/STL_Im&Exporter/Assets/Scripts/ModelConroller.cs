using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelConroller : MonoBehaviour
{
    private Vector3 MoveUP = new Vector3(0, 1, 0);
    private Vector3 MoveDOWN = new Vector3(0, -1, 0);
    private Vector3 MoveLEFT = new Vector3(-1, 0, 0);
    private Vector3 MoveRIGHT = new Vector3(1, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //¼üÅÌÆ½ÒÆ¿ØÖÆ
        if (Input.GetKeyDown(KeyCode.W))
        {
            this.transform.position += MoveUP;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            this.transform.position += MoveDOWN;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            this.transform.position += MoveLEFT;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            this.transform.position += MoveRIGHT;
        }

        float moveSpeed = 0;
        if (Input.GetMouseButton(0))
        {
            moveSpeed = 300f;
        }
        else
        {
            moveSpeed = 0f;
        }
        transform.Rotate(0, moveSpeed * Time.deltaTime, 0);

        //¹öÂÖËõ·Å
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (Camera.main.fieldOfView <= 100)
                Camera.main.fieldOfView += 2;
            if (Camera.main.orthographicSize <= 20)
                Camera.main.orthographicSize += 0.5F;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (Camera.main.fieldOfView > 2)
                Camera.main.fieldOfView -= 2;
            if (Camera.main.orthographicSize >= 1)
                Camera.main.orthographicSize -= 0.5F;
        }
    }
}
