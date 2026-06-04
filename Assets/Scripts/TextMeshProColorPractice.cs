// TextMeshProColorPractice.cs

using TMPro;
using UnityEngine;

/// <summary>
/// Beginner practice script for changing partial text color using TextMeshPro rich text tags.
/// </summary>
public class TextMeshProColorPractice : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _statusText;

    private int healthPoint = 100;

    private void Start()
    {
        UpdateHealthText();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            healthPoint -= 25;

            if (healthPoint < 0)
            {
                healthPoint = 100;
            }

            UpdateHealthText();
        }
    }

    /// <summary>
    /// Updates the health text and changes only the health value color based on the current value.
    /// </summary>
    private void UpdateHealthText()
    {
        if (healthPoint > 50)
        {
            _statusText.text = "HP: <color=green>" + healthPoint + "</color>";
        }
        else if (healthPoint > 25)
        {
            _statusText.text = "HP: <color=yellow>" + healthPoint + "</color>";
        }
        else
        {
            _statusText.text = "HP: <color=red>" + healthPoint + "</color>";
        }
    }
}