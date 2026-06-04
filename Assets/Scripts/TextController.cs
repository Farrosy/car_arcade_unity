using UnityEngine;
using TMPro;

public class TextController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _messageText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _messageText.text = "<color=red>Teks Merah</color>";
    }

        public void ChangeText()
    {
        _messageText.text = "Button diklik!";
    }
}
