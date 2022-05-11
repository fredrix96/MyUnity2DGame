using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager
{
    GameObject go;
    AudioClip[] audioClips;
    AudioSource audioSource;

    public AudioManager()
    {
        audioClips = Resources.LoadAll<AudioClip>("Audio/");
        go = new GameObject { name = "audio_source" };
        go.transform.parent = GameManager.GameManagerObject.transform;
        audioSource = go.AddComponent<AudioSource>();
    }

    public bool PlayAudio(string audioName, float volume, bool loop = false)
    {
        bool found = false;

        foreach (AudioClip clip in audioClips)
        {
            if (clip.name == audioName)
            {
                found = true;

                audioSource.clip = clip;
                audioSource.volume = volume;
                audioSource.loop = loop;
                audioSource.Play();

                return found;
            }
        }

        if (!found)
        {
            Debug.LogWarning("Warning: No audio clip named " + audioName + " was found!");
        }

        return found;
    }
}
