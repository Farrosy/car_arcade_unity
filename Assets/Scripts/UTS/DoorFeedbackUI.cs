using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DoorFeedbackUI : MonoBehaviour
{
    [Header("Status Text Settings")]
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color warningColor = Color.red;
    [SerializeField] private Color successColor = Color.green;

    [Header("Popup Settings (Bonus Points)")]
    [SerializeField] private CanvasGroup popupCanvasGroup;
    [SerializeField] private TextMeshProUGUI popupText;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float displayDuration = 1.5f;

    [Header("3D Physical Button Settings")]
    [Tooltip("Tarik objek TombolPintu_A yang ada script SimpleButton3D milik temen lu ke sini")]
    [SerializeField] private SimpleButton3D targetButtonA;

    // === VARIABEL PENANDA STATUS PINTU ===
    // Secara default, saat game mulai pintu dalam posisi tertutup (false)
    private bool isDoorAOpened = false; 

    private void Start()
    {
        UpdateStatus("Terminal Ready. Select a door.", normalColor);
        
        if (popupCanvasGroup != null)
        {
            popupCanvasGroup.gameObject.SetActive(false);
            popupCanvasGroup.alpha = 0f;
        }

        if (targetButtonA != null)
        {
            targetButtonA.OnButtonClicked += HandleButtonAClicked;
        }
    }

    private void OnDestroy()
    {
        if (targetButtonA != null)
        {
            targetButtonA.OnButtonClicked -= HandleButtonAClicked;
        }
    }

    // Fungsi otomatis yang berjalan kalau tombol temen lu ditekan
    private void HandleButtonAClicked()
    {
        // JIKA PINTU LAGI TERTUTUP (Kondisi pertama kali klik)
        if (!isDoorAOpened)
        {
            // 1. Ubah status teks di terminal jadi dibuka
            ShowDoorAOpened();
            
            // 2. Jalankan popup "ACCESS GRANTED" lu
            TriggerPopup("ACCESS GRANTED", true);
            
            // 3. Set status kalau pintu SEKARANG SUDAH TERBUKA
            isDoorAOpened = true;
        }
        // JIKA PINTU LAGI TERBUKA (Kondisi klik kedua kalinya / menutup)
        else
        {
            // 1. Ubah status teks di terminal jadi tertutup kembali
            UpdateStatus("Terminal Ready. Door A Closed.", normalColor);
            
            // 2. KOSONGKAN / JANGAN PANGGIL TRIGGER POPUP LU DI SINI
            // Biar popup-nya ga muncul pas nutup pintu.

            // 3. Set status kalau pintu SEKARANG SUDAH TERTUTUP KEMBALI
            isDoorAOpened = false;
        }
    }

    public void UpdateStatus(string message, Color textColor)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.color = textColor;
        }
    }

    public void ShowDoorAOpened() => UpdateStatus("Door A Opened", successColor);
    public void ShowNeedKeycard() => UpdateStatus("Need Keycard", warningColor);
    public void ShowKeycardAcquired() => UpdateStatus("Keycard Acquired", successColor);
    public void ShowSystemOffline() => UpdateStatus("System Offline", warningColor);

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