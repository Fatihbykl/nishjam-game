using UnityEngine;
using UnityEngine.Audio;

public class PlayerSounds : MonoBehaviour
{
    public AudioSource stepSounds;

    public void Step()
    {
        stepSounds.Play();
    }
}
