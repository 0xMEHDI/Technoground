using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip mainTheme;

    private void Update()
    {
        AudioManager.instance.PlayMusic(mainTheme, 0.05f);
    }
}
