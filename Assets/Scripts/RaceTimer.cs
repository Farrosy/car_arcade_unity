using UnityEngine;
using TMPro;

/// <summary>
/// Mengatur timer balapan: mencatat waktu mulai, waktu finish,
/// menyimpan rekor waktu terbaik DAN rekor koin terbanyak menggunakan PlayerPrefs.
/// </summary>
public class RaceTimer : MonoBehaviour
{
    public static RaceTimer Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerText;       // Tampilan timer berjalan
    [SerializeField] private TextMeshProUGUI bestTimeText;    // Tampilan rekor waktu terbaik
    [SerializeField] private TextMeshProUGUI finishTimeText;  // Tampilan waktu saat finish
    [SerializeField] private TextMeshProUGUI bestCoinText;    // Tampilan rekor koin terbanyak

    [Header("Player Reference")]
    [SerializeField] private CubeController playerScript;    // Referensi ke player untuk ambil TotalPoints

    [Header("Settings")]
    [SerializeField] private string bestTimeKey = "BestRaceTime"; // Kunci PlayerPrefs waktu
    [SerializeField] private string bestCoinKey = "BestCoinRecord"; // Kunci PlayerPrefs koin

    // --- State Internal ---
    private float _startTime;
    private float _finishTime;
    private float _elapsedTime;
    private bool _isRunning = false;
    private bool _hasFinished = false;

    // Properti publik agar script lain bisa cek statusnya
    public bool IsRunning => _isRunning;
    public bool HasFinished => _hasFinished;
    public float ElapsedTime => _elapsedTime;

    private void Awake()
    {
        // Singleton pattern agar bisa diakses dari mana saja
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Langsung mulai timer saat scene dimuat
        StartTimer();
        UpdateBestTimeDisplay();
        UpdateBestCoinDisplay();
    }

    private void Update()
    {
        if (_isRunning && !_hasFinished)
        {
            _elapsedTime = Time.time - _startTime;
            UpdateTimerDisplay(_elapsedTime);
        }
    }

    /// <summary>
    /// Mulai timer. Dipanggil otomatis di Start(), bisa juga dipanggil manual (misalnya dari countdown).
    /// </summary>
    public void StartTimer()
    {
        _startTime = Time.time;
        _elapsedTime = 0f;
        _hasFinished = false;
        _isRunning = true;

        Debug.Log($"[RaceTimer] Timer MULAI pada: {System.DateTime.Now:HH:mm:ss}");

        if (finishTimeText != null) finishTimeText.text = "";
    }

    /// <summary>
    /// Hentikan timer saat pemain mencapai garis finish.
    /// Simpan rekor jika ini adalah waktu terbaik.
    /// </summary>
    public void StopTimer()
    {
        if (!_isRunning || _hasFinished) return;

        _finishTime = Time.time - _startTime;
        _isRunning = false;
        _hasFinished = true;

        Debug.Log($"[RaceTimer] Timer SELESAI! Waktu: {FormatTime(_finishTime)} | Timestamp: {System.DateTime.Now:HH:mm:ss}");

        // Cek apakah ini rekor waktu baru
        CheckAndSaveBestTime(_finishTime);

        // Cek apakah ini rekor koin baru
        if (playerScript != null)
            CheckAndSaveBestCoin(playerScript.TotalPoints);

        // Update tampilan waktu finish
        if (finishTimeText != null)
        {
            finishTimeText.text = $"Waktu Finish: {FormatTime(_finishTime)}";
        }
    }

    /// <summary>
    /// Reset timer untuk memulai ulang.
    /// </summary>
    public void ResetTimer()
    {
        _isRunning = false;
        _hasFinished = false;
        _elapsedTime = 0f;

        if (timerText != null) timerText.text = "00:00.000";
        if (finishTimeText != null) finishTimeText.text = "";

        StartTimer();
    }

    // ----------------------------------------------------------------
    // Private Helpers
    // ----------------------------------------------------------------

    private void CheckAndSaveBestCoin(int coins)
    {
        int savedBest = PlayerPrefs.GetInt(bestCoinKey, 0);

        if (coins > savedBest)
        {
            PlayerPrefs.SetInt(bestCoinKey, coins);
            PlayerPrefs.Save();
            Debug.Log($"[RaceTimer] REKOR KOIN BARU! {coins} koin (sebelumnya: {savedBest})");

            if (bestCoinText != null)
                bestCoinText.text = $"REKOR KOIN BARU! Best: {coins}";
        }
        else
        {
            Debug.Log($"[RaceTimer] Koin {coins} tidak memecahkan rekor ({savedBest} koin).");
            UpdateBestCoinDisplay();
        }
    }

    private void UpdateBestCoinDisplay()
    {
        if (bestCoinText == null) return;

        int savedBest = PlayerPrefs.GetInt(bestCoinKey, 0);
        bestCoinText.text = savedBest == 0
            ? "Best Coin: -"
            : $"Best Coin: {savedBest}";
    }

    private void CheckAndSaveBestTime(float time)
    {
        float savedBest = PlayerPrefs.GetFloat(bestTimeKey, float.MaxValue);

        if (time < savedBest)
        {
            PlayerPrefs.SetFloat(bestTimeKey, time);
            PlayerPrefs.Save();
            Debug.Log($"[RaceTimer] REKOR BARU! {FormatTime(time)} (sebelumnya: {(savedBest == float.MaxValue ? "-" : FormatTime(savedBest))})");

            if (bestTimeText != null)
                bestTimeText.text = $"REKOR BARU! Best: {FormatTime(time)}";
        }
        else
        {
            Debug.Log($"[RaceTimer] Waktu {FormatTime(time)} tidak memecahkan rekor ({FormatTime(savedBest)}).");
            UpdateBestTimeDisplay();
        }
    }

    private void UpdateBestTimeDisplay()
    {
        if (bestTimeText == null) return;

        float savedBest = PlayerPrefs.GetFloat(bestTimeKey, float.MaxValue);
        if (savedBest == float.MaxValue)
            bestTimeText.text = "Best: --:--.---";
        else
            bestTimeText.text = $"Best: {FormatTime(savedBest)}";
    }

    private void UpdateTimerDisplay(float time)
    {
        if (timerText != null)
            timerText.text = FormatTime(time);
    }

    /// <summary>
    /// Format waktu ke format MM:SS.mmm (menit:detik.milidetik)
    /// </summary>
    public static string FormatTime(float timeInSeconds)
    {
        int minutes = (int)(timeInSeconds / 60f);
        int seconds = (int)(timeInSeconds % 60f);
        int milliseconds = (int)((timeInSeconds * 1000f) % 1000f);
        return $"{minutes:00}:{seconds:00}.{milliseconds:000}";
    }
}
