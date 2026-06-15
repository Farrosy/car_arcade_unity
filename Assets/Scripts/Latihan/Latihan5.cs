using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Latihan5 : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private Button _damageButton;
    [SerializeField] private Button _healButton;
    [SerializeField] private Image _effectImage;

    [Header("HP Settings")]
    [SerializeField] private int _maxHp = 100;
    [SerializeField] private int _damageAmount = 10;
    [SerializeField] private int _healAmount = 10;

    [Header("Effect Settings")]
    [SerializeField, Range(0f, 1f)] private float _damageAlpha = 0.6f;
    [SerializeField, Range(0f, 1f)] private float _healAlpha = 0.4f;
    [SerializeField, Range(0f, 1f)] private float _lowHpAlpha = 0.2f;
    [SerializeField] private float _fadeSpeed = 2.5f;

    private int currentHp;
    private Color currentEffectColor;
    private float currentAlpha;
    private float targetAlpha;

    /// <summary>
    /// Inisialisasi nilai awal HP dan warna efek.
    /// </summary>
    private void Awake()
    {
        // Validasi agar Max HP tidak kurang dari 1
        if (_maxHp < 1) _maxHp = 1;

        currentHp = _maxHp;
        currentEffectColor = Color.red;
        currentAlpha = 0f;
        targetAlpha = 0f;
    }

    /// <summary>
    /// Setup event listener tombol dan UI awal.
    /// </summary>
    private void Start()
    {
        _damageButton.onClick.AddListener(TakeDamage);
        _healButton.onClick.AddListener(Heal);

        UpdateStatusText("HP penuh. Siap bermain.");
        UpdateUI();
        UpdateEffectImageColor();
    }

    /// <summary>
    /// Transisi alpha ke targetAlpha secara halus (fade).
    /// </summary>
    private void Update()
    {
        if (currentAlpha > targetAlpha)
        {
            currentAlpha -= _fadeSpeed * Time.deltaTime;
            if (currentAlpha < targetAlpha) currentAlpha = targetAlpha;

            UpdateEffectImageColor();
        }
        else if (currentAlpha < targetAlpha)
        {
            currentAlpha += _fadeSpeed * Time.deltaTime;
            if (currentAlpha > targetAlpha) currentAlpha = targetAlpha;

            UpdateEffectImageColor();
        }
    }

    /// <summary>
    /// Membersihkan listener button saat objek dihancurkan.
    /// </summary>
    private void OnDestroy()
    {
        _damageButton.onClick.RemoveListener(TakeDamage);
        _healButton.onClick.RemoveListener(Heal);
    }

    /// <summary>
    /// Mengurangi HP dan memicu efek warna merah.
    /// </summary>
    private void TakeDamage()
    {
        currentHp -= _damageAmount;
        currentHp = ClampHpValue(currentHp);

        currentEffectColor = Color.red;
        currentAlpha = _damageAlpha;
        targetAlpha = GetLowHpAlpha();

        if (currentHp <= 0) UpdateStatusText("Player kalah.");
        else if (IsHpLow()) UpdateStatusText("HP rendah!");
        else UpdateStatusText("Player terkena damage.");

        UpdateUI();
        UpdateEffectImageColor();
    }

    /// <summary>
    /// Menambah HP dan memicu efek warna hijau.
    /// </summary>
    private void Heal()
    {
        currentHp += _healAmount;
        currentHp = ClampHpValue(currentHp);

        currentEffectColor = Color.green;
        currentAlpha = _healAlpha;
        targetAlpha = GetLowHpAlpha();

        if (currentHp >= _maxHp) UpdateStatusText("HP penuh kembali.");
        else UpdateStatusText("Player mendapatkan heal.");

        UpdateUI();
        UpdateEffectImageColor();
    }

    /// <summary>
    /// Membatasi nilai HP agar tetap di rentang 0 sampai Max HP.
    /// </summary>
    private int ClampHpValue(int hpValue)
    {
        if (hpValue < 0) return 0;
        if (hpValue > _maxHp) return _maxHp;
        return hpValue;
    }

    /// <summary>
    /// Memperbarui teks HP dan status interaksi tombol.
    /// </summary>
    private void UpdateUI()
    {
        UpdateHpText();
        UpdateButtonStates();
    }

    /// <summary>
    /// Memperbarui teks HP dan warnanya berdasarkan persentase sisa HP.
    /// </summary>
    private void UpdateHpText()
    {
        string hpColor = "white";

        if (currentHp <= _maxHp * 0.25f) hpColor = "red";       // Sisa HP <= 25%
        else if (currentHp <= _maxHp * 0.5f) hpColor = "yellow"; // Sisa HP <= 50%

        _hpText.text = $"HP: <color={hpColor}>{currentHp}</color>/{_maxHp}";
    }

    /// <summary>
    /// Mengatur keaktifan tombol berdasarkan kondisi HP.
    /// </summary>
    private void UpdateButtonStates()
    {
        _damageButton.interactable = currentHp > 0;
        _healButton.interactable = currentHp < _maxHp;
    }

    /// <summary>
    /// Mengaplikasikan warna dan transparansi (alpha) ke UI Image.
    /// </summary>
    private void UpdateEffectImageColor()
    {
        Color color = currentEffectColor;
        color.a = currentAlpha;
        _effectImage.color = color;
    }

    /// <summary>
    /// Mengubah teks status/notifikasi.
    /// </summary>
    private void UpdateStatusText(string message)
    {
        _statusText.text = message;
    }

    /// <summary>
    /// Mengecek apakah HP berada di bawah atau sama dengan 25%.
    /// </summary>
    private bool IsHpLow()
    {
        return currentHp <= _maxHp * 0.25f;
    }

    /// <summary>
    /// Menentukan batas minimal alpha (efek menetap jika HP rendah).
    /// </summary>
    private float GetLowHpAlpha()
    {
        return IsHpLow() ? _lowHpAlpha : 0f;
    }
}