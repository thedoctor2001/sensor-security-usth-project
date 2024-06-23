using UnityEngine;

public class PickupController : MonoBehaviour
{
    [SerializeField] private CommonConfigurations configurations;

    private void Start()
    {
        EventManager.StartListening("Player collided a pickup " + gameObject.GetHashCode(), TriggerIncreaseScore);
    }

    // Update is called once per frame
    private void Update()
    {
        transform.Rotate(configurations.rotation * Time.deltaTime);
    }

    private void TriggerIncreaseScore()
    {
        EventManager.TriggerEvent("Increase score");
    }
}