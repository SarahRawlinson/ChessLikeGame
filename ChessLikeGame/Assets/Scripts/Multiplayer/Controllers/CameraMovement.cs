using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float minDistanceFromTarget = 5f;
    [SerializeField] private float maxDistanceFromTarget = 15f;
    [SerializeField] private float minPitchAngle = 10f;
    [SerializeField] private float maxPitchAngle = 80f;
    [SerializeField] private GameObject target;

    private Vector3 currentRotation;
    private float currentDistanceFromTarget;
    private Quaternion initialRotation;

    private void Start()
    {
        currentRotation = transform.eulerAngles;
        currentDistanceFromTarget = Vector3.Distance(transform.position, target.transform.position);
        initialRotation = transform.rotation;
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        // Rotation input from keyboard
        if (Input.GetKey(KeyCode.W))
            currentRotation.x -= rotationSpeed * deltaTime;

        if (Input.GetKey(KeyCode.S))
            currentRotation.x += rotationSpeed * deltaTime;

        if (Input.GetKey(KeyCode.A))
            currentRotation.y -= rotationSpeed * deltaTime;

        if (Input.GetKey(KeyCode.D))
            currentRotation.y += rotationSpeed * deltaTime;

        // Rotation input from joystick
        float joystickPitch = Input.GetAxis("Vertical");
        float joystickYaw = Input.GetAxis("Horizontal");

        currentRotation.x -= joystickPitch * rotationSpeed * deltaTime;
        currentRotation.y += joystickYaw * rotationSpeed * deltaTime;

        // Clamp pitch rotation
        currentRotation.x = Mathf.Clamp(currentRotation.x, minPitchAngle, maxPitchAngle);

        // Zoom in and out with Q and E keys
        if (Input.GetKey(KeyCode.Q))
            currentDistanceFromTarget -= zoomSpeed * deltaTime;

        if (Input.GetKey(KeyCode.E))
            currentDistanceFromTarget += zoomSpeed * deltaTime;

        // Clamp the distance from the target
        currentDistanceFromTarget = Mathf.Clamp(currentDistanceFromTarget, minDistanceFromTarget, maxDistanceFromTarget);

        // Apply rotation and calculate new position
        Quaternion rotation = Quaternion.Euler(currentRotation);
        Vector3 direction = rotation * Vector3.back;
        transform.position = target.transform.position + direction * currentDistanceFromTarget;

        // Look at the target
        transform.LookAt(target.transform);

        // Reset rotation
        if (Input.GetKeyDown(KeyCode.R) || Input.GetButtonDown("Fire3")) // Assuming "Fire3" is the button to reset on the joystick
        {
            transform.rotation = initialRotation;
            currentRotation = initialRotation.eulerAngles;
        }

        // Flip camera to the opposite side
        if (Input.GetKeyDown(KeyCode.F))
        {
            currentRotation.y += 180f;
        }
    }
}
