using TMPro;
using UnityEngine;

public class KeycardManager : MonoBehaviour
{
    public TMP_Text statusText;

    public bool HasKeycard { get; private set; }
    public static bool HasKeycardGlobal { get; private set; }

    void Start()
    {
        // Tampilkan status awal
        if (statusText != null)
        {
            statusText.text = HasKeycardGlobal ? "Keycard Acquired" : "No Keycard";
        }
    }

    public void TakeKeycard()
    {
        HasKeycard = true;
        HasKeycardGlobal = true;

        if (statusText != null)
        {
            statusText.text = "Keycard Acquired";
        }

        Debug.Log("[KeycardManager] Keycard Acquired!");
    }

    public void ResetKeycard()
    {
        HasKeycard = false;
        HasKeycardGlobal = false;

        if (statusText != null)
        {
            statusText.text = "No Keycard";
        }
    }

    /// <summary>
    /// Fallback static setter — dipakai oleh KeycardPickup 
    /// jika tidak ada referensi langsung ke instance KeycardManager.
    /// </summary>
    public static void SetGlobalKeycard(bool value)
    {
        HasKeycardGlobal = value;
        Debug.Log($"[KeycardManager] Global keycard set to: {value}");
    }
}
