using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CubeController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 15f;
    [SerializeField] private float _turnSpeed = 100f; // Kecepatan belok mobil
    [SerializeField] private float _scale;
    
    // Kita pisahkan sensitivitas mouse jika kamu ingin kamera tetap bisa menengadah/menunduk
    [Header("Camera Look Settings")]
    [SerializeField] private float _mouseSensivity = 2f;
    [SerializeField] private float _minRotationX = -30f;
    [SerializeField] private float _maxRotationX = 30f;
    
    public string Brand;
    private Transform myTransform;
    private Rigidbody myRigidbody;
    
    // Input variables
    public float InputVertical, InputHorizontal, MouseY, MouseX;

    private float currentRotationX;
    private float currentRotationY;

    public Transform MainCameraTransform;
    
    [Header("Score System")]
    public int TotalPoints = 0;

    private void Awake()
    {
        Inisialisasi();
    }

    private void Inisialisasi()
    {
        myTransform = GetComponent<Transform>();
        myRigidbody = GetComponent<Rigidbody>();

        // Mengoptimalkan Rigidbody untuk pergerakan halus
        myRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        myRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        
        // Mengunci rotasi Fisika agar mobil tidak guling saat menabrak
        myRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        currentRotationY = myTransform.eulerAngles.y;
    }

    void Start()
    {
        Scale();
    }

    void Update()
    {
        GetInput();
        
        // Kalkulasi rotasi kamera (melihat atas/bawah) dilakukan di Update agar responsif
        RotateCamera(); 
    }

    private void FixedUpdate()
    {
        // Kontrol fisik dijalankan di FixedUpdate
        Move();
        Turn();
    }

    public void GetInput()
    {
        // W / S atau Arrow Up / Down
        InputVertical = Input.GetAxis("Vertical"); 
        // A / D atau Arrow Left / Right
        InputHorizontal = Input.GetAxis("Horizontal"); 

        // Input Mouse untuk menengadah/menunduk
        MouseY = Input.GetAxis("Mouse Y");
    }

    public void Move()
    {
        // Mobil hanya bergerak maju atau mundur searah dengan body-nya sendiri (myTransform.forward)
        Vector3 targetVelocity = myTransform.forward * InputVertical * _moveSpeed;
        
        // Tetap pertahankan efek gravitasi (kecepatan Y jatuh) yang sedang berjalan
        targetVelocity.y = myRigidbody.linearVelocity.y;
        
        // Terapkan ke Rigidbody
        myRigidbody.linearVelocity = targetVelocity;
    }

    public void Turn()
    {
        // Mobil hanya bisa berbelok KETIKA sedang bergerak (maju atau mundur)
        // Jika kamu ingin mobil bisa berbelok saat diam, hapus bagian "if" ini
        if (Mathf.Abs(InputVertical) > 0.01f)
        {
            // Jika mundur (InputVertical negatif), arah belok biasanya dibalik agar natural
            float directionModifier = InputVertical > 0 ? 1f : -1f;

            // Hitung perubahan sudut rotasi Y berdasarkan input A/D
            float turnAmount = InputHorizontal * _turnSpeed * directionModifier * Time.fixedDeltaTime;
            currentRotationY += turnAmount;

            // Terapkan rotasi horizontal ke Rigidbody
            myRigidbody.MoveRotation(Quaternion.Euler(0f, currentRotationY, 0f));
        }
    }

    public void RotateCamera()
    {
        // Mengatur kamera melihat ke atas / bawah secara independen menggunakan mouse Y
        if (MainCameraTransform != null)
        {
            currentRotationX -= MouseY * _mouseSensivity;
            currentRotationX = Mathf.Clamp(currentRotationX, _minRotationX, _maxRotationX);
            
            MainCameraTransform.localRotation = Quaternion.Euler(currentRotationX, 0f, 0f);
        }
    }

    public void Scale()
    {
        if (_scale <= 0f)
        {
            _scale = 1f;
        }
    
        myTransform.localScale = Vector3.one * _scale;
    }

    public void AddPoint(int amount)
    {
        TotalPoints += amount;
        Debug.Log("Poin Bertambah! Total Poin Sekarang: " + TotalPoints);
    }
}