using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public int health;
    [SerializeField] GameObject Textref;
    TextMeshProUGUI text; 
    string healthText;
    public float speed = 10.0f;
    float cameraSpeed = 2.0f;
    [SerializeField] Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        text = Textref.GetComponent<TextMeshProUGUI>();
        Cursor.lockState = CursorLockMode.Locked;
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        float h = cameraSpeed * Input.GetAxis("Mouse X");
        float v = -cameraSpeed * Input.GetAxis("Mouse Y");
        if (Input.GetKey("w"))
        {
            pos += transform.forward * speed * Time.deltaTime;
        }
        if (Input.GetKey("s"))
        {
            pos -= transform.forward * speed * Time.deltaTime;
        }
        if (Input.GetKey("d"))
        {
            pos += transform.right * speed * Time.deltaTime;
        }
        if (Input.GetKey("a"))
        {
            pos -= transform.right * speed * Time.deltaTime;
        }
        text.SetText(health.ToString());
        transform.position = pos;
        transform.Rotate(0, h, 0);
        cam.transform.Rotate(v, 0, 0);
        if(health <= 0)
        {
            health = 0;
            cameraSpeed = 0;
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}
