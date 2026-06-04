
using TMPro; 
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Beginner practice script for changing TextMeshPro text when a Button is clicked. 
/// </summary>
public class ButtonTextPractice: MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _messageText; 
    [SerializeField] private Button _changeTextButton;
    
    /// <summary>
    /// Initializes the text and registers the button click event.
    /// </summary>
    
    private void Start()
    {
        _messageText.text = "Belum diklik";
        _changeTextButton.onClick.AddListener(ChangeText);
    }
    /// <summary>
    /// Changes the TextMeshPro text when the button is clicked.
    /// </summary>
    private void ChangeText()
    {
        _messageText.text = "Button sudah diklik!";
    }
}