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
    public float speed = 10.0f;
    float cameraSpeed = 2.0f;
    [SerializeField] Camera cam;
    private CharacterController characterController;
    private bool groundedPlayer;
    private Vector3 playerVelocity;
    public float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;
    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        text = Textref.GetComponent<TextMeshProUGUI>();
        Cursor.lockState = CursorLockMode.Locked;
    }
    // Update is called once per frame
    void Update()
    {

        groundedPlayer = characterController.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector3 pos = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        float h = cameraSpeed * Input.GetAxis("Mouse X");
        float v = -cameraSpeed * Input.GetAxis("Mouse Y");

        characterController.Move(pos * Time.deltaTime * speed);

        if (pos != Vector3.zero)
        {
            transform.forward = pos;
        }
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }
        playerVelocity.y += gravityValue * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime);
        text.SetText(health.ToString());
        transform.Rotate(0, h, 0);
        cam.transform.Rotate(v, 0, 0);
        if (health <= 0)
        {
            health = 0;
            cameraSpeed = 0;
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}