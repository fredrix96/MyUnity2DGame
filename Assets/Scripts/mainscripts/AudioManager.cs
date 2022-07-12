using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioManager
{
    static GameObject go;
    static AudioClip[] audioClips;
    static AudioSource audioSource2D;
    static AudioSource audioSource3D;
    static AudioSource backgroundMusic;

    public static void Init()
    {
        audioClips = Resources.LoadAll<AudioClip>("Audio/");
        go = new GameObject { name = "audio_source" };
        go.transform.parent = GameManager.GameManagerObject.transform;
        backgroundMusic = go.AddComponent<AudioSource>();
        audioSource2D = go.AddComponent<AudioSource>();
        audioSource3D = go.AddComponent<AudioSource>();
    }

    public static bool PlayBackgroundMusic(string audioName, float volume, bool loop = false)
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

    public static bool PlayAudio2D(string audioName, float volume, bool loop = false)
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

    public static bool PlayAudio3D(string audioName, float volume, Vector3 pos, float fadeDist = 30, float atMaxDist = 2, float dopplerLevel = 0, bool loop = false)
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
