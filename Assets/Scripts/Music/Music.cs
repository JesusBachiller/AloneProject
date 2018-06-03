using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour
{
    public AudioSource audioSource;

    public int count = 0;

    public AudioClip[] musics;

    private void Start()
    {
        playNextMusic();
    }

    private void FixedUpdate()
    {
        if(audioSource.isPlaying == false)
        {
            playNextMusic();
        }
    }

    private void playNextMusic()
    {
        audioSource.clip = musics[count];
        audioSource.Play();
        count++;
        if(count == musics.Length)
        {
            count = 0;
        }
    }
}
