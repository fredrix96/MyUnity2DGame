using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioManager
{
    static GameObject go;
    static AudioClip[] audioClips;
    static List<AudioSource> audioSource2DList;
    static List<AudioSource> audioSource3DList;
    static List<AudioSource> backgroundMusicList;

    public static void Init()
    {
        audioClips = Resources.LoadAll<AudioClip>("Audio/");
        go = new GameObject { name = "audio_pool" };
        go.transform.parent = GameManager.GameManagerObject.transform;
        audioSource2DList = new List<AudioSource>();
        audioSource3DList = new List<AudioSource>();
        backgroundMusicList = new List<AudioSource>();
    }

    static AudioSource GetAudioSource(List<AudioSource> list, string audioName)
    {
        AudioSource audioSource = null;

        bool sourceExists = false;
        foreach (AudioSource source in list)
        {
            if (source.clip != null)
            {
                if (source.clip.name == audioName && !source.isPlaying)
                {
                    sourceExists = true;
                    audioSource = source;
                }
            }
        }

        if (!sourceExists)
        {
            AudioSource newAudioSource = go.AddComponent<AudioSource>();
            list.Add(newAudioSource);
            audioSource = newAudioSource;
        }

        return audioSource;
    }

    public static bool PlayBackgroundMusic(string audioName, float volume, bool loop = false)
    {
        bool found = false;

        AudioSource audioSource = GetAudioSource(backgroundMusicList, audioName);

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

    public static bool StopBackgroundMusic(string audioName)
    {
        bool found = false;

        AudioSource audioSource = GetAudioSource(backgroundMusicList, audioName);

        foreach (AudioClip clip in audioClips)
        {
            if (clip.name == audioName)
            {
                found = true;
                audioSource.Stop();

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

        AudioSource audioSource2D = GetAudioSource(audioSource2DList, audioName);

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

        AudioSource audioSource3D = GetAudioSource(audioSource3DList, audioName);

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
