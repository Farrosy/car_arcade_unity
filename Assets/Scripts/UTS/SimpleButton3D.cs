using UnityEngine;
using System;

public class SimpleButton3D : MonoBehaviour
{
    // Event murni kode yang akan didengar oleh pintu
    public event Action OnButtonClicked;

    public void PressButton()
    {
        Debug.Log($"[SUKSES] Tombol 3D Cube '{gameObject.name}' berhasil diklik!");
        // Beritahu pintu kalau tombol ini sudah ditekan
        OnButtonClicked?.Invoke();
    }
}