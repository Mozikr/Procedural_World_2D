using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    private void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.spatialBlend = s.specialBlend;
        }
    }
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.Log("Can't find sound file: " + name);
            return;
        }
        if (!s.source.isPlaying)
        {
            s.source.Play();
        }
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.Log("Can't find sound file: " + name);
            return;
        }
        if (s.source.isPlaying)
        {
            s.source.Stop();
        }

    }

    public void RandomPitch(string name, float minR, float maxR)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Can't find sound file: " + name);
            return;
        }
        s.pitch = Random.Range(minR, maxR);
    }

    public void RandomVolume(string name, float minR, float maxR)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Can't find sound file: " + name);
            return;
        }
        s.volume = Random.Range(minR, maxR);
    }

    public void PlayWithPitch(string name, float pitch)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.Log("Can't find sound file: " + name);
            return;
        }
        if (!s.source.isPlaying)
        {
            s.source.pitch = pitch;
            s.source.Play();
        }
    }
}
