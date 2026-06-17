using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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

    [Header("Flexible Destroy Settings")]
    [Tooltip("Masukkan daftar objek (misal: Keycard_DoorC, barrier, dll) yang ingin ikut terhapus saat keycard diambil")]
    [SerializeField] private GameObject[] objectsToDestroy;

    // === VARIABEL PENANDA STATUS GAME ===
    private bool hasKeycard = false;    
    private bool isDoorAOpened = false; 
    private bool isDoorBOpened = false; 

    // Coroutine helper untuk mencatat reset status agar tidak tumpang tindih
    private Coroutine resetStatusBCoroutine;
    private Coroutine resetStatusCCoroutine;
    private Coroutine popupCoroutine;

    private void Start()
    {
        UpdateIndividualStatus(statusTextA, "Terminal Ready", normalColor);
        UpdateIndividualStatus(statusTextB, "Terminal Ready", normalColor);
        UpdateIndividualStatus(statusTextC, "Terminal Error", warningColor);
        
        if (popupCanvasGroup != null)
        {
            popupCanvasGroup.gameObject.SetActive(false);
            popupCanvasGroup.alpha = 0f;
        }

        // Subskripsi Event Tombol
        if (targetButtonA != null) targetButtonA.OnButtonClicked += HandleButtonAClicked;
        if (targetButtonB != null) targetButtonB.OnButtonClicked += HandleButtonBClicked;
        if (targetButtonC != null) targetButtonC.OnButtonClicked += HandleButtonCClicked;
        if (targetButtonKeycard != null) targetButtonKeycard.OnButtonClicked += HandleButtonKeycardClicked;
    }

    private void OnDestroy()
    {
        // Unsubskripsi Event untuk menghindari Memory Leak
        if (targetButtonA != null) targetButtonA.OnButtonClicked -= HandleButtonAClicked;
        if (targetButtonB != null) targetButtonB.OnButtonClicked -= HandleButtonBClicked;
        if (targetButtonC != null) targetButtonC.OnButtonClicked -= HandleButtonCClicked;
        if (targetButtonKeycard != null) targetButtonKeycard.OnButtonClicked -= HandleButtonKeycardClicked;
    }

    // === 1. LOGIKA TOMBOL AMBIL KEYCARD (TRIGGER UTAMA) ===
    private void HandleButtonKeycardClicked()
    {
        if (!hasKeycard)
        {
            hasKeycard = true; 
            TriggerPopup("ACCESS GRANTED", true);
            KeycardManager.SetGlobalKeycard(true);

            // Menghapus daftar objek eksternal yang didaftarkan di Inspector (misal: Keycard_DoorC)
            if (objectsToDestroy != null && objectsToDestroy.Length > 0)
            {
                foreach (GameObject obj in objectsToDestroy)
                {
                    if (obj != null)
                    {
                        Destroy(obj);
                    }
                }
            }

            // Menghancurkan visual tombol keycard itu sendiri secara aman
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
            if (resetStatusBCoroutine != null) StopCoroutine(resetStatusBCoroutine);

            if (!isDoorBOpened)
            {
                isDoorBOpened = true;
                UpdateIndividualStatus(statusTextB, "Keycard Acquired", successColor);
                TriggerPopup("ACCESS GRANTED", true);

                SimpleDoorController doorBController = targetButtonB.GetComponentInParent<SimpleDoorController>();
                if (doorBController != null) doorBController.ToggleDoor();
            }
            else
            {
                isDoorBOpened = false;
                UpdateIndividualStatus(statusTextB, "Terminal Ready", normalColor);
                
                SimpleDoorController doorBController = targetButtonB.GetComponentInParent<SimpleDoorController>();
                if (doorBController != null && doorBController.IsOpen) doorBController.ToggleDoor();
            }
        }
        else
        {
            UpdateIndividualStatus(statusTextB, "Need Keycard", warningColor);
            TriggerPopup("ACCESS DENIED", false);

            if (resetStatusBCoroutine != null) StopCoroutine(resetStatusBCoroutine);
            resetStatusBCoroutine = StartCoroutine(ResetStatusAfterDelay(statusTextB, "Terminal Ready", normalColor, displayDuration + fadeDuration));
        }
    }

    // === 4. LOGIKA PINTU C (Selalu Offline) ===
    private void HandleButtonCClicked()
    {
        UpdateIndividualStatus(statusTextC, "System Offline", warningColor);
        TriggerPopup("ACCESS DENIED", false);
        
        if (resetStatusCCoroutine != null) StopCoroutine(resetStatusCCoroutine);
        resetStatusCCoroutine = StartCoroutine(ResetStatusAfterDelay(statusTextC, "Terminal Error", warningColor, displayDuration + fadeDuration));
    }

    // === FUNGSI RE-USEABLE UNTUK MENGUBAH TEKS ===
    private void UpdateIndividualStatus(TextMeshProUGUI targetText, string message, Color textColor)
    {
        if (targetText != null)
        {
            targetText.text = message;
            targetText.color = textColor;
            targetText.ForceMeshUpdate();
        }
    }

    // === COROUTINE: RESET TEKS SETELAH JEDA WAKTU ===
    private IEnumerator ResetStatusAfterDelay(TextMeshProUGUI targetText, string defaultMessage, Color defaultColor, float delay)
    {
        yield return new WaitForSeconds(delay);
        UpdateIndividualStatus(targetText, defaultMessage, defaultColor);
    }

    // === FUNGSI POPUP OVERLAY SCREEN ===
    public void TriggerPopup(string message, bool isSuccess)
    {
        if (popupCanvasGroup == null || popupText == null) return;
        if (popupCoroutine != null) StopCoroutine(popupCoroutine); 
        
        popupText.text = message;
        popupText.color = isSuccess ? successColor : warningColor;
        popupCoroutine = StartCoroutine(FadePopupRoutine());
    }

    private IEnumerator FadePopupRoutine()
    {
        popupCanvasGroup.gameObject.SetActive(true);
        float counter = 0f;
        
        // Fade In
        while (counter < fadeDuration)
        {
            counter += Time.deltaTime;
            popupCanvasGroup.alpha = Mathf.Lerp(0f, 1f, counter / fadeDuration);
            yield return null;
        }
        
        // Hold Display
        yield return new WaitForSeconds(displayDuration);
        
        // Fade Out
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