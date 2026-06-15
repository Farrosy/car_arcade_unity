using UnityEngine;

using UnityEngine.UI;

 

public class FillAmountController : MonoBehaviour

{

    public Image GambarReload;

    public float Speed;

    public float CurrentAmount;

     

    void Start()

    {

        CurrentAmount = 0;

        GambarReload.fillAmount = CurrentAmount;

    }

 

    void Update()

    {

        CurrentAmount += Speed * Time.deltaTime;

        GambarReload.fillAmount = CurrentAmount;

    }

}