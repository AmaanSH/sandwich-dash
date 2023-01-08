using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AudioEntry
{
    public AudioClip audio;
    public string id;
    public bool looped;
}

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public List<AudioEntry> audios = new List<AudioEntry>();
    public List<AudioSource> sfxSources;
    public List<AudioSource> gameAudioLayers = new List<AudioSource>();

    private int currentLayer = -1;

    public void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void Play(string id)
    {
        AudioEntry entry = audios.Find(x => x.id == id);
        if (entry != null)
        {
            AudioSource sfxSource = sfxSources.Find(x => !x.isPlaying);
            if (sfxSource == null)
            {
                sfxSources[0].Stop();
                sfxSource = sfxSources[0];
            }

            sfxSource.loop = entry.looped;
            sfxSource.clip = entry.audio;
            sfxSource.Play();
        }
    }

    public void StopAllLayers()
    {
        foreach (AudioSource source in gameAudioLayers)
        {
            source.Stop();
        }
    }

    public void PlayAudioLayer()
    {
        if (currentLayer < gameAudioLayers.Count)
        {
            if (currentLayer + 1 < gameAudioLayers.Count)
            {
                currentLayer++;
            }

            StartCoroutine(FadeAudio(gameAudioLayers[currentLayer]));
        }
    }
    
    public void StopAudioLayer()
    {        
        if (currentLayer >= 0)
        {
            AudioSource source = gameAudioLayers[currentLayer];

            StartCoroutine(FadeAudio(source, true));

            if (currentLayer - 1 >= -1)
            {
                currentLayer--;
            }
        }
    }

    public IEnumerator FadeAudio(AudioSource source, bool fadeOut = false)
    {
        float end = fadeOut ? 0 : 1;

        if (!source.isPlaying)
        {
            source.volume = 0;
            source.Play();
        }

        while (source.volume != end)
        {
            source.volume = Mathf.MoveTowards(source.volume, end, Time.deltaTime * 0.5f);
            yield return null;
        }

        // just in case
        source.volume = end;
    }
}
