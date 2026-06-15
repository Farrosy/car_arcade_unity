using UnityEngine;
using TMPro;
using UnityEngine.UI;   

public class Latihan3Script : MonoBehaviour
{
    public int MaxAmmo, CurrentAmmo;
    public TextMeshProUGUI TextAmmo;
    public Button ButtonShoot, ButtonReaload;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentAmmo = MaxAmmo;
        TextAmmo.text = CurrentAmmo + "/" + MaxAmmo;

        ButtonShoot.onClick.AddListener(Shoot);
        ButtonShoot.onClick.AddListener(Reload);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot()
    {
        CurrentAmmo = CurrentAmmo - 1;
        TextAmmo.text = CurrentAmmo + "/" + MaxAmmo;

        if (CurrentAmmo == 0)
        {
            ButtonShoot.interactable = false;
        }
    }

    public void Reload()
    {
        CurrentAmmo = MaxAmmo;
        TextAmmo.text = CurrentAmmo + "/" + MaxAmmo;
        ButtonShoot.interactable = true;
    }
}
