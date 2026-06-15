using UnityEngine;

public class SimpleDoorController : MonoBehaviour
{
    [Header("Assign Tombol Cube Di Sini")]
    public SimpleButton3D targetButton; // Menggunakan skrip tombol 3D yang baru

    [Header("Door Movement Settings")]
    public Transform doorMesh; 
    public Vector3 openOffset = new Vector3(-1.5f, 0f, 0f); 
    public float speed = 3f;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpen = false;

    void Start()
    {
        if (doorMesh == null) doorMesh = transform;
        
        closedPosition = doorMesh.localPosition;
        openPosition = closedPosition + openOffset;

        // Otomatis mendengarkan klik dari tombol 3D
        if (targetButton != null)
        {
            targetButton.OnButtonClicked += ToggleDoor;
        }
        else
        {
            Debug.LogError($"Tombol 3D belum dimasukkan pada objek pintu: {gameObject.name}");
        }
    }

    void OnDestroy()
    {
        if (targetButton != null)
        {
            targetButton.OnButtonClicked -= ToggleDoor;
        }
    }

    void Update()
    {
        Vector3 targetPosition = isOpen ? openPosition : closedPosition;
        doorMesh.localPosition = Vector3.Lerp(doorMesh.localPosition, targetPosition, Time.deltaTime * speed);
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
        Debug.Log($"Pintu [{gameObject.name}] Terbuka = {isOpen}");
    }
}