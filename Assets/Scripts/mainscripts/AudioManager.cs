using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager
{
    GameObject go;
    AudioClip[] audioClips;
    AudioSource audioSource2D;
    AudioSource audioSource3D;
    AudioSource backgroundMusic;

    public AudioManager()
    {
        audioClips = Resources.LoadAll<AudioClip>("Audio/");
        go = new GameObject { name = "audio_source" };
        go.transform.parent = GameManager.GameManagerObject.transform;
        backgroundMusic = go.AddComponent<AudioSource>();
        audioSource2D = go.AddComponent<AudioSource>();
        audioSource3D = go.AddComponent<AudioSource>();
    }

    public bool PlayBackgroundMusic(string audioName, float volume, bool loop = false)
    {
        bool found = false;

        foreach (AudioClip clip in audioClips)
        {
            if (clip.name == audioName)
            {
                found = true;

                backgroundMusic.clip = clip;
                backgroundMusic.volume = volume;
                backgroundMusic.loop = loop;
                backgroundMusic.Play();

                return found;
            }
        }

        if (!found)
        {
            Debug.LogWarning("Warning: No audio clip named " + audioName + " was found!");
        }

        return found;
    }

    public bool PlayAudio2D(string audioName, float volume, bool loop = false)
    {
        bool found = false;

        foreach (AudioClip clip in audioClips)
        {
            if (clip.name == audioName)
            {
                found = true;

                audioSource2D.clip = clip;
                audioSource2D.volume = volume;
                audioSource2D.loop = loop;
                audioSource2D.Play();

                return found;
            }
        }

        if (!found)
        {
            Debug.LogWarning("Warning: No audio clip named " + audioName + " was found!");
        }

        return found;
    }

    public bool PlayAudio3D(string audioName, float volume, Vector3 pos, float fadeDist, float atMaxDist, float dopplerLevel = 0, bool loop = false)
    {
        bool found = false;

        foreach (AudioClip clip in audioClips)
        {
            if (clip.name == audioName)
            {
                found = true;

                audioSource3D.clip = clip;
                audioSource3D.volume = volume;
                audioSource3D.loop = loop;
                audioSource3D.spatialBlend = 1; // full 3D
                audioSource3D.transform.position = pos;
                audioSource3D.maxDistance = fadeDist;
                audioSource3D.minDistance = atMaxDist;
                audioSource3D.rolloffMode = AudioRolloffMode.Linear;
                audioSource3D.dopplerLevel = 0;
                audioSource3D.Play();

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
