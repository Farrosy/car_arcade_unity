using UnityEngine;

public class CoinController : MonoBehaviour
{
    [SerializeField] private int _pointValue = 1;
    [SerializeField] private float _rotationSpeed = 100f; 

    void Update()
    {
        transform.Rotate(Vector3.up * _rotationSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        NewMonoBehaviourScript player = other.GetComponent<NewMonoBehaviourScript>();

        if (player != null)
        {
            player.AddPoint(_pointValue);

            Destroy(gameObject);
        }
    }
}