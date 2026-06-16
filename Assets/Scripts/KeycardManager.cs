using TMPro;
using UnityEngine;

public class KeycardManager : MonoBehaviour
{
    public TMP_Text statusText;

    public void TakeKeycard()
    {
        statusText.text = "Keycard Acquired";
    }
}