using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Collections;

public class SimpleButtonWorldSpace : MonoBehaviour
{
    [Header("Pengaturan Warna")]
    public Color warnaDefault = Color.gray;
    public float durasiFeedback = 1f;

    [Header("Apa yang terjadi saat ditekan?")]
    public UnityEvent onButtonPress;
    public event Action OnButtonClicked;

    private Image ButtonImage;
    private bool isAnimating = false;

    void Awake()
    {
        ButtonImage = GetComponent<Image>();

        if (ButtonImage != null)
        {
            ButtonImage.color = warnaDefault;
        }
    }

    public void PressButton()
    {
        Debug.Log("Tombol World Space Ditekan!");
        onButtonPress?.Invoke();
        OnButtonClicked?.Invoke();
    }

    public void TampilkanFeedbackWarna(Color warnaHasil)
    {
        if (ButtonImage != null && !isAnimating && gameObject.activeInHierarchy)
        {
            StartCoroutine(AnimasiFeedback(warnaHasil));
        }
    }

    private IEnumerator AnimasiFeedback(Color warnaHasil)
    {
        isAnimating = true;
        ButtonImage.color = warnaHasil;
        yield return new WaitForSeconds(durasiFeedback);
        ButtonImage.color = warnaDefault;
        isAnimating = false;
    }
}