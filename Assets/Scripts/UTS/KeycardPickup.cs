using TMPro;
using UnityEngine;
using System.Collections;

/// <summary>
/// Taruh script ini pada GameObject Keycard di scene.
/// Keycard bisa diambil dengan DUA cara:
///   1. Raycast Klik (klik kiri saat crosshair mengarah ke keycard)
///   2. Trigger Masuk (player/car masuk ke collider keycard yang di-set Is Trigger)
/// </summary>
public class KeycardPickup : MonoBehaviour
{
    [Header("Keycard Settings")]
    [Tooltip("Referensi ke KeycardManager milik player")]
    public KeycardManager keycardManager;

    [Tooltip("Tag player atau car yang boleh mengambil keycard")]
    public string playerTag = "Player";

    [Tooltip("Notifikasi UI saat keycard diambil (opsional)")]
    public TMP_Text pickupNotifText;

    [Tooltip("Durasi notifikasi tampil (detik)")]
    public float notifDuration = 2.5f;

    [Header("Visual Feedback")]
    [Tooltip("Kecepatan rotasi keycard agar menarik perhatian")]
    public float rotationSpeed = 90f;

    [Tooltip("Efek bobbing naik-turun")]
    public float bobbingAmount = 0.15f;
    public float bobbingSpeed = 2f;

    private Vector3 startPos;
    private bool isPickedUp = false;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (isPickedUp) return;

        // Rotasi keycard agar eye-catching
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);

        // Bobbing naik-turun
        float newY = startPos.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmount;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    // ─── CARA 1: Player/Car masuk ke Trigger Collider keycard ─────────────────
    private void OnTriggerEnter(Collider other)
    {
        // Cek apakah yang masuk adalah player atau car berdasarkan tag
        if (other.CompareTag(playerTag) || other.GetComponent<CubeController>() != null
            || other.GetComponent<PlayerController>() != null)
        {
            // Coba ambil KeycardManager dari player jika belum di-assign
            if (keycardManager == null)
            {
                keycardManager = other.GetComponentInChildren<KeycardManager>();
                if (keycardManager == null)
                    keycardManager = other.GetComponentInParent<KeycardManager>();
            }

            Pickup();
        }
    }

    // ─── CARA 2: Dipanggil dari PlayerController via Raycast Klik ─────────────
    public void Pickup()
    {
        if (isPickedUp) return;
        isPickedUp = true;

        // Beritahu KeycardManager bahwa player sudah punya keycard
        if (keycardManager != null)
        {
            keycardManager.TakeKeycard();
        }
        else
        {
            // Fallback: set global static jika tidak ada referensi langsung
            Debug.LogWarning("[KeycardPickup] keycardManager tidak di-assign! Menggunakan global fallback.");
            KeycardManager.SetGlobalKeycard(true);
        }

        Debug.Log("[KeycardPickup] Keycard berhasil diambil!");

        // Tampilkan notifikasi UI
        if (pickupNotifText != null)
        {
            StartCoroutine(ShowNotif("Keycard Acquired!"));
        }

        // Sembunyikan mesh keycard tapi jangan langsung Destroy
        // agar coroutine notifikasi bisa selesai dulu
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.enabled = false;

        // Matikan semua collider agar tidak trigger ulang
        foreach (Collider c in GetComponentsInChildren<Collider>())
            c.enabled = false;

        Destroy(gameObject, notifDuration + 0.5f);
    }

    IEnumerator ShowNotif(string msg)
    {
        pickupNotifText.text = msg;
        yield return new WaitForSeconds(notifDuration);
        if (pickupNotifText != null)
            pickupNotifText.text = "";
    }
}
