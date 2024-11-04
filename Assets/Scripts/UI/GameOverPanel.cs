using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPanel : MonoBehaviour
{
    public AudioClip gameOverSound;       // Âm thanh khi di chuột qua nút
    public AudioSource audioSource;         // AudioSource để phát âm thanh
    
    public void PlaySound()
    {
        audioSource.PlayOneShot(gameOverSound);
    }
}