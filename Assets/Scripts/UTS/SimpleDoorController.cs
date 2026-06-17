using TMPro;
using UnityEngine;
using System.Collections;

/// <summary>
/// Script pintu dengan tiga mode:
///   - Door A: requiresKeycard=false, alwaysLocked=false → bisa dibuka bebas
///   - Door B: requiresKeycard=true,  alwaysLocked=false → butuh keycard
///   - Door C: alwaysLocked=true                        → selalu terkunci, tampil "System Offline"
///
/// Pintu bisa dibuka via:
///   1. Tombol 3D (SimpleButton3D / SimpleButtonWorldSpace)
///   2. Raycast klik langsung ke pintu (PlayerController)
///   3. Trigger zone — player/car masuk ke collider pintu (untuk Door B otomatis cek keycard)
/// </summary>
public class SimpleDoorController : MonoBehaviour
{
    [Header("Assign Tombol Cube Di Sini")]
    public SimpleButton3D targetButton;
    public SimpleButtonWorldSpace targetWorldButton;

    [Header("Door Movement Settings")]
    public Transform doorMesh;
    public Vector3 openOffset = new Vector3(-1.5f, 0f, 0f);
    public float speed = 3f;

    [Header("Door Access Settings")]
    public bool requiresKeycard = false;
    public bool alwaysLocked = false;
    public KeycardManager keycardManager;
    public TMP_Text statusText;
    public string keycardRequiredMessage = "Keycard Required";
    public string lockedMessage = "System Offline";

    [Header("Trigger Zone Settings")]
    [Tooltip("Jika true, pintu akan otomatis dicoba dibuka saat player/car masuk ke trigger collider")]
    public bool openOnTriggerEnter = false;
    [Tooltip("Tag player atau car yang boleh memicu trigger")]
    public string playerTag = "Player";

    [Header("Status Display")]
    [Tooltip("Jika true, pesan status akan hilang otomatis setelah beberapa detik")]
    public bool autoHideStatus = true;
    public float statusDisplayDuration = 3f;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpen = false;
    private Coroutine hideStatusCoroutine;

    public bool IsOpen => isOpen;

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

        if (targetWorldButton != null)
        {
            targetWorldButton.OnButtonClicked += ToggleDoor;
        }

        // Door C: Langsung tampilkan "System Offline" dari awal
        if (alwaysLocked)
        {
            ShowStatus(lockedMessage, persistent: true);
        }
    }

    void OnDestroy()
    {
        if (targetButton != null)
        {
            targetButton.OnButtonClicked -= ToggleDoor;
        }

        if (targetWorldButton != null)
        {
            targetWorldButton.OnButtonClicked -= ToggleDoor;
        }
    }

    void Update()
    {
        // Animasi buka/tutup pintu
        Vector3 targetPosition = isOpen ? openPosition : closedPosition;
        doorMesh.localPosition = Vector3.Lerp(doorMesh.localPosition, targetPosition, Time.deltaTime * speed);

        // Door C: Paksa statusText selalu tampilkan "System Offline" setiap frame
        // if (alwaysLocked && statusText != null)
        // {
        //     statusText.text = lockedMessage;
        // }
    }

    // ─── TRIGGER ZONE: Player/Car masuk ke collider pintu ─────────────────────
    private void OnTriggerEnter(Collider other)
    {
        if (!openOnTriggerEnter) return;

        // Cek apakah yang masuk adalah player atau car
        bool isPlayer = other.CompareTag(playerTag)
                        || other.GetComponent<CubeController>() != null
                        || other.GetComponent<PlayerController>() != null;

        if (!isPlayer) return;

        // Coba ambil KeycardManager dari player/car jika belum di-assign
        if (keycardManager == null)
        {
            keycardManager = other.GetComponentInChildren<KeycardManager>();
            if (keycardManager == null)
                keycardManager = other.GetComponentInParent<KeycardManager>();
        }

        ToggleDoor();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!openOnTriggerEnter) return;

        bool isPlayer = other.CompareTag(playerTag)
                        || other.GetComponent<CubeController>() != null
                        || other.GetComponent<PlayerController>() != null;

        // Tutup kembali pintu saat player keluar (opsional — hanya jika pintu sudah terbuka)
        if (isPlayer && isOpen && !alwaysLocked)
        {
            isOpen = false;
            ShowStatus("Door Closed", persistent: false);
            Debug.Log($"[Door] [{gameObject.name}] Player keluar → pintu ditutup.");
        }
    }

    // ─── TOGGLE MANUAL via Tombol / Raycast ───────────────────────────────────
    public void ToggleDoor()
    {
        if (alwaysLocked)
        {
            // Door C: Selalu tampilkan System Offline, tidak pernah bisa dibuka
            ShowStatus(lockedMessage, persistent: true);
            Debug.Log($"[Door] [{gameObject.name}] ALWAYS LOCKED: {lockedMessage}");
            return;
        }

        if (requiresKeycard && !HasKeycard())
        {
            // Door B: Butuh keycard dulu
            ShowStatus(keycardRequiredMessage, persistent: false);
            Debug.Log($"[Door] [{gameObject.name}] Butuh keycard — akses ditolak.");
            return;
        }

        // Door A atau Door B (sudah punya keycard): toggle buka/tutup
        isOpen = !isOpen;
        ShowStatus(isOpen ? "Access Granted" : "Door Closed", persistent: false);
        Debug.Log($"[Door] [{gameObject.name}] IsOpen = {isOpen}");
    }

    private bool HasKeycard()
    {
        if (keycardManager != null)
        {
            return keycardManager.HasKeycard;
        }
        // Fallback ke global static
        return KeycardManager.HasKeycardGlobal;
    }

    /// <summary>
    /// Tampilkan pesan pada statusText.
    /// persistent = true  → pesan tidak akan auto-hide (untuk Door C)
    /// persistent = false → pesan hilang otomatis setelah statusDisplayDuration detik
    /// </summary>
    private void ShowStatus(string message, bool persistent)
    {
        if (statusText == null) return;

        // Hentikan coroutine hide sebelumnya jika ada
        if (hideStatusCoroutine != null)
        {
            StopCoroutine(hideStatusCoroutine);
            hideStatusCoroutine = null;
        }

        statusText.text = message;

        if (!persistent && autoHideStatus)
        {
            hideStatusCoroutine = StartCoroutine(HideStatusAfterDelay());
        }
    }

    private IEnumerator HideStatusAfterDelay()
    {
        yield return new WaitForSeconds(statusDisplayDuration);

        // Jangan hapus jika door ini alwaysLocked (override di Update)
        if (!alwaysLocked && statusText != null)
        {
            statusText.text = "";
        }
    }
}
