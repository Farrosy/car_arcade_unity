using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GunController : MonoBehaviour
{
    public int MaxAmmo = 15;
    public int CurrentAmmo;

    public TextMeshProUGUI TMP_AmmoCount;

    public Button BTN_Shoot, BTN_Reload;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentAmmo = MaxAmmo;
        UpdateAmmoText();
        BTN_Shoot.onClick.AddListener(Shoot);
        BTN_Reload.onClick.AddListener(Reload);
    }

    public void Shoot()
    {
        CurrentAmmo--; //CurrentAmmo = CurrentAmmo - 1; cCrrentAmmo berkurang 1
        UpdateAmmoText();

        if (CurrentAmmo == 0)
        {
            BTN_Shoot.interactable = false;
        }
    }

    public void Reload() 
    {
        CurrentAmmo = MaxAmmo;
        UpdateAmmoText();
        BTN_Shoot.interactable = true;
    }

    private void UpdateAmmoText()
    {
        TMP_AmmoCount.text = CurrentAmmo + "/" + MaxAmmo;
    }
}
