using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }
    
    public float panSpeed = 100f;
    public float scrollSpeed = 10f;
    Camera cam;
    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        if (Input.GetKey("w")){
            pos.y += panSpeed * Time.deltaTime;
        };
        if (Input.GetKey("s"))
        {
            pos.y -= panSpeed * Time.deltaTime;
        };
        if (Input.GetKey("a"))
        {
            pos.x -= panSpeed * Time.deltaTime;
        };
        if (Input.GetKey("d"))
        {
            pos.x += panSpeed * Time.deltaTime;
        };
        if (Input.GetKey("e"))
        {
            cam.orthographicSize -= 1.0f;
        };
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            cam.orthographicSize -= scrollSpeed;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
        {
            cam.orthographicSize += scrollSpeed;
        }
        //pos.x = Mathf.Clamp(pos.x, -1000,1000)
        transform.position = pos;
    }
}
