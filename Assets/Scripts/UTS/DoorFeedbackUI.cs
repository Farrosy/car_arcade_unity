using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DoorFeedbackUI : MonoBehaviour
{
    [Header("Status Text Settings (World Space)")]
    [SerializeField] private TextMeshProUGUI statusTextA;
    [SerializeField] private TextMeshProUGUI statusTextB;
    [SerializeField] private TextMeshProUGUI statusTextC;

    [Header("Color Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color warningColor = Color.red;
    [SerializeField] private Color successColor = Color.green;

    [Header("Popup Settings (Screen Overlay)")]
    [SerializeField] private CanvasGroup popupCanvasGroup;
    [SerializeField] private TextMeshProUGUI popupText;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float displayDuration = 1.5f;

    [Header("3D Physical Button Settings")]
    [SerializeField] private SimpleButton3D targetButtonA;
    [SerializeField] private SimpleButton3D targetButtonB;
    [SerializeField] private SimpleButton3D targetButtonC;
    [SerializeField] private SimpleButton3D targetButtonKeycard;

    // === VARIABEL PENANDA STATUS GAME ===
    private bool isDoorAOpened = false; 
    private bool isDoorBOpened = false; 
    private bool hasKeycard = false;    

    private void Start()
    {
        // FIX PADA START: Semua papan status di awal game di-set kosong ("") atau Terminal Ready bawaan standar
        UpdateIndividualStatus(statusTextA, "Terminal Ready", normalColor);
        UpdateIndividualStatus(statusTextB, "Terminal Ready", normalColor);
        
        // Sesuai Request: Pintu C kosong dulu di awal game, tidak langsung memunculkan System Offline
        UpdateIndividualStatus(statusTextC, "", normalColor);
        
        if (popupCanvasGroup != null)
        {
            popupCanvasGroup.gameObject.SetActive(false);
            popupCanvasGroup.alpha = 0f;
        }

        // Daftarkan fungsi klik ke semua tombol fisik
        if (targetButtonA != null) targetButtonA.OnButtonClicked += HandleButtonAClicked;
        if (targetButtonB != null) targetButtonB.OnButtonClicked += HandleButtonBClicked;
        if (targetButtonC != null) targetButtonC.OnButtonClicked += HandleButtonCClicked;
        if (targetButtonKeycard != null) targetButtonKeycard.OnButtonClicked += HandleButtonKeycardClicked;
    }

    private void OnDestroy()
    {
        if (targetButtonA != null) targetButtonA.OnButtonClicked -= HandleButtonAClicked;
        if (targetButtonB != null) targetButtonB.OnButtonClicked -= HandleButtonBClicked;
        if (targetButtonC != null) targetButtonC.OnButtonClicked -= HandleButtonCClicked;
        if (targetButtonKeycard != null) targetButtonKeycard.OnButtonClicked -= HandleButtonKeycardClicked;
    }

    // === 1. LOGIKA TOMBOL AMBIL KEYCARD ===
    private void HandleButtonKeycardClicked()
    {
        if (!hasKeycard)
        {
            hasKeycard = true; 
            
            // Sesuai Request: Di tengah layar overlay ganti menjadi "ACCESS GRANTED" atau "KEYCARD ACQUIRED" yang sifatnya umum, 
            // di sini kita beri "ACCESS GRANTED" agar seragam dengan fungsi akses sukses.
            TriggerPopup("ACCESS GRANTED", true);

            // Set sistem global KeycardManager bawaan UTS
            KeycardManager.SetGlobalKeycard(true);

            // Hilangkan visual objek Keycard dari map
            if (targetButtonKeycard != null)
            {
                foreach (Renderer r in targetButtonKeycard.GetComponentsInChildren<Renderer>()) r.enabled = false;
                foreach (Collider c in targetButtonKeycard.GetComponentsInChildren<Collider>()) c.enabled = false;
                Destroy(targetButtonKeycard.gameObject, 0.1f);
            }
        }
    }

    // === 2. LOGIKA PINTU A ===
    private void HandleButtonAClicked()
    {
        if (!isDoorAOpened)
        {
            isDoorAOpened = true;
            UpdateIndividualStatus(statusTextA, "Door A Opened", successColor);
            TriggerPopup("ACCESS GRANTED", true);
        }
        else
        {
            isDoorAOpened = false;
            UpdateIndividualStatus(statusTextA, "Terminal Ready", normalColor);
        }
    }

    // === 3. LOGIKA PINTU B (Butuh Keycard) ===
    private void HandleButtonBClicked()
    {
        if (hasKeycard)
        {
            if (!isDoorBOpened)
            {
                isDoorBOpened = true;
                // Status akses melayang berubah sesuai request kamu
                UpdateIndividualStatus(statusTextB, "Keycard Acquired", successColor);
                
                // FIX OVERLAY: Di layar tengah hanya memunculkan "ACCESS GRANTED" murni
                TriggerPopup("ACCESS GRANTED", true);

                // Jalankan fungsi buka fisik pintu milik temanmu
                SimpleDoorController doorBController = targetButtonB.GetComponentInParent<SimpleDoorController>();
                if (doorBController != null)
                {
                    doorBController.ToggleDoor();
                }
            }
            else
            {
                isDoorBOpened = false;
                UpdateIndividualStatus(statusTextB, "Terminal Ready", normalColor);
                
                SimpleDoorController doorBController = targetButtonB.GetComponentInParent<SimpleDoorController>();
                if (doorBController != null && doorBController.IsOpen)
                {
                    doorBController.ToggleDoor();
                }
            }
        }
        else
        {
            // Status akses melayang berubah jadi "Need Keycard"
            UpdateIndividualStatus(statusTextB, "Need Keycard", warningColor);
            
            // FIX OVERLAY: Di layar tengah murni memunculkan teks dasar "ACCESS DENIED" saja
            TriggerPopup("ACCESS DENIED", false);
        }
    }

    // === 4. LOGIKA PINTU C (Selalu Offline) ===
    private void HandleButtonCClicked()
    {
        // Sesuai Request: Ketika diklik baru tulisan "System Offline" muncul di papan melayang pintu C
        UpdateIndividualStatus(statusTextC, "System Offline", warningColor);
        
        // FIX OVERLAY: Di layar tengah murni memunculkan teks dasar "ACCESS DENIED" saja
        TriggerPopup("ACCESS DENIED", false);
    }

    // === FUNGSI RE-USEABLE UNTUK MENGUBAH TEKS TERTENTU ===
    private void UpdateIndividualStatus(TextMeshProUGUI targetText, string message, Color textColor)
    {
        if (targetText != null)
        {
            targetText.text = message;
            targetText.color = textColor;
            targetText.ForceMeshUpdate();
        }
    }

    // === FUNGSI POPUP OVERLAY SCREEN ===
    public void TriggerPopup(string message, bool isSuccess)
    {
        if (popupCanvasGroup == null || popupText == null) return;

        StopAllCoroutines(); 
        popupText.text = message;
        popupText.color = isSuccess ? successColor : warningColor;
        StartCoroutine(FadePopupRoutine());
    }

    private System.Collections.IEnumerator FadePopupRoutine()
    {
        popupCanvasGroup.gameObject.SetActive(true);
        float counter = 0f;
        while (counter < fadeDuration)
        {
            counter += Time.deltaTime;
            popupCanvasGroup.alpha = Mathf.Lerp(0f, 1f, counter / fadeDuration);
            yield return null;
        }
        yield return new WaitForSeconds(displayDuration);
        counter = 0f;
        while (counter < fadeDuration)
        {
            counter += Time.deltaTime;
            popupCanvasGroup.alpha = Mathf.Lerp(1f, 0f, counter / fadeDuration);
            yield return null;
        }
        popupCanvasGroup.gameObject.SetActive(false);
    }
}