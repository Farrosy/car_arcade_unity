using UnityEngine;
using TMPro;
using System.Collections;
public class CollisionFinish : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI notifText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
            if (notifText != null)
            {
                StartCoroutine(TimerText("ENTER! FINISH"));
            }
        }
    }

    IEnumerator TimerText(string currentText)
    {
        notifText.text = currentText;

        yield return new WaitForSeconds(3f);

        notifText.text = "";
    }

    // void OnTriggerStay(Collider other)
    // {
    //     if (other.gameObject.tag == "Finish")
    //     {
    //         print("STAY");
    //     }
        
    // }

    // void OnTriggerExit(Collider other)
    // {
    //     if (other.gameObject.tag == "Finish")
    //     {
    //         print("EXIT");
    //     }
    // }
}
