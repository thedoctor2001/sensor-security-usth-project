using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;

    private float _movementX;
    private float _movementY;

    private Rigidbody _rb;

    // Start is called before the first frame update
    private void Start()
    {
        Input.gyro.enabled = true;
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezePositionY;
    }
    
    private void FixedUpdate()
    {
#if UNITY_IOS || UNITY_ANDROID
        // Read the gyroscope rotation rate
        Vector3 gyroRotationRate = Input.gyro.rotationRateUnbiased;

        // Map the gyroscope rotation rate to movement values
        _movementX = gyroRotationRate.y; // Use y rotation rate for horizontal movement
        _movementY = -gyroRotationRate.x; // Use x rotation rate for forward/backward movement

        // Apply the movement along the x-axis and z-axis only, no y-axis movement
        Vector3 movement = new Vector3(_movementX, 0, _movementY);
        _rb.AddForce(movement * speed);
#else
        // Use desktop input
        var movement = new Vector3(_movementX, _movementY, 0);

        _rb.AddForce(movement * speed);
#endif

    }
    
#if !(UNITY_IOS || UNITY_ANDROID)
    // This method is only used for desktop input
    public void OnMove(InputValue movementValue)
    {
        var movementVector = movementValue.Get<Vector2>();

        _movementX = movementVector.x;
        _movementY = movementVector.y;
    }
#endif

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            EventManager.TriggerEvent("Player collided a pickup " + other.gameObject.GetHashCode());
        }
    }


}