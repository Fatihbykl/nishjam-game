using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource ambience;
    public AudioSource music;
    public AudioSource selection;

    public float default_volume = 0.4f;
    public float transition_time = 1f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
        
    }

    public void StartGameplayMusic()
    {
        ambience.Play();
        music.Play();
    }

    public void EnterSelectionMode()
    {
        AudioSource now = music;
        AudioSource target = selection;
        if (now.isPlaying == false)
        {
            now = selection;
            target = music;
        }

        StartCoroutine(MixSources(now, target));
    }

    public void ExitSelectionMode()
    {
        AudioSource now = music;
        AudioSource target = selection;
        if (now.isPlaying == false)
        {
            now = selection;
            target = music;
        }

        StartCoroutine(MixSources(now, target));
    }

    IEnumerator MixSources(AudioSource now, AudioSource target)
    {
        float percentage = 0f;
        while (now.volume > 0)
        {
            now.volume = Mathf.Lerp(default_volume, 0, percentage);
            percentage += Time.deltaTime / transition_time;
            yield return null;
        }
        now.Pause();
        if (target.isPlaying == false)
        {
            target.Play();
        }
        target.UnPause();
        percentage = 0f;

        while (target.volume < default_volume)
        {
            target.volume = Mathf.Lerp(0, default_volume, percentage);
            percentage += Time.deltaTime / transition_time;
            yield return null;
        }
    }

}
