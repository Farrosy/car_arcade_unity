using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Header("Movement Settings")]
    public float interactionDistance = 3f; // Jarak maksimal bisa ngeklik tombol
    public LayerMask interactionLayer;     // Layer khusus untuk tombol pintu
    public float moveSpeed = 5f;
    public float gravity = -9.81f;

    [Header("Camera Settings")]
    public Transform playerCamera;
    public float mouseSensitivity = 200f;
    public float topLookLimit = -90f;
    public float bottomLookLimit = 90f;

    private CharacterController characterController;
    private Vector3 velocity;
    private float xRotation = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Mengunci kursor mouse di tengah layar dan menyembunyikannya
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Tambahkan pemanggilan fungsi di dalam Update()
    void Update()
    {
        HandleLook();
        HandleMovement();
        HandleInteraction(); // <-- Tambahkan ini
    }

    void HandleInteraction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, interactionDistance, interactionLayer))
            {
                // Mencari komponen tombol 3D Cube baru
                SimpleButton3D button3D = hit.collider.GetComponent<SimpleButton3D>();
                if (button3D != null)
                {
                    button3D.PressButton();
                }
            }
        }
    }

    void HandleMovement()
    {
        // 1. Ambil input dari WASD / Arrow Keys
        float moveX = Input.GetAxis("Horizontal"); // A/D atau Kiri/Kanan
        float moveZ = Input.GetAxis("Vertical");   // W/S atau Atas/Bawah

        // 2. Hitung arah gerak berdasarkan hadapan karakter
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        characterController.Move(move * moveSpeed * Time.deltaTime);

        // 3. Efek Gravitasi (supaya karakter tidak melayang)
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Mengunci karakter ke tanah
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    void HandleLook()
    {
        // 1. Ambil input pergerakan mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 2. Putar Kamera ke atas/bawah (Rotasi sumbu X) dan batasi sudutnya
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, topLookLimit, bottomLookLimit);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 3. Putar Badan Karakter ke kiri/kanan (Rotasi sumbu Y)
        transform.Rotate(Vector3.up * mouseX);
    }
    
}