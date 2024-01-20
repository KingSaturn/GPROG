using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public int health;
    [SerializeField] GameObject Textref;
    TextMeshProUGUI text;
    string healthText;

    public float speed = 8.0f;
    public float runspeed = 12.0f;
    public float jumpHeight = 8.0f;
    private float gravityValue = 20f;
    [SerializeField] Camera cam;
    public float cameraSpeed = 2.0f;
    public float lookXLimit = 35.0f;

    private CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;
    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        //HUD Text
        text = Textref.GetComponent<TextMeshProUGUI>();
        //lock cursor
        Cursor.lockState = CursorLockMode.Locked;
    }
    // Update is called once per frame
    void Update()
    {

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runspeed : speed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runspeed : speed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if(Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpHeight;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }
        if(!characterController.isGrounded)
        {
            moveDirection.y -= gravityValue * Time.deltaTime;
        }
        characterController.Move(moveDirection * Time.deltaTime);
        if(canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * cameraSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            cam.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * cameraSpeed, 0);
        }


        text.SetText(health.ToString());
        if (health <= 0)
        {
            health = 0;
            cameraSpeed = 0;
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}