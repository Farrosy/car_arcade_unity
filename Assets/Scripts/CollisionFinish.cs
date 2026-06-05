using UnityEngine;
using TMPro;
using System.Collections;

public class CollisionFinish : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI notifText;

    void Start()
    {
        if (notifText != null)
        {
            StartCoroutine(TimerText("START, GO TO FINISH!"));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Finish"))
        {
            // Hentikan timer & simpan rekor
            if (RaceTimer.Instance != null)
            {
                RaceTimer.Instance.StopTimer();
            }

            if (notifText != null)
            {
                string finishMsg = "FINISH!";

                // Tambahkan waktu finish ke notifikasi
                if (RaceTimer.Instance != null)
                {
                    string timeStr = RaceTimer.FormatTime(RaceTimer.Instance.ElapsedTime);
                    finishMsg = $"FINISH!\nWaktu: {timeStr}";
                }

                StartCoroutine(TimerText(finishMsg));
            }
        }
    }

    IEnumerator TimerText(string currentText)
    {
        notifText.text = currentText;

        yield return new WaitForSeconds(3f);

        notifText.text = "";
    }
}
