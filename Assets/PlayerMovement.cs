using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed = 2.5f;
    public float runSpeed = 5f;

    [Header("Mouse")]
    public float mouseSensitivity = 200f;

    [Header("Gravedad")]
    public float gravity = -9.81f;

    [Header("Camara")]
    public Transform cameraPivot;

    [Header("Head Bob")]
    public float bobSpeed = 7f;
    public float bobAmount = 0.03f;

    CharacterController controller;

    float xRotation = 0f;
    float verticalVelocity;

    Vector3 originalCamPos;
    float bobTimer;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        originalCamPos = cameraPivot.localPosition;

        Cursor.lockState = CursorLockMode.Locked;
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        // MOUSE
        float mouseX =
            Input.GetAxis("Mouse X")
            * mouseSensitivity
            * Time.deltaTime;

        float mouseY =
            Input.GetAxis("Mouse Y")
            * mouseSensitivity
            * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraPivot.localRotation =
            Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);

        // CAMINAR / CORRER
        float currentSpeed = walkSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = runSpeed;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move =
            transform.right * x +
            transform.forward * z;

        // GRAVEDAD
        if (controller.isGrounded &&
            verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity +=
            gravity * Time.deltaTime;

        move.y = verticalVelocity;

        controller.Move(
            move *
            currentSpeed *
            Time.deltaTime
        );

        // HEAD BOB
        Vector3 horizontalVelocity =
            controller.velocity;

        horizontalVelocity.y = 0;

        if (horizontalVelocity.magnitude > 0.1f &&
            controller.isGrounded)
        {
            bobTimer +=
                Time.deltaTime *
                bobSpeed *
                (currentSpeed / walkSpeed);

            cameraPivot.localPosition =
                originalCamPos +
                new Vector3(
                    0,
                    Mathf.Sin(bobTimer)
                    * bobAmount,
                    0
                );
        }
        else
        {
            bobTimer = 0;

            cameraPivot.localPosition =
                Vector3.Lerp(
                    cameraPivot.localPosition,
                    originalCamPos,
                    Time.deltaTime * 8f
                );
        }
    }
}