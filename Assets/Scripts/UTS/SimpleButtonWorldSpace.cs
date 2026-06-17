using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Collections;

public class SimpleButtonWorldSpace : MonoBehaviour
{
    [Header("Apa yang terjadi saat ditekan?")]
    public UnityEvent onButtonPress;
    public event Action OnButtonClicked;
    private Image ButtonImage;
    private Color CurrentStatusColor = Color.white;
    private bool isAnimating = false;

    void Awake()
    {
        ButtonImage = GetComponent<Image>();
    }

    public void PressButton()
    {
        Debug.Log("Tombol World Space Ditekan!");
        onButtonPress?.Invoke();
        OnButtonClicked?.Invoke();

        if (!isAnimating)
        {
            StartCoroutine(PressFeedbackAnimation());
        }
    }

    public void SetButtonColor(Color NewColor)
    {
        if (ButtonImage != null && !isAnimating)
        {
            CurrentStatusColor = NewColor;
            ButtonImage.color = CurrentStatusColor;
        }
    }

    private IEnumerator PressFeedbackAnimation()
    {
        isAnimating = true;
        ButtonImage.color = CurrentStatusColor * 0.5f;
        yield return new WaitForSeconds(0.15f);
        ButtonImage.color = CurrentStatusColor;
        isAnimating = false;
    }
}