using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioManager
{
    static GameObject go;
    static AudioClip[] audioClips;
    static AudioClip[] audioClipsWeapons;
    static List<AudioSource> audioSource2DList;
    static List<AudioSource> audioSource3DList;
    static List<AudioSource> backgroundMusicList;

    public static void Init()
    {
        audioClips = Resources.LoadAll<AudioClip>("Audio/");
        audioClipsWeapons = Resources.LoadAll<AudioClip>("Audio/WeaponSounds");
        go = new GameObject { name = "audio_pool" };
        go.transform.parent = GameManager.GameManagerObject.transform;
        audioSource2DList = new List<AudioSource>();
        audioSource3DList = new List<AudioSource>();
        backgroundMusicList = new List<AudioSource>();
    }

    public static void Update()
    {
        // Destroy sounds that are not playing
        for (int i = 0; i < audioSource3DList.Count; i++)
        {
            if (!audioSource3DList[i].isPlaying)
            {
                Object.DestroyImmediate(audioSource3DList[i].gameObject);
                audioSource3DList.Remove(audioSource3DList[i]);
            }
        }

        for (int i = 0; i < audioSource2DList.Count; i++)
        {
            if (!audioSource2DList[i].isPlaying)
            {
                Object.DestroyImmediate(audioSource2DList[i].gameObject);
                audioSource2DList.Remove(audioSource2DList[i]);
            }
        }
    }

    public static bool PlayBackgroundMusic(string audioName, float volume, bool loop = false)
    {
        bool found = false;

        GameObject newSoundObject = new GameObject(audioName);
        newSoundObject.transform.SetParent(go.transform);

        AudioSource audioSource2D = newSoundObject.AddComponent<AudioSource>();
        audioSource2D.name = audioName;

        foreach (AudioClip clip in audioClips)
        {
            if (clip.name == audioName)
            {
                found = true;

                audioSource2D.clip = clip;
                audioSource2D.volume = volume;
                audioSource2D.loop = loop;
                audioSource2D.Play();

                backgroundMusicList.Add(audioSource2D);

                return found;
            }
        }

        Object.Destroy(newSoundObject);

        if (!found)
        {
            Debug.LogWarning("Warning: No audio clip named " + audioName + " was found!");
        }

        return found;
    }

    public static void StopAllBackgroundMusic()
    {
        foreach (AudioSource audio in backgroundMusicList)
        {
            if (audio.isPlaying)
            {
                audio.Stop();
            }
        }
    }

    public static bool PlayAudio2D(string audioName, float volume, bool loop = false)
    {
        bool found = false;

        GameObject newSoundObject = new GameObject(audioName);
        newSoundObject.transform.SetParent(go.transform);

        AudioSource audioSource2D = newSoundObject.AddComponent<AudioSource>();
        audioSource2D.name = audioName;

        foreach (AudioClip clip in audioClips)
        {
            if (clip.name == audioName)
            {
                found = true;

                audioSource2D.clip = clip;
                audioSource2D.volume = AdjustVolumeIfMany2D(audioSource2DList, audioName, volume);
                audioSource2D.loop = loop;
                audioSource2D.Play();

                audioSource2DList.Add(audioSource2D);

                return found;
            }
        }

        Object.Destroy(newSoundObject);

        if (!found)
        {
            Debug.LogWarning("Warning: No audio clip named " + audioName + " was found!");
        }

        return found;
    }

    public static bool PlayAudio3D(string audioName, float volume, Vector3 pos, float fadeDist = 30, float atMaxDist = 2, float dopplerLevel = 0, bool loop = false)
    {
        bool found = false;

        GameObject newSoundObject = new GameObject(audioName);
        newSoundObject.transform.SetParent(go.transform);

        AudioSource audioSource3D = newSoundObject.AddComponent<AudioSource>();
        audioSource3D.name = audioName;

        foreach (AudioClip clip in audioClips)
        {
            if (clip.name == audioName)
            {
                found = true;

                audioSource3D.transform.position = pos;
                audioSource3D.clip = clip;
                audioSource3D.maxDistance = fadeDist;
                audioSource3D.minDistance = atMaxDist;
                audioSource3D.volume = AdjustVolumeIfMany3D(audioSource3DList, audioName, volume, audioSource3D);
                audioSource3D.loop = loop;
                audioSource3D.spatialBlend = 1; // full 3D
                audioSource3D.rolloffMode = AudioRolloffMode.Linear;
                audioSource3D.dopplerLevel = 0;
                audioSource3D.Play();

                audioSource3DList.Add(audioSource3D);

                return found;
            }
        }

        Object.Destroy(newSoundObject);

        if (!found)
        {
            Debug.LogWarning("Warning: No audio clip named " + audioName + " was found!");
        }

        return found;
    }

    public static bool PlayWeaponsAudio3D(string audioName, float volume, Vector3 pos, float fadeDist = 30, float atMaxDist = 2, float dopplerLevel = 0, bool loop = false)
    {
        bool found = false;

        GameObject newSoundObject = new GameObject(audioName);
        newSoundObject.transform.SetParent(go.transform);

        AudioSource audioSource3D = newSoundObject.AddComponent<AudioSource>();
        audioSource3D.name = audioName;

        foreach (AudioClip clip in audioClipsWeapons)
        {
            if (clip.name == audioName)
            {
                found = true;

                audioSource3D.transform.position = pos;
                audioSource3D.clip = clip;
                audioSource3D.maxDistance = fadeDist;
                audioSource3D.minDistance = atMaxDist;
                audioSource3D.volume = AdjustVolumeIfMany3D(audioSource3DList, audioName, volume, audioSource3D);
                audioSource3D.loop = loop;
                audioSource3D.spatialBlend = 1; // full 3D
                audioSource3D.rolloffMode = AudioRolloffMode.Linear;
                audioSource3D.dopplerLevel = 0;
                audioSource3D.Play();

                audioSource3DList.Add(audioSource3D);

                return found;
            }
        }

        Object.Destroy(newSoundObject);

        if (!found)
        {
            Debug.LogWarning("Warning: No audio clip named " + audioName + " was found!");
        }

        return found;
    }

    static bool CheckIfAudioIsPlaying(List<AudioSource> list, string audioName)
    {
        foreach (AudioSource audioSource in list)
        {
            if (audioSource.name == audioName && audioSource.isPlaying)
            {
                return true;
            }
        }

        return false;
    }

    // Adjust volume depending on the amount of the same sounds
    static float AdjustVolumeIfMany2D(List<AudioSource> list, string audioName, float volume)
    {
        int counter = 0;

        foreach (AudioSource audioSource in list)
        {
            if (audioSource.name == audioName && audioSource.isPlaying)
            {
                counter++;
            }
        }

        float db = 20f * Mathf.Log10(volume);
        db -= counter;
        volume = Mathf.Pow(10, (db / 20));

        return volume;
    }

    // Adjust volume depending on the amount of the same sounds and their distance
    static float AdjustVolumeIfMany3D(List<AudioSource> list, string audioName, float volume, AudioSource currSound)
    {
        int counter = 0;

        foreach (AudioSource audioSource in list)
        {
            if (audioSource.name == audioName && audioSource.isPlaying)
            {
                // Only count the sounds inside the current sound's range
                if (audioSource.transform.position.x < currSound.transform.position.x + currSound.maxDistance / 5 && audioSource.transform.position.x > currSound.transform.position.x - currSound.maxDistance / 5
                    && audioSource.transform.position.y < currSound.transform.position.y + currSound.maxDistance / 5 && audioSource.transform.position.y > currSound.transform.position.y - currSound.maxDistance / 5)
                {
                    counter++;
                }
            }
        }

        float db = 20f * Mathf.Log10(volume);
        db -= counter * 3;
        volume = Mathf.Pow(10, (db / 20));

        return volume;
    }
}
