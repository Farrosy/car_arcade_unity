using UnityEngine;
using System;

public class SimpleButtonWorldSpace : MonoBehaviour
{
    // Event murni kode menggunakan Action C#
    public event Action OnButtonClicked;

    public void PressButton()
    {
        Debug.Log($"Tombol [{gameObject.name}] berhasil ditembak oleh Raycast!");
        
        // Picu fungsi pintu yang sudah mendaftar lewat kode
        OnButtonClicked?.Invoke();
    }
}