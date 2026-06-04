using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _scale;
    [SerializeField] private float _rotationSensivity;
    [SerializeField] private float _minRotationX = -45f;
    [SerializeField] private float _maxRotationX = 45f;
    public string Brand;
    private Transform myTransform;
    public float RotationY;
    public float InputVertical, InputHorizontal, MouseY, MouseX;

    private float currentRotationX;
    private float currentRotationY;

    public Transform MainCameraTransform;

    private void Awake()
    {
        // Debug.Log("Awake");
        Inisialisasi();
    }
    private void Inisialisasi()
    {
        myTransform = GetComponent<Transform>(); //mencri component pada game object yang sama
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Debug.Log("Start");
        // Debug.Log($"Mobil brand {Brand} memiliki speed {_moveSpeed} ");
        Move();
        Rotate();
        Scale();
    }
    private void OnEnable()
    {
        // Debug.Log("OnEnable");
    }
    private void OnDisable()
    {
        // Debug.Log("OnDisable");
    }


    public void Move()
    {
        // Debug.Log($"Mobil brand {Brand} on start pada posisi {myTransform.position} ");
        myTransform.position += (myTransform.forward * _moveSpeed * InputVertical * Time.deltaTime);
        myTransform.position += (myTransform.right * _moveSpeed * InputHorizontal * Time.deltaTime);

        // Debug.Log($"Mobil brand {Brand} setelah bergerak ada pada posisi {myTransform.position} ");
    }

    public void Rotate()
    {
        // Debug.Log($"Mobil brand {Brand} on start pada rotasi {myTransform.localEulerAngles} ");

        currentRotationY += MouseX* _rotationSensivity;
        currentRotationX -= MouseY* _rotationSensivity;

        currentRotationX = Mathf.Clamp(currentRotationX, _minRotationX, _maxRotationX);
        
        myTransform.localRotation = Quaternion.Euler(0f, currentRotationY, 0f);

        MainCameraTransform.localRotation = Quaternion.Euler(currentRotationX, 0f, 0f);
        // Debug.Log($"Mobil brand {Brand} setelah bergerak pada rotasi {myTransform.rotation} ");
    }

    public void Scale()
    {
        // Debug.Log($"Mobil brand {Brand} on start pada scale {myTransform.localScale} ");
        myTransform.localScale += (Vector3.one * _scale);
        // Debug.Log($"Mobil brand {Brand} setelah bergerak pada scale {myTransform.localScale} ");
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("Update");
        GetInput();
        Move();
        Rotate();
        // Scale();
    }

    public void GetInput()
    {
        InputVertical = Input.GetAxis("Vertical");
        InputHorizontal = Input.GetAxis("Horizontal");

        MouseX = Input.GetAxis("Mouse X");
        MouseY = Input.GetAxis("Mouse Y");
    }

    private void FixedUpdate()
    {
        // Debug.Log("FixedUpdate");
    }
     private void OnDestroy()
     {
        // Debug.Log("OnDestroy");
     }
     public void AddPoint(int amount)
    {
        TotalPoints += amount;
        Debug.Log("Poin Bertambah! Total Poin Sekarang: " + TotalPoints);
    }
}
