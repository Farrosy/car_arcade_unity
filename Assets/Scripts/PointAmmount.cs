using UnityEngine;
using TMPro; 

public class PointAmmount : MonoBehaviour
{
    [Header("Referensi Objek")]
    [SerializeField] private CubeController _playerScript; 
    
    private TextMeshProUGUI _textMeshPro;

    void Start()
    {
        _textMeshPro = GetComponent<TextMeshProUGUI>();

        if (_playerScript == null)
        {
            Debug.LogError("Player Script belum dimasukkan ke dalam komponen PointAmmount di Inspector!");
        }
    }

    void Update()
    {
        if (_playerScript != null && _textMeshPro != null)
        {
            _textMeshPro.text = "Poin: " + _playerScript.TotalPoints;
        }
    }
}