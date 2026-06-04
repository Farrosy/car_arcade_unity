using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private Button _startButton;
    [SerializeField] private TextController _textController;

    private void Start()
    {
        _startButton.interactable = true;
        _startButton.onClick.AddListener(_textController.ChangeText);
    }
}
