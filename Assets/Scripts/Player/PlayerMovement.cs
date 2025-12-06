using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 6f;
    public float gravity = -15f;
    public float jumpHeight = 2f;
    public Transform cameraTransform;

    [Header("Crouch Settings")]
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float crouchSpeed = 3f;
    [SerializeField] private float standingSpeed = 6f;
    [SerializeField] private float crouchCameraY = 0.7f;
    [SerializeField] private float standingCameraY = 1.6f;
    [SerializeField] private float crouchTransitionSpeed = 8f;

    private bool isCrouching = false;

    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        //Cursor.lockState = CursorLockMode.Locked;

        Debug.Log("Current Time Scale: " + Time.timeScale); 
        //Time.timeScale = 1f; // Force the game to run at normal speed
    }

    private void StartCrouch()
    {
        isCrouching = true;
    }

    private void StopCrouch()
    {
        isCrouching = false;
    }


    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        if (controller.isGrounded)
        {
            if (velocity.y < 0)
                velocity.y = -2f; // keep player stuck to the ground

            if (Input.GetButtonDown("Jump"))
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Crouch toggle/hold
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartCrouch();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            StopCrouch();
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Smooth height transition
        float targetHeight = isCrouching ? crouchHeight : standingHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);

        // Smooth camera height offset
        float targetCameraY = isCrouching ? crouchCameraY : standingCameraY;
        Vector3 camPos = cameraTransform.localPosition;
        camPos.y = Mathf.Lerp(camPos.y, targetCameraY, Time.deltaTime * crouchTransitionSpeed);
        cameraTransform.localPosition = camPos;

        // Variable movement speed based on crouch
        float targetSpeed = isCrouching ? crouchSpeed : standingSpeed;
        speed = Mathf.Lerp(speed, targetSpeed, Time.deltaTime * 10f);

    }
}
