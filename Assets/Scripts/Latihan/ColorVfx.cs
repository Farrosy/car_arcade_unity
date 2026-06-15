using UnityEngine;

using UnityEngine.UI;

 

public class ColorVfx : MonoBehaviour

{

    public Image GambarOverlay;

    public float DamageAlpha, HealAlpha;

    public Color DamageColor, HealColor;

    public float FadeSpeed = 0.1f;

 

    public float CurrentAlpha;

    public Color CurrentColor;

 

    private void Start()

    {

        CurrentAlpha = 1f;

    }

 

    private void Update()

    {

        CurrentAlpha -= FadeSpeed * Time.deltaTime;

        ChangeColor();

    }

 

    public void ChangeColor() 

    {

        CurrentColor = DamageColor;

        CurrentColor.a = CurrentAlpha;

 

        GambarOverlay.color = CurrentColor;

    }

}